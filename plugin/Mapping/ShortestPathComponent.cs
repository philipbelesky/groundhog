using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using groundhog.Properties;
using Rhino.Geometry;
using ShortestWalk.Geometry;
using ShortestWalk.Gh;

// Component source lightly modified from Giulio Piacentino's repo, see License/Attribution in /ShortestWalk/

namespace groundhog
{
    public class GroundhogShortestPathComponent : GroundHogComponent
    {
        private static readonly Predicate<Curve> _removeNullAndInvalidDelegate = RemoveNullAndInvalid;
        private static readonly Predicate<Point3d> _removeInvalidDelegate = RemoveInvalid;
        private static readonly Predicate<double> _isNegative = IsNegative;

        public GroundhogShortestPathComponent()
            : base("Shortest Path", "ShortPath", "Calculates the shortest path in a network of curves", "Groundhog",
                "Mapping")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.icon_path_shortest;

        public override Guid ComponentGuid => new Guid("{07169277-e561-4d6f-93a2-5f9d6bb0084a}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "The group of curves that form the network to traverse",
                GH_ParamAccess.list);
            pManager.AddPointParameter("Starts", "S", "The point or points that form the starting point of the path",
                GH_ParamAccess.list);
            pManager.AddPointParameter("Ends", "E", "The point or points that form the end point of the path",
                GH_ParamAccess.list);
            pManager.AddNumberParameter("Lengths", "L", string.Join(
                "A manually-specified length for each curve; useful if you want to artificially ",
                "increase or decrease their traversability. If no lengths provided, they will be ",
                "manually calculated for each of the curves."), GH_ParamAccess.list);
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Shortest Path", "P", "The curve showing the shortest connection",
                GH_ParamAccess.item);
            pManager.AddIntegerParameter("Succession", "S", "The indices of the curves that form the shortest path",
                GH_ParamAccess.tree);
            pManager.AddBooleanParameter("Direction", "D",
                "True if the curve in succession is walked from start to end, false otherwise", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Length", "L",
                "The total length, as an aggregation of the input lengths measured along the path",
                GH_ParamAccess.list);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            var CURVES = new List<Curve>();
            var STARTS = new List<Point3d>();
            var ENDS = new List<Point3d>();
            var LENGTHS = new List<double>();

            // Access and extract data from the input parameters individually
            if (!DA.GetDataList(0, CURVES)) return;
            if (!DA.GetDataList(1, STARTS)) return;
            if (!DA.GetDataList(2, ENDS)) return;
            DA.GetDataList(3, LENGTHS);

            // Input validation
            var negativeIndex = LENGTHS.FindIndex(_isNegative);
            if (negativeIndex != -1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                    string.Format("Distances cannot be negative. At least one negative value found at index {0}.",
                        negativeIndex));
                return;
            }

            CURVES.RemoveAll(_removeNullAndInvalidDelegate);
            if (CURVES.Count < 1) return;

            STARTS.RemoveAll(_removeInvalidDelegate);
            ENDS.RemoveAll(_removeInvalidDelegate);
            if (STARTS.Count != ENDS.Count)
            {
                if (ENDS.Count == 1 && STARTS.Count > 1)
                {
                    // Assume multiple starts going to single end; populate ends to match
                    for (var i = 1; i < STARTS.Count; i++) ENDS.Add(ENDS[0]);
                }
                else if (STARTS.Count == 1 && ENDS.Count > 1)
                {
                    // Assume single start going to multiple ends; populate starts to match
                    for (var i = 1; i < ENDS.Count; i++) STARTS.Add(STARTS[0]);
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                        "The quantity of start points does not match the quantity of end points");
                    return;
                }
            }

            if (LENGTHS.Count > 0 && LENGTHS.Count != CURVES.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                    "If lengths are provided they must match the number of curves");
                return;
            }

            // Construct topology
            var top = new CurvesTopology(CURVES, DocumentTolerance());
            //CurvesTopologyPreview.Mark(top, Color.BurlyWood, Color.Bisque);
            PathMethod pathSearch;

            if (LENGTHS.Count == 0)
            {
                IList<double> distances = top.MeasureAllEdgeLengths();
                pathSearch = new AStar(top, distances);
            }
            else if (LENGTHS.Count == 1)
            {
                pathSearch = new Dijkstra(top, LENGTHS[0]);
            }
            else
            {
                IList<double> interfLengths = LENGTHS;

                if (interfLengths.Count < top.EdgeLength)
                    interfLengths = new ListByPattern<double>(interfLengths, top.EdgeLength);

                var isAlwaysShorterOrEqual = true;
                for (var i = 0; i < top.EdgeLength; i++)
                    if (top.LinearDistanceAt(i) > interfLengths[i])
                    {
                        isAlwaysShorterOrEqual = false;
                        break;
                    }

                if (isAlwaysShorterOrEqual)
                    pathSearch = new AStar(top, interfLengths);
                else
                    pathSearch = new Dijkstra(top, interfLengths);
            }

            var resultCurves = new List<Curve>();
            var resultLinks = new GH_Structure<GH_Integer>();
            var resultDirs = new GH_Structure<GH_Boolean>();
            var resultLengths = new List<double>();

            for (var i = 0; i < STARTS.Count; i++)
            {
                var fromIndex = top.GetClosestNode(STARTS[i]);
                var toIndex = top.GetClosestNode(ENDS[i]);

                if (fromIndex == toIndex)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                        "The start and end positions are equal; perhaps because they are not close enough to one of the curves in the network.");
                    resultCurves.Add(null);
                    continue;
                }

                var current = pathSearch.Cross(fromIndex, toIndex, out var nodes, out var edges, out var dir,
                    out var tot);

                if (current == null)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                        string.Format("No walk found for start point at position {0}. Are end points isolated?",
                            i.ToString()));
                }
                else
                {
                    var pathLinks = DA.ParameterTargetPath(1).AppendElement(i);

                    resultLinks.AppendRange(GhWrapTypeArray<int, GH_Integer>(edges), pathLinks);
                    resultDirs.AppendRange(GhWrapTypeArray<bool, GH_Boolean>(dir), pathLinks);
                    resultLengths.Add(tot);
                }

                resultCurves.Add(current);
            }

            DA.SetDataList(0, resultCurves);
            DA.SetDataTree(1, resultLinks);
            DA.SetDataTree(2, resultDirs);
            DA.SetDataList(3, resultLengths);
        }

        private static bool RemoveNullAndInvalid(Curve obj)
        {
            return obj == null || !obj.IsValid;
        }

        private static bool RemoveInvalid(Point3d obj)
        {
            return !obj.IsValid;
        }

        private static bool IsNegative(double number)
        {
            return number < 0;
        }

        private TGh[] GhWrapTypeArray<T, TGh>(T[] input)
            where TGh : GH_Goo<T>, new()
        {
            if (input == null)
                return null;

            var newArray = new TGh[input.Length];
            for (var i = 0; i < input.Length; i++)
            {
                var gh = new TGh
                {
                    Value = input[i]
                };
                newArray[i] = gh;
            }

            return newArray;
        }
    }
}