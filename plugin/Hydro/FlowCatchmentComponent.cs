﻿namespace Groundhog
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Net.Http.Headers;
    using Grasshopper;
    using Grasshopper.Kernel;
    using Grasshopper.Kernel.Geometry;
    using Grasshopper.Kernel.Geometry.Voronoi;
    using Groundhog.Properties;
    using Rhino.Collections;
    using Rhino.Display;
    using Rhino.Geometry;
    using Rhino.Render.UI;
    using Plane = Rhino.Geometry.Plane;

    public class FlowCatchmentComponent : GroundHogComponent
    {
        private bool boundariesAroundEndPoints;
        private List<Curve> providedFlowPaths;
        private double providedMinProximity = 0;

        public FlowCatchmentComponent()
            : base("Flow Catchments", "Catchments", "Identify the catchments within a set of flow paths", "Groundhog",
                "Hydro")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{2d241bdc-ecaa-4cf3-815a-c8001d1798d1}");

        protected override Bitmap Icon => Resources.icon_flows_catchments;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Flow Paths", "C", "The flow paths as generated by the flows component",
                GH_ParamAccess.list);
            pManager[0].Optional = false;
            pManager.AddNumberParameter("Proximity Threshold", "T", "The distance between end points required to form a catchment",
                GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Catchments", "B", "The catchment boundaries identified", GH_ParamAccess.tree);
            pManager.AddCurveParameter("Flow Paths", "P", "The flow paths grouped by catchment", GH_ParamAccess.tree);
            pManager.AddColourParameter("Color Codes", "C", "Colour codes the uniquely identify each path and boundary",
                GH_ParamAccess.tree);
            pManager.AddNumberParameter("Volumes %", "V", "The % of the total flow paths that drain to this area",
                GH_ParamAccess.tree);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            this.providedFlowPaths = new List<Curve>();
            DA.GetDataList(0, this.providedFlowPaths);
            this.providedFlowPaths.RemoveAll(curve => curve == null); // Remove null items; can be due to passing in the points not the path
            if (this.providedFlowPaths.Count == 0)
            {
                this.AddRuntimeMessage(
                    GH_RuntimeMessageLevel.Error,
                    "No Flow Paths provided or they were provided as an inappropriate geometry.");
                return;
            }

            DA.GetData(1, ref this.providedMinProximity);
            if (this.providedMinProximity < 0)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Proximity threshold must be a positive number.");
                return;
            }
            else if (this.providedMinProximity == 0)
            {
                Polyline samplePath; // Attempt to guess a good proximity amount
                if (this.providedFlowPaths[0].TryGetPolyline(out samplePath))
                {
                    this.providedMinProximity = samplePath.SegmentAt(0).Length * 2;
                    this.AddRuntimeMessage(
                        GH_RuntimeMessageLevel.Remark,
                        $"Proximity Threshold parameter not provided; guessed {this.providedMinProximity} as a good value");
                }
            }

            // End initial variable setup

            // Get end points of all flowPaths
            var flowPathEnds = this.providedFlowPaths.Select(t => t.PointAtEnd).Distinct();

            // Classify all of the end points of the flow paths into groups based on their proximity to each other
            // Just use the Group Points component via node in code to do this sorting
            var groupPointsInfo = Rhino.NodeInCode.Components.FindComponent("PointGroups");
            var groupPointsFunction = groupPointsInfo.DelegateTree as dynamic;
            var groupPointsResults = groupPointsFunction(flowPathEnds, this.providedMinProximity);
            var flowPathEndsGrouped = groupPointsResults[0] as DataTree<object>; // A tree (group then item)
            var flowGroupIndicesforPaths = groupPointsResults[1] as DataTree<object>; // A tree (group then item)
            int flowGroupsCount = flowPathEndsGrouped.BranchCount;

            // Create a Voronoi diagram based on the end points of all flowPaths
            Rhino.Geometry.Rectangle3d boundaryForVoronoi = this.GetBoundary(flowPathEnds as IEnumerable<Point3d>);
            var boundaryNodes = new Node2List(boundaryForVoronoi.ToPolyline().ToList());
            var voronoiCells = Solver.Solve_BruteForce(new Node2List(flowPathEnds), boundaryNodes);
            var voronoiResults = voronoiCells.Select(cell => cell.ToPolyline()).ToList();

            // Create the branch structures where each path is a catchment group and add the relevant geometry
            var groupedCurves = new DataTree<Curve>();
            var groupedBounds = new DataTree<Curve>();
            var groupedColors = new DataTree<Color>();
            var groupedVolumes = new DataTree<double>();

            for (var groupIndex = 0; groupIndex < flowGroupsCount; groupIndex++)
            {
                // Setup paths for output data trees
                var nextPath = groupedCurves.Paths.Count;
                groupedCurves.EnsurePath(nextPath);
                groupedBounds.EnsurePath(nextPath);
                groupedColors.EnsurePath(nextPath);
                groupedVolumes.EnsurePath(nextPath);

                // Gather all the original flow paths and voronoi cells based on their grouped index
                var indicesForGroup = flowGroupIndicesforPaths.Branch(groupIndex).Cast<int>();
                var voronoiCellsForGroup = new List<Polyline>(); // Temporary grouping to avoid casting issues
                foreach (var pathGroupIndex in indicesForGroup)
                {
                    groupedCurves.Add(this.providedFlowPaths[pathGroupIndex], groupedCurves.Path(nextPath));
                    voronoiCellsForGroup.Add(voronoiResults[pathGroupIndex]);
                }

                var numberOfFlowPaths = groupedCurves.Branch(nextPath).Count;

                // Generate a distinct color for this group and populate the count
                var colors = this.GenerateGroupColors(groupIndex, flowGroupIndicesforPaths.Branch(groupIndex).Count, flowGroupsCount);
                groupedColors.AddRange(colors, groupedColors.Path(nextPath));

                // Merge together all of the voronoi cells based on their proximity
                if (voronoiCellsForGroup.Count > 1)
                {
                    // Extract all edges from the cells in group into a single list
                    var cellEdgesForGroup = new List<Line>();
                    foreach (var cell in voronoiCellsForGroup)
                    {
                        foreach (var edge in cell.GetSegments())
                        {
                            if (edge.From > edge.To)
                            {
                                edge.Flip(); // Align lines to make next stage's equality check easier
                            }

                            cellEdgesForGroup.Add(edge);
                        }
                    }

                    // Remove duplicate edges from the list (i.e. inner cell boundaries)
                    var cellEdgesForHull = new List<Curve>();
                    for (int i = 0; i <= cellEdgesForGroup.Count - 1; i++)
                    {
                        var dupe = false;
                        for (int j = 0; j <= cellEdgesForGroup.Count - 1; j++)
                        {
                            if (i == j)
                                continue;

                            var testA = Math.Abs(cellEdgesForGroup[i].From.DistanceTo(cellEdgesForGroup[j].From));
                            var testB = Math.Abs(cellEdgesForGroup[i].To.DistanceTo(cellEdgesForGroup[j].To));
                            if (testA <= this.docUnitTolerance && testB <= this.docUnitTolerance)
                            {
                                dupe = true;
                                break;
                            }
                        }

                        if (!dupe)
                            cellEdgesForHull.Add(cellEdgesForGroup[i].ToNurbsCurve());
                    }

                    var groupedEdges = Curve.JoinCurves(cellEdgesForHull);
                    groupedBounds.AddRange(groupedEdges, groupedBounds.Path(nextPath));
                }
                else
                {
                    groupedBounds.Add(voronoiCellsForGroup.First().ToNurbsCurve(), groupedBounds.Path(nextPath));
                }

                // Calculate flow volumes via proportion of curves in given path
                double flowVolumesPercent = (double)numberOfFlowPaths / this.providedFlowPaths.Count;
                groupedVolumes.Add(flowVolumesPercent, groupedVolumes.Path(nextPath));
            }

            // Assign variables to output parameters
            DA.SetDataTree(0, groupedBounds);
            DA.SetDataTree(1, groupedCurves);
            DA.SetDataTree(2, groupedColors);
            DA.SetDataTree(3, groupedVolumes);
        }

        private List<Color> GenerateGroupColors(int groupIndex, int groupSize, int groupsCount)
        {
            // Here are want to create a rectangularly shaped matrix to try and maximise contrast
            // Given an area of X, we want to find x/y lengths given an 2:1 ratio
            var ratio = groupsCount / 2.0;
            var square = Math.Sqrt(ratio);
            var xMax = (int)Math.Floor(square * 2.0) + 1;
            var yMax = (int)Math.Floor(square * 1.0);

            // Once we have the lengths we go from the index to the x/y position
            int x;
            if (groupIndex == 0)
                x = 0;
            else
                x = groupIndex % xMax; // Get position on x axis

            int y;
            if (groupIndex == 0)
            {
                y = 0;
            }
            else
            {
                double yPos = groupIndex / xMax;
                y = (int)Math.Floor(yPos); // Get position on y axis
            }

            // Create a color from within a given range (set bounds to ensure things are relatively bright/distinct)
            var hue = this.ColorDistributionInRange(0.0, 1.0, x, xMax); // Not -1 as 0.0 and 1.0 are equivalent
            var saturation = 1.0; // Maximise contrast
            var luminance = this.ColorDistributionInRange(0.2, 0.6, y, yMax - 1); // -1 as we want to use the full range or 0.2-0.6
            var groupColorHSL = new ColorHSL(hue, saturation, luminance);

            // Convert to RGB and make a list with a color for each item in the branch
            var groupColorARGB = groupColorHSL.ToArgbColor();
            var groupColors = Enumerable.Repeat(groupColorARGB, groupSize).ToList();

            return groupColors;
        }

        private double ColorDistributionInRange(double lower, double upper, int index, int count)
        {
            var step = (upper - lower) / Math.Max(count, 1.0); // Max to prevent division by infinity
            var value = lower + (step * index);
            return value;
        }

        private Rhino.Geometry.Rectangle3d GetBoundary(IEnumerable<Point3d> points)
        {
            var sortedX = points.OrderBy(item => item.X);
            var sortedY = points.OrderBy(item => item.Y);
            var sortedZ = points.OrderBy(item => item.Z);
            var width = sortedX.Last().X - sortedX.First().X;
            var height = sortedY.Last().Y - sortedY.First().Y;

            // We need to offset the rectangle slightly so that the voronoi points aren't right on the boundary. To do so we just shift things over by a small amount
            var offset = 0.01;
            Point3d shiftedOrigin = new Point3d(sortedX.First().X - (width * offset), sortedY.First().Y - (height * offset), sortedZ.First().Z);
            Plane plane = new Plane(shiftedOrigin, new Vector3d(0, 0, 1));
            return new Rectangle3d(plane, width * (1 + (offset * 2)), height * (1 + (offset * 2)));
        }
    }
}