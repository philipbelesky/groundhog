﻿namespace Groundhog
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Grasshopper.Kernel;
    using Groundhog.Properties;
    using Rhino.Collections;
    using Rhino.Geometry;

    public class ContourCheckGapsComponent : GroundHogComponent
    {
        public ContourCheckGapsComponent()
            : base("Contour Gap Fix", "Contour Gaps", "Checks if contours have gaps, and bridges them if so",
                "Groundhog", "Utilities")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{2d234cdc-ecaa-4ce7-815a-c8136d1798d0}");

        protected override Bitmap Icon => Resources.icon_contour_gap;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Contour Curves", "C", "The contours to check", GH_ParamAccess.list);
            pManager[0].Optional = false;
            pManager.AddCurveParameter("Boundary", "B", "The boundary rectangle to clip to", GH_ParamAccess.item);
            pManager[1].Optional = false;
            pManager.AddNumberParameter("Maximum Distance", "D",
                "The maximum distance allowed as a gap between two contours", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Contours", "C", "The contours with gaps filled in", GH_ParamAccess.list);
            pManager.AddCurveParameter("Joins", "J", "The joins used to fill in the gaps (for reference)",
                GH_ParamAccess.list);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            // Create holder variables for input parameters
            var ALL_CONTOURS = new List<Curve>();
            var BOUNDARY = default(Curve);
            var MAXIMUM_GAP = default(double);

            // Access and extract data from the input parameters individually
            if (!DA.GetDataList(0, ALL_CONTOURS)) return;
            if (!DA.GetData(1, ref BOUNDARY)) return;
            if (!DA.GetData(2, ref MAXIMUM_GAP)) return;

            // Input Validation
            var preCullSize = ALL_CONTOURS.Count;
            ALL_CONTOURS.RemoveAll(item => item == null);
            if (ALL_CONTOURS.Count == 0)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No valid contour curves were provided.");
                return;
            }

            if (ALL_CONTOURS.Count < preCullSize)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Remark,
                    string.Format(
                        "{0} contours were removed because they were null items — perhaps because they are no longer present in the Rhino model.",
                        preCullSize - ALL_CONTOURS.Count));
            }

            // Ensure the maximum gap is a positive number (won't crash if not; but just does nothing)
            if (MAXIMUM_GAP <= 0.0)
            {
                DA.SetDataList(0, ALL_CONTOURS);
                DA.SetDataList(1, null);
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                    "Maximum Distance was set at or below 0 so no changes were made to the contours.");
                return;
            }

            // Create holder variables for ouput parameters
            var fixedContours = new List<Curve>();
            var connections = new List<Curve>();

            // For each contour, get start and end points that arent outside or on the boundaries
            var possibleSplitPoints = new List<Point3d>();
            foreach (var contour in ALL_CONTOURS)
            {
                fixedContours.Add(contour);
                if (contour.IsClosed == false)
                {
                    var pointContainmentS = BOUNDARY.Contains(contour.PointAtStart, Plane.WorldXY, this.docUnitTolerance);
                    if (pointContainmentS.ToString() == "Inside")
                        possibleSplitPoints.Add(contour.PointAtStart);

                    var pointContainmentE = BOUNDARY.Contains(contour.PointAtEnd, Plane.WorldXY, this.docUnitTolerance);
                    if (pointContainmentE.ToString() == "Inside")
                        possibleSplitPoints.Add(contour.PointAtEnd);
                }
            }

            if (possibleSplitPoints != null && possibleSplitPoints.Count > 0)
            {
                // For each point that needs to be connected, find the closest non-self point, and creating a joining line
                var joinedPoints = new List<Point3d>();
                foreach (var point in possibleSplitPoints)
                    if (joinedPoints.Contains(point) == false)
                    {
                        var possibleSplitPointsWithoutSelf =
                            new List<Point3d>(possibleSplitPoints); // clone the current list
                        possibleSplitPointsWithoutSelf.Remove(point); // remove the item we are searching from

                        if (possibleSplitPointsWithoutSelf.Count == 0) continue;

                        // Get the closest point on the boundary and check we aren't extending to a boundary gap
                        var closestPoint = Point3dList.ClosestPointInList(possibleSplitPointsWithoutSelf, point);
                        BOUNDARY.ClosestPoint(point, out var closestBoundaryPtParamater);
                        var closestBoundaryPt = BOUNDARY.PointAt(closestBoundaryPtParamater);
                        closestBoundaryPt.Z = point.Z; // Set the Z's to be the same as the boundary is assumed to be 3D

                        // Don't proceed if the closest point has already been joined
                        if (joinedPoints.Contains(closestPoint) == false)
                            if (point.DistanceTo(closestPoint) < point.DistanceTo(closestBoundaryPt))
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

            // Assign variables to output parameters
            DA.SetDataList(0, Curve.JoinCurves(fixedContours));
            DA.SetDataList(1, connections);
        }

        private double CalculateMean(List<double> values)
        {
            var count = values.Count;

            if (count == 0) throw new InvalidOperationException("Empty collection");
            return values[Convert.ToInt32(count / 2)];
        }

        private List<double> GetZValues(List<Point3d> controlPts)
        {
            var zValues = new List<double>();
            foreach (var point in controlPts)
                zValues.Add(point.Z);
            zValues.Sort();
            return zValues;
        }
    }
}