using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace badger
{
    public class badgerContourClipComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public badgerContourClipComponent()
            : base("Contour Clip", "Contour Clip",
                "Checks contours are planar, corrects them if not",
                "Badger", "Terrain")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Contour Curves", "C", "The contours to clip", GH_ParamAccess.list);
            pManager[0].Optional = false;
            pManager.AddCurveParameter("Boundary", "B", "The boundary rectangle to clip to", GH_ParamAccess.item);
            pManager[1].Optional = false;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Contours", "C", "The clipped contours", GH_ParamAccess.list); 
            pManager.AddCurveParameter("Failed Contours", "F", "The contours that couldn't be clipped", GH_ParamAccess.list);
         }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {            
            // Create holder variables for input parameters
            List<Curve> ALL_CONTOURS = new List<Curve>();
            Curve BOUNDARY = default(Curve);
            
            // Access and extract data from the input parameters individually
            if (!DA.GetDataList(0, ALL_CONTOURS)) return;
            if (!DA.GetData(1, ref BOUNDARY)) return;

            // Create holder variables for ouput parameters
            List<Curve> fixedContours = new List<Curve>();
            List<Curve> failedContours = new List<Curve>();

            List<double> heightGuages = new List<double>();
            foreach (Curve curve in ALL_CONTOURS) {
              heightGuages.Add(curve.PointAtEnd.Z);
            }
            heightGuages.Sort();

            double contourLow = heightGuages[0];
            double contourHigh = heightGuages[heightGuages.Count - 1];

            // Move plane to lowest point
            double boundaryZ = BOUNDARY.PointAtEnd.Z;
            double boundaryMove;
            if (boundaryZ < contourLow) {
              boundaryMove = boundaryZ - contourLow;
            } else if (boundaryZ > contourLow) {
              boundaryMove = contourLow - boundaryZ;
            } else {
              boundaryMove = 0;
            }

            // Extrude up to highest - lowest
            BOUNDARY.Transform(Transform.Translation(new Vector3d(0, 0, boundaryMove - 1)));

            Vector3d boundaryExtrusion = new Vector3d(0, 0, contourHigh - contourLow + 2);
            Surface boundarySrf = Surface.CreateExtrusion(BOUNDARY, boundaryExtrusion);

            // Clip
            double tolerance = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            foreach (Curve curve in ALL_CONTOURS)
            {
                Rhino.Geometry.Intersect.CurveIntersections ci = Rhino.Geometry.Intersect.Intersection.CurveSurface(curve, boundarySrf, tolerance, tolerance);

                if (ci.Count > 2)
                {
                    if (ci.Count % 2 == 0)
                    {
                        List<Curve> insideSegments = new List<Curve>();
                        for (int i = 0; i < ci.Count; i = i + 2)
                        {
                            Curve trim = curve.Trim(ci[i].OverlapA.T0, ci[i + 1].OverlapA.T1);
                            insideSegments.Add(trim);
                        }
                        foreach (Curve segment in insideSegments)
                        {
                            fixedContours.Add(segment);
                        }
                    }
                    else
                    {
                        failedContours.Add(curve);
                        //Print("2+ odd");
                        //is odd
                    }

                }
                else if (ci.Count == 2)
                {
                    // Simple two point overlap
                    Curve trim = curve.Trim(ci[0].OverlapA.T0, ci[1].OverlapA.T1);
                    fixedContours.Add(trim);
                }
                else if (ci.Count == 1)
                {
                    //Print("1");

                    // Split into two; grab the larger inner end
                    Curve[] splitCurves = curve.Split(ci[0].OverlapA.T0);
                    Curve insideSegment;
                    if (splitCurves[0].GetLength() > splitCurves[1].GetLength())
                    {
                        insideSegment = splitCurves[0];
                    }
                    else
                    {
                        insideSegment = splitCurves[1];
                    }

                    // TODO: deduplicate this with the below function
                    double u;
                    double v;

                    Point3d start = insideSegment.PointAtStart;
                    boundarySrf.ClosestPoint(start, out u, out v);
                    Point3d srfStartEnd = boundarySrf.PointAt(u, v);
                    Line startCrv = new Line(start, srfStartEnd);

                    Point3d end = insideSegment.PointAtEnd;
                    boundarySrf.ClosestPoint(end, out u, out v);
                    Point3d srfEndEnd = boundarySrf.PointAt(u, v);
                    Line endCrv = new Line(end, srfEndEnd);

                    var toJoin = new List<Curve> { startCrv.ToNurbsCurve(), insideSegment, endCrv.ToNurbsCurve() };
                    Curve[] joinedCrv = Curve.JoinCurves(toJoin);

                    if (joinedCrv.Length == 1)
                    {
                        fixedContours.Add(joinedCrv[0]); // Its an island; no need to intersect
                    }

                    // Extend both

                }
                else if (ci.Count == 0)
                {
                    if (curve.IsClosed == true)
                    {
                        fixedContours.Add(curve); // Its an island; no need to intersect
                    }
                    else
                    {
                        double u;
                        double v;

                        Point3d start = curve.PointAtStart;
                        boundarySrf.ClosestPoint(start, out u, out v);
                        Point3d srfStartEnd = boundarySrf.PointAt(u, v);
                        Line startCrv = new Line(start, srfStartEnd);

                        Point3d end = curve.PointAtEnd;
                        boundarySrf.ClosestPoint(end, out u, out v);
                        Point3d srfEndEnd = boundarySrf.PointAt(u, v);
                        Line endCrv = new Line(end, srfEndEnd);

                        var toJoin = new List<Curve> { startCrv.ToNurbsCurve(), curve, endCrv.ToNurbsCurve() };
                        Curve[] joinedCrv = Curve.JoinCurves(toJoin);

                        if (joinedCrv.Length == 1)
                        {
                            fixedContours.Add(joinedCrv[0]); // Its an island; no need to intersect
                        }

                    }
                    // Extend both
                }

            }

            // Assign variables to output parameters
            DA.SetDataList(0, fixedContours);
            DA.SetDataList(1, failedContours);
        }



           /// <summary>
        /// The Exposure property controls where in the panel a component icon 
        /// will appear. There are seven possible locations (primary to septenary), 
        /// each of which can be combined with the GH_Exposure.obscure flag, which 
        /// ensures the component will only be visible on panel dropdowns.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return badger.Properties.Resources.icon_pplacer;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{2d234adc-dcfd-4cf7-815a-c8136d1798d0}"); }
        }
    }
}
