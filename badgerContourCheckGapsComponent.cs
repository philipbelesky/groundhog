using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace badger
{
    public class badgerContourCheckGapsComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public badgerContourCheckGapsComponent()
            : base("Contour Gap Fix", "Contour Gap Fix",
                "Checks if contours have gaps, and bridges them if so",
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
            pManager.AddCurveParameter("Boundary", "B", "The boundary rectangle to clip to", GH_ParamAccess.item);
            pManager[1].Optional = false;
            pManager.AddNumberParameter("Maximum Distance", "D", "The maximum distance allowed as a gap between two contours", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Contours", "C", "The contours with gaps filled in", GH_ParamAccess.list);
            pManager.AddCurveParameter("Joins", "J", "The joins used to fill in the gaps (for reference)", GH_ParamAccess.list);
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
            double MAXIMUM_GAP = default(double);
            
            // Access and extract data from the input parameters individually
            if (!DA.GetDataList(0, ALL_CONTOURS)) return;
            if (!DA.GetData(1, ref BOUNDARY)) return;
            if (!DA.GetData(2, ref MAXIMUM_GAP)) return;

            // Create holder variables for ouput parameters
            List<Curve> fixedContours = new List<Curve>();
            List<Curve> connections = new List<Curve>();
            

            // For each contour, get start and end points that arent outside or on the boundaries
            List<Point3d> possibleSplitPoints = new List<Point3d>();
            foreach (Curve contour in ALL_CONTOURS)
            {
                fixedContours.Add(contour);
                if (contour.IsClosed == false)
                {
                    Rhino.Geometry.PointContainment pointContainmentS = BOUNDARY.Contains(contour.PointAtStart);
                    if (pointContainmentS.ToString() == "Inside")
                    {
                        possibleSplitPoints.Add(contour.PointAtStart);
                        //Print("{0}", "tets start");
                    }

                    Rhino.Geometry.PointContainment pointContainmentE = BOUNDARY.Contains(contour.PointAtEnd);
                    if (pointContainmentE.ToString() == "Inside")
                    {
                        possibleSplitPoints.Add(contour.PointAtEnd);
                        //Print("{0}", "tets end");
                    }
                }
            }

            // For each point that needs to be connected, find the closest non-self point, and creating a joining line
            List<Point3d> joinedPoints = new List<Point3d>();
            foreach (Point3d point in possibleSplitPoints)
            {
                if (joinedPoints.Contains(point) == false)
                {
                    var possibleSplitPointsWithoutSelf = new List<Point3d>(possibleSplitPoints); // clone the current list
                    possibleSplitPointsWithoutSelf.Remove(point); // remove the item we are searching from

                    // Get the closest point on the boundary and check we aren't extending to a boundary gap
                    var closestPoint = Rhino.Collections.Point3dList.ClosestPointInList(possibleSplitPointsWithoutSelf, point);
                    double closestBoundaryPtParamater;
                    BOUNDARY.ClosestPoint(point, out closestBoundaryPtParamater);
                    Point3d closestBoundaryPt = BOUNDARY.PointAt(closestBoundaryPtParamater);
                    closestBoundaryPt.Z = point.Z; // Set the Z's to be the same as the boundary is assumed to be 3D

                    // Don't proceed if the closest point has already been joined
                    if (joinedPoints.Contains(closestPoint) == false)
                    {
                        // Or if we are a line the hasn't been extended to the boundary
                        if (point.DistanceTo(closestPoint) < point.DistanceTo(closestBoundaryPt))
                        {
                            // Or we are less than the specified tolerance
                            if (point.DistanceTo(closestPoint) <= MAXIMUM_GAP)
                            {
                                Curve connection = new LineCurve(point, closestPoint);
                                connections.Add(connection);
                                fixedContours.Add(connection);
                                joinedPoints.Add(point);
                                joinedPoints.Add(closestPoint);
                            }
                        }
                    }
                }
            }
            
            // Assign variables to output parameters
            DA.SetDataList(0, Curve.JoinCurves(fixedContours));
            DA.SetDataList(1, connections);
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
            get { return new Guid("{2d234cdc-ecaa-4ce7-815a-c8136d1798d0}"); }
        }
    }
}
