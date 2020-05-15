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
    public class GroundhogContourClipComponent : GroundHogComponent
    {
        public GroundhogContourClipComponent()
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
            
            // Input Validation
            int preCullSize = ALL_CONTOURS.Count;
            ALL_CONTOURS.RemoveAll(item => item == null);
            if (ALL_CONTOURS.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No valid contour curves were provided.");
                return;
            }
            else if (ALL_CONTOURS.Count < preCullSize)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, String.Format("{0} contours were removed because they were null items — perhaps because they are no longer present in the Rhino model.", preCullSize - ALL_CONTOURS.Count));
            }

            // Get lowest point
            var heightGuages = new List<double>();
            foreach (var curve in ALL_CONTOURS)
            {
                heightGuages.Add(curve.PointAtEnd.Z);
            }
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
            
            // Create holder variables for ouput parameters
            var fixedContours = new List<Curve>();
            var edgedContours = new List<Curve>();
            var planarSrfs = new List<Brep>();

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
                    var planarSurfaces = Brep.CreatePlanarBreps(allContours[i], docUnitTolerance); // Create planar surfaces
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

            var inBoundary = BOUNDARY.Contains(testPoint, Plane.WorldXY, docUnitTolerance);

            if (inBoundary == PointContainment.Inside)
            {
                // Extend
                var extendedCurve = ExtendCurveTerminusToBoundary(initialCurve, point, boundarySrf);
                if (extendedCurve != null) // Extension presumably fails due to no extendable path
                {
                    return extendedCurve;
                }
            }
            return initialCurve;
        }

        private Curve ClipCurveEndsToBoundary(Curve initialCurve, Point3d targetPoint, Surface boundarySrf)
        {
            Curve trimmedCurveEnds;

            // Find where the boundary and curve intersect
            Intersection.CurveBrep(initialCurve, boundarySrf.ToBrep(), docUnitTolerance, 
                                   out Curve[] intersectCurves, out Point3d[] intersectPoints);

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
            initialCurve.ClosestPoint(closestPoint, out double startTrim, 0); // Get Paramter of Intersection

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

            if (startPoint.DistanceTo(initialCurve.PointAtEnd) == 0)
                return initialCurve.Extend(CurveEnd.End, CurveExtensionStyle.Smooth, boundaryCollision);
            else
                return initialCurve.Extend(CurveEnd.Start, CurveExtensionStyle.Smooth, boundaryCollision);
        }

        private List<Curve> ClipMeanderingCurvesToBoundary(Curve initialCurve, Curve BOUNDARY, Surface boundarySrf)
        {
            var returnCurves = new List<Curve>();

            var splitCurves = initialCurve.Split(boundarySrf, docUnitTolerance, docAngularTolerance);

            if (splitCurves.Length > 0)
            {
                for (var i = 0; i < splitCurves.Length; i = i + 1)
                {
                    var testPoint = splitCurves[i].PointAt(splitCurves[i].Domain.Mid);
                    testPoint.Z = BOUNDARY.PointAtEnd.Z;

                    var pointContainment = BOUNDARY.Contains(testPoint, Plane.WorldXY, docUnitTolerance);

                    if (pointContainment.ToString() == "Inside")
                        returnCurves.Add(splitCurves[i]);
                }
            }
            else
            {
                // There are no intersections with the boundary. This could be because it is entirely outside the boundary though
                // Check this and return None if that is the case
                var evaluationPoint = initialCurve.PointAt(initialCurve.Domain.T0);
                var containmentCheck = BOUNDARY.Contains(evaluationPoint, Plane.WorldXY, docUnitTolerance);
                if (containmentCheck.ToString() == "Outside")
                    returnCurves.Add(null); // Is completely outside the clipped area
                else
                    returnCurves.Add(initialCurve); // Isn't completely outiside

            }
            return returnCurves;
        }

        private Curve GetBoundedContour(Curve initialCurve, Curve BOUNDARY)
        {
            // Move the boundary down to the same plane so we can do a curve curve intersection
            BOUNDARY.Transform(Transform.Translation(new Vector3d(0, 0, initialCurve.PointAtEnd.Z - BOUNDARY.PointAtEnd.Z)));

            var ccx = Intersection.CurveCurve(initialCurve, BOUNDARY, docUnitTolerance, docUnitTolerance);

            // Used to be 0. It is possible to have a ccx array of only 1 element (which will crash) but unclear what that signifies
            if (ccx.Count > 1)
            {
                var innerEdgeA = BOUNDARY.Trim(ccx[0].ParameterB, ccx[1].ParameterB); // remove before t0; after t1
                var innerEdgeB = BOUNDARY.Trim(ccx[1].ParameterB, ccx[0].ParameterB); // remove before t0; after t1

                // This is going to be incorrect sometimes, but we want to get the shorter of the two pieces
                if (innerEdgeA.GetLength() >= innerEdgeB.GetLength())
                {
                    var innerEdge = Curve.JoinCurves(new[] {innerEdgeB, initialCurve}, docUnitTolerance);
                    return innerEdge[0];
                }
                else
                {
                    var innerEdge = Curve.JoinCurves(new[] {innerEdgeA, initialCurve}, docUnitTolerance);
                    return innerEdge[0];
                }
            }
            return initialCurve;
        }
    }
}