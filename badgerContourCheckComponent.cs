using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace badger
{
    public class badgerContourCheckComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public badgerContourCheckComponent()
            : base("Contour Fix", "Contour Fix",
                "Checks contours are planar, corrects them if not",
                "Badger", "Terrain")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Contour Curves", "C", "The contours to check", GH_ParamAccess.list);
            pManager[0].Optional = false;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("All Contours", "AC", "The contours that were fixed", GH_ParamAccess.list);
            pManager.AddCurveParameter("Fixed Contours", "FC", "The contours that were fixed", GH_ParamAccess.list);
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
            
            // Access and extract data from the input parameters individually
            if (!DA.GetDataList(0, ALL_CONTOURS)) return;

            // Create holder variables for ouput parameters
            List<Curve> fixedContours = new List<Curve>();
            List<Curve> allContours = new List<Curve>();

            foreach (Curve contour in ALL_CONTOURS)
            {
                int degree = contour.Degree;
                if (contour.IsPolyline()) {
                    
                    Polyline contourPLine;
                    contour.TryGetPolyline(out contourPLine); // Convert to Polyline

                    List<double> zValues = getZValues(new List<Point3d>(contourPLine.ToArray())); 

                    if (zValues[0] != zValues[zValues.Count - 1]) {
                        // All are not the same z-index
                        double medianZ = calculateMean(zValues);

                        List<Point3d> newPoints = new List<Point3d>();
                        foreach (Point3d point in contourPLine)
                        {
                            Point3d newPoint = point;
                            newPoint.Z = medianZ;
                            newPoints.Add(newPoint);
                        }

                        PolylineCurve fixedPolyline = new PolylineCurve(newPoints);
                        fixedContours.Add(fixedPolyline);
                        allContours.Add(fixedPolyline);

                    }
                    else
                    {
                        allContours.Add(contour);
                    }

                }
                else
                {
                    // TODO: probably shouldn't just assume the curve is nurbs
                    NurbsCurve contourNurbsCurve = contour.ToNurbsCurve();
                    
                    List<Point3d> pts = new List<Point3d>();
                    foreach (ControlPoint ncp in contourNurbsCurve.Points)
                    {
                        pts.Add(ncp.Location);
                    }

                    List<double> zValues = getZValues(pts);

                    if (zValues[0] != zValues[zValues.Count - 1])
                    {
                        double medianZ = calculateMean(zValues);

                        for (int index = 0; index < contourNurbsCurve.Points.Count; index++)
                        {
                            Point3d tempPt = contourNurbsCurve.Points[index].Location;
                            double tempWeight = contourNurbsCurve.Points[index].Weight;
                            tempPt.Z = medianZ;
                            contourNurbsCurve.Points[index] = new ControlPoint(tempPt, tempWeight);
                        }
                        fixedContours.Add(contourNurbsCurve);
                        allContours.Add(contourNurbsCurve);
                    }
                    else
                    {
                        allContours.Add(contour);
                    }

   
                }


            }

            // Assign variables to output parameters
            DA.SetDataList(0, allContours);
            DA.SetDataList(1, fixedContours);
        }

        private double calculateMean(List<double> values)
        {
            int count = values.Count;

            if (count == 0) {
                throw new InvalidOperationException("Empty collection");
            } else {
                // Doesn't matter if odd or evenly sized, will just get middleish
                return values[Convert.ToInt32(count / 2)]; 
            }
        }

        private List<double> getZValues(List<Point3d> controlPts)
        {
            List<double> zValues = new List<double>();
            foreach (Point3d point in controlPts)
            {
                zValues.Add(point.Z);
            }
            zValues.Sort();
            return zValues;
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
            get { return new Guid("{2d234bdc-ecaa-4cf7-815a-c8136d1798d0}"); }
        }
    }
}
