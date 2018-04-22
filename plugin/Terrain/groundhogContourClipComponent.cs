using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using groundhog.Properties;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;

namespace groundhog
{
    public class groundhogContourClipComponent : GroundHog_Component
    {
        public groundhogContourClipComponent()
            : base("Contour Clipper", "Contour Clip", "Checks contours meet a specific boundary, otherwise extend/trim them", "Groundhog", "Terrain")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.icon_contour_clip;

        public override Guid ComponentGuid => new Guid("{2d234adc-dcfd-4cf7-815a-c8136d1798d0}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Contour Curves", "C", "The contours to clip", GH_ParamAccess.list);
            pManager[0].Optional = false;
            pManager.AddCurveParameter("Boundary", "B", "The boundary rectangle to clip to", GH_ParamAccess.item);
            pManager[1].Optional = false;
            pManager.AddBooleanParameter("Create PlanarSrfs", "P", "Whether to create planar surfaces; may be slow with large quantities of contours!", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Contours", "C", "The clipped contours", GH_ParamAccess.list);
            pManager.AddCurveParameter("Edged Contours", "E", "All contours with edges following the boundary", GH_ParamAccess.list);
            pManager.AddBrepParameter("Planar Surfaces", "P", "Edge contours as planar surfaces (must be toggled on)", GH_ParamAccess.list);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            // Create holder variables for input parameters
            var ALL_CONTOURS = new List<Curve>();
            var BOUNDARY = default(Curve);
            var CREATE_SRFS = false;

            // Access and extract data from the input parameters individually
            if (!DA.GetDataList(0, ALL_CONTOURS)) return;
            if (!DA.GetData(1, ref BOUNDARY)) return;
            DA.GetData(2, ref CREATE_SRFS);
            
            if (ALL_CONTOURS.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No contour curves provided.");
                return;
            }

            // Create holder variables for ouput parameters
            var fixedContours = new List<Curve>();
            var edgedContours = new List<Curve>();
            var planarSrfs = new List<Brep>();

            // Get lowest point
            var heightGuages = new List<double>();
            foreach (var curve in ALL_CONTOURS)
                heightGuages.Add(curve.PointAtEnd.Z);
            heightGuages.Sort();

            var contourLow = heightGuages[0];
            var contourHigh = heightGuages[heightGuages.Count - 1];

            // Move plane to lowest point
            var boundaryZ = BOUNDARY.PointAtEnd.Z;
            double boundaryMove;
            if (boundaryZ < contourLow)
                boundaryMove = boundaryZ - contourLow;
            else if (boundaryZ > contourLow)
                boundaryMove = contourLow - boundaryZ;
            else
                boundaryMove = 0;

            // Extrude up to highest - lowest
            BOUNDARY.Transform(Transform.Translation(new Vector3d(0, 0, boundaryMove - 1)));

            var boundaryExtrusion = new Vector3d(0, 0, contourHigh - contourLow + 2);
            var boundarySrf = Surface.CreateExtrusion(BOUNDARY, boundaryExtrusion);

            // End Point Clipping
            foreach (var curve in ALL_CONTOURS)
            {
                if (curve.IsValid == false)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                        "A contour curve was not valid and has been skipped. You probably want to find and fix that curve.");
                    continue;
                }

                Curve curveWithEndClipped;
                if (curve.IsClosed == false)
                {
                    var curveWithStartClipped = ClipCurveTerminus(curve, curve.PointAtStart, BOUNDARY, boundarySrf);
                    curveWithEndClipped = ClipCurveTerminus(curveWithStartClipped, curve.PointAtEnd, BOUNDARY,
                        boundarySrf);
                }
                else
                {
                    curveWithEndClipped = curve;
                }

                var curveWithMiddlesClipped = ClipMeanderingCurvesToBoundary(curveWithEndClipped, BOUNDARY, boundarySrf);
                foreach (var curveClip in curveWithMiddlesClipped)
                {
                    if (curveClip == null)
                        continue; // Null if the curve is totally outside the boundary

                    fixedContours.Add(curveClip);
                    var edgedContour = GetBoundedContour(curveClip, BOUNDARY); // Create the profiles matching the boundary
                    edgedContours.Add(edgedContour);
                }
            }

            if (CREATE_SRFS)
            {
                var allContours = edgedContours.ToArray();
                var planarSrfsArray = new Brep[allContours.Length - 1];
                Parallel.For(0, allContours.Length - 1, i => // Shitty multithreading
                {
                    var planarSurfaces = Brep.CreatePlanarBreps(allContours[i]); // Create planar surfaces
                    planarSrfsArray[i] = planarSurfaces[0];
                });
                planarSrfs = new List<Brep>(planarSrfsArray); // Probably unecessary
            }

            // Assign variables to output parameters
            DA.SetDataList(0, fixedContours);
            DA.SetDataList(1, edgedContours);
            DA.SetDataList(2, planarSrfs);
        }


        private Curve ClipCurveTerminus(Curve initialCurve, Point3d point, Curve BOUNDARY, Surface boundarySrf)
        {
            // Test, for a particular point where it is in relation to boundary and clip curve accordingly

            var testPoint = new Point3d(point.X, point.Y, BOUNDARY.PointAtEnd.Z); // Equalise the Z's for containment check

            var pointContainment = BOUNDARY.Contains(testPoint);

            if (pointContainment.ToString() == "Inside")
            {
                // Extend
                var extendedCurve = ExtendCurveTerminusToBoundary(initialCurve, point, boundarySrf);
                return extendedCurve;
            }
            return initialCurve;
        }

        private Curve ClipCurveEndsToBoundary(Curve initialCurve, Point3d targetPoint, Surface boundarySrf)
        {
            var tolerance = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            Curve trimmedCurveEnds;

            // Find where the boundary and curve intersect
            Curve[] intersectCurves;
            Point3d[] intersectPoints;
            Intersection.CurveBrep(initialCurve, boundarySrf.ToBrep(), tolerance, out intersectCurves, out intersectPoints);

            // Get closest point from intersections and its parameters
            double? minimumDistance = null;
            var index = -1;
            for (var i = 0; i < intersectPoints.Length; i++)
            {
                var thisNum = intersectPoints[i].DistanceTo(targetPoint);
                if (!minimumDistance.HasValue || thisNum < minimumDistance.Value)
                {
                    minimumDistance = thisNum;
                    index = i;
                }
            }
            var closestPoint = intersectPoints[index];
            double startTrim;
            initialCurve.ClosestPoint(closestPoint, out startTrim, 0); // Get Paramter of Intersection

            // Trim the curve bit that over extends
            if (startTrim != 0.0)
            {
                // Is the intersection closer to the start or end curve parameter?
                var t0Distance = initialCurve.PointAt(initialCurve.Domain.T0).DistanceTo(targetPoint);
                var t1Distance = initialCurve.PointAt(initialCurve.Domain.T1).DistanceTo(targetPoint);

                // Trim off the useless bit
                if (t0Distance > t1Distance)
                    trimmedCurveEnds = initialCurve.Trim(initialCurve.Domain.T0, startTrim);
                else
                    trimmedCurveEnds = initialCurve.Trim(startTrim, initialCurve.Domain.T1);
            }
            else
            {
                trimmedCurveEnds = initialCurve;
            }

            return trimmedCurveEnds;
        }

        private Curve ExtendCurveTerminusToBoundary(Curve initialCurve, Point3d startPoint, Surface boundarySrf)
        {
            Brep[] boundaryCollision = {boundarySrf.ToBrep()};

            Curve extendedCurve;
            if (startPoint.DistanceTo(initialCurve.PointAtEnd) == 0)
                extendedCurve = initialCurve.Extend(CurveEnd.End, CurveExtensionStyle.Smooth, boundaryCollision);
            else
                extendedCurve = initialCurve.Extend(CurveEnd.Start, CurveExtensionStyle.Smooth, boundaryCollision);

            return extendedCurve;
        }

        private List<Curve> ClipMeanderingCurvesToBoundary(Curve initialCurve, Curve BOUNDARY, Surface boundarySrf)
        {
            var tolerance = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            var returnCurves = new List<Curve>();

            var splitCurves = initialCurve.Split(boundarySrf, tolerance);

            if (splitCurves.Length > 0)
            {
                for (var i = 0; i < splitCurves.Length; i = i + 1)
                {
                    var testPoint = splitCurves[i].PointAt(splitCurves[i].Domain.Mid);
                    testPoint.Z = BOUNDARY.PointAtEnd.Z;

                    var pointContainment = BOUNDARY.Contains(testPoint);

                    if (pointContainment.ToString() == "Inside")
                        returnCurves.Add(splitCurves[i]);
                }
            }
            else
            {
                // There are no intersections with the boundary. This could be because it is entirely outside the boundary though
                // Check this and return None if that is the case
                var evaluationPoint = initialCurve.PointAt(initialCurve.Domain.T0);
                var containmentCheck = BOUNDARY.Contains(evaluationPoint);
                if (containmentCheck.ToString() == "Outside")
                    returnCurves.Add(null); // Is completely outside the clipped area
                else
                    returnCurves.Add(initialCurve); // Isn't completely outiside

            }
            return returnCurves;
        }

        private Curve GetBoundedContour(Curve initialCurve, Curve BOUNDARY)
        {
            var tolerance = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;

            // Move the boundary down to the same plane so we can do a curve curve intersection
            BOUNDARY.Transform(Transform.Translation(new Vector3d(0, 0, initialCurve.PointAtEnd.Z - BOUNDARY.PointAtEnd.Z)));

            var ccx = Intersection.CurveCurve(initialCurve, BOUNDARY, tolerance, tolerance);

            // Used to be 0. It is possible to have a ccx array of only 1 element (which will crash) but unclear what that signifies
            if (ccx.Count > 1)
            {
                var innerEdgeA = BOUNDARY.Trim(ccx[0].ParameterB, ccx[1].ParameterB); // remove before t0; after t1
                var innerEdgeB = BOUNDARY.Trim(ccx[1].ParameterB, ccx[0].ParameterB); // remove before t0; after t1

                // This is going to be incorrect sometimes, but we want to get the shorter of the two pieces
                if (innerEdgeA.GetLength() >= innerEdgeB.GetLength())
                {
                    var innerEdge = Curve.JoinCurves(new[] {innerEdgeB, initialCurve}, tolerance);
                    return innerEdge[0];
                }
                else
                {
                    var innerEdge = Curve.JoinCurves(new[] {innerEdgeA, initialCurve}, tolerance);
                    return innerEdge[0];
                }
            }
            return initialCurve;
        }
    }
}