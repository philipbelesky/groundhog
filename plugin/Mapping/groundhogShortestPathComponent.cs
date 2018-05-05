using System;
using System.Collections.Generic;
using System.Drawing;
using groundhog.Properties;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using ShortestWalk.Gh;
using ShortestWalk.Geometry;

// Component source lightly modified from Giulio Piacentino's repo, see License/Attribution in /ShortestWalk/

namespace groundhog
{
    public class groundhogShortestPathComponent : GroundHog_Component
    {
        public groundhogShortestPathComponent()
            : base("Shortest Path", "ShortPath", "Calculates the shortest path in a network of curves",
                   "Groundhog", "Mapping")
        { 
        }

        protected override Bitmap Icon => Resources.icon_surface_slope;

        public override Guid ComponentGuid => new Guid("{07169277-e561-4d6f-93a2-5f9d6bb0084a}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "The group of curves that form the network to traverse", GH_ParamAccess.list);
            pManager.AddNumberParameter("Lengths", "L", "A length for each curve. If the number of lengths is less than the one of curves, length values are repeated in pattern.\nIf there are no lengths provided, they will be calculated for each of the curves.", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddLineParameter("Desired Path", "P", "The lines from the start to the end of the path", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Shortest Path", "P", "The curve showing the shortest connection", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Succession", "S", "The indices of the curves that form the shortest path", GH_ParamAccess.tree);
            pManager.AddBooleanParameter("Direction", "D", "True if the curve in succession is walked from start to end, false otherwise", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Length", "L", "The total length, as an aggregation of the input lengths measured along the path", GH_ParamAccess.list);
        }

        static Predicate<Curve> _removeNullAndInvalidDelegate = RemoveNullAndInvalid;
        static Predicate<Line> _removeInvalidDelegate = RemoveInvalid;
        static Predicate<double> _isNegative = IsNegative;

        private static bool RemoveNullAndInvalid(Curve obj)
        {
            return obj == null || !obj.IsValid;
        }

        private static bool RemoveInvalid(Line obj)
        {
            return !obj.IsValid;
        }

        private static bool IsNegative(double number)
        {
            return number < 0;
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            var curves = new List<Curve>();
            var lengths = new List<double>();
            var lines = new List<Line>();

            if (DA.GetDataList(0, curves) && DA.GetDataList(2, lines))
            {
                DA.GetDataList(1, lengths);

                int negativeIndex = lengths.FindIndex(_isNegative);
                if (negativeIndex != -1)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                        string.Format("Distances cannot be negative. At least one negative value encounter at index {0}.",
                        negativeIndex));
                    return;
                }

                curves.RemoveAll(_removeNullAndInvalidDelegate);
                lines.RemoveAll(_removeInvalidDelegate);

                if (curves.Count < 1)
                {
                    return;
                }
                
                CurvesTopology top = new CurvesTopology(curves, GH_Component.DocumentTolerance());
                //CurvesTopologyPreview.Mark(top, Color.BurlyWood, Color.Bisque);

                PathMethod pathSearch;
                if (lengths.Count == 0)
                {
                    IList<double> distances = top.MeasureAllEdgeLengths();
                    pathSearch = new AStar(top, distances);
                }
                else if (lengths.Count == 1)
                {
                    pathSearch = new Dijkstra(top, lengths[0]);
                }
                else
                {
                    IList<double> interfLengths = lengths;

                    if (interfLengths.Count < top.EdgeLength)
                        interfLengths = new ListByPattern<double>(interfLengths, top.EdgeLength);

                    bool isAlwaysShorterOrEqual = true;
                    for(int i=0; i<top.EdgeLength; i++)
                    {
                        if (top.LinearDistanceAt(i) > interfLengths[i])
                        {
                            isAlwaysShorterOrEqual = false;
                            break;
                        }
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

                for (int i = 0; i < lines.Count; i++)
                {
                    var line = lines[i];

                    int fromIndex = top.GetClosestNode(line.From);
                    int toIndex = top.GetClosestNode(line.To);

                    if (fromIndex == toIndex)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The start and end positions are equal");
                        resultCurves.Add(null);
                        continue;
                    }

                    int[] nodes, edges; bool[] dir; double tot;
                    var current = pathSearch.Cross(fromIndex, toIndex, out nodes, out edges, out dir, out tot);

                    if (current == null)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                            string.Format("No walk found for line at position {0}. Are end points isolated?", i.ToString()));
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
        }

        private TGh[] GhWrapTypeArray<T, TGh>(T[] input)
            where TGh : GH_Goo<T>, new()
        {
            if (input == null)
                return null;

            var newArray = new TGh[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                var gh = new TGh();
                gh.Value = input[i];
                newArray[i] = gh;
            }
            return newArray;
        }


    }
}
