using System;
using System.Collections.Generic;
using System.Drawing;
using groundhog.Properties;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using Point = System.Drawing.Point;

namespace groundhog
{
    public class groundhogFieldComponent : GH_Component
    {

        public groundhogFieldComponent()
            : base("Field Mapper", "Field", "Create ", "Groundhog", "Mapping")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.icon_field;

        public override Guid ComponentGuid => new Guid("{2d268bdc-ecaa-4cf7-811a-c8111d1798d4}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Bounds", "B", "Boundary box for the resulting field", GH_ParamAccess.item);
            pManager.AddNumberParameter("Divisions", "D", "Sample points spacings for the resulting field (greatest extent in one direction)", GH_ParamAccess.item);
            pManager.AddCurveParameter("Areas", "A", "Boundary box for the resulting field", GH_ParamAccess.list);
            pManager.AddNumberParameter("Z Range", "Z", "Maximum height of the surface field (defaults to 5% of boundary width/height)", GH_ParamAccess.item, 0.0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            // Generic is its a GH_ObjectWrapper wrapper for our custom class
            pManager.AddSurfaceParameter("Field", "F", "Resulting field", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Create holder variables for input parameters
            Curve gridBounds = null;
            var gridDivisions = 24.0;
            var areas = new List<Curve>();
            var zRange = 0.0; // Default value; is later set to 5% of maximum dimension if still 0

            // Access and extract data from the input parameters individually
            if (!DA.GetData(0, ref gridBounds)) return;
            if (!DA.GetData(1, ref gridDivisions)) return;
            if (!DA.GetDataList(2, areas)) return;
            if (!DA.GetData(3, ref zRange)) return;

            var TOLERANCE = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            var FIDELITY = Convert.ToInt32(gridDivisions);
            var BOUNDARY = gridBounds;
            var ALL_DATA_REGIONS = areas;
            var INTERPOLATE = true;

            if (gridDivisions < 4)
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Numver of grid divisions must be greater than 4 in order to create a surface");

            // Validate input curves are planar
            if (!BOUNDARY.IsPlanar())
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Boundary curve is not planar");
            if (!BOUNDARY.IsClosed)
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Boundary curve is not closed");

            for (var i = 0; i < areas.Count; i = i + 1)
            {
                if (areas[i] == null)
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "An area curve is not defined (null)");
                else
                    if (!areas[i].IsPlanar())
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "A boundary curve is not planar");
                    if (!areas[i].IsClosed)
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "A boundary curve is not closed");
            }


            // Construct the bounding box for the search limits boundary; identify its extents and if its square
            var boundaryBox = BOUNDARY.GetBoundingBox(false); // False = uses estimate method
            var BOUNDARY_IS_RECT = IsBoundaryRect(BOUNDARY, TOLERANCE);

            // Construct the boundary boxes for the search targets
            var regionBoxes = new List<BoundingBox>();
            for (var i = 0; i < ALL_DATA_REGIONS.Count; i = i + 1)
                regionBoxes.Add(ALL_DATA_REGIONS[i].GetBoundingBox(false));

            // Construct the grid points
            var gridInfo = CreateGridPts(BOUNDARY, boundaryBox, BOUNDARY_IS_RECT, FIDELITY, zRange);
            // Unpack the returned values
            var gridPts = gridInfo.Item1;
            zRange = gridInfo.Item2;
            var xGridExtents = gridInfo.Item3;
            var yGridExtents = gridInfo.Item4;

            // TODO: cull the grid points here if they are outside of the bounds?
            // Don't feed those outside the box into the RTree
            // Best check by setting an out of bounds = false property or something

            // Create and search the rTree
            //Print("Creating/searching the rTree");
            var rTree = new RTree();
            for (var x = 0; x < gridPts.GetLength(0); x = x + 1)
            for (var y = 0; y < gridPts.GetLength(1); y = y + 1)
                // Only add points inside the containment area to the RTree to search
                if (gridPts[x, y].InsideBoundary)
                    rTree.Insert(gridPts[x, y].Location, gridPts[x, y].GridIndex);

            for (var i = 0; i < regionBoxes.Count; i = i + 1)
            {
                // Construct a tuple to stuff state into the callback
                var data = Tuple.Create(i, gridPts, ALL_DATA_REGIONS, xGridExtents, yGridExtents, TOLERANCE);
                rTree.Search(regionBoxes[i], RegionOverlapCallback, data);
            }

            // Set all the point's z values based on count; maybe set it to like 10% of extents?
            if (INTERPOLATE)
                for (var x = 0; x < gridPts.GetLength(0); x = x + 1)
                for (var y = 0; y < gridPts.GetLength(1); y = y + 1)
                {
                    var pt = gridPts[x, y];
                    if (pt.BaseOverlaps == 0)
                        pt.InterpolatedOverlaps = InterpolateOverlaps(x, y, pt, gridPts);
                }

            double overlapsMax = 0;
            for (var x = 0; x < gridPts.GetLength(0); x = x + 1)
            for (var y = 0; y < gridPts.GetLength(1); y = y + 1)
                if (gridPts[x, y].BaseOverlaps > overlapsMax)
                    overlapsMax = gridPts[x, y].BaseOverlaps;
            //Print("overlapsMax: {0}", overlapsMax);

            // Set Z values according to range/max
            for (var x = 0; x < gridPts.GetLength(0); x = x + 1)
            for (var y = 0; y < gridPts.GetLength(1); y = y + 1)
            {
                var pt = gridPts[x, y];
                var newZ = pt.Location.Z + pt.LargestOverlap() / overlapsMax * zRange;
                pt.Location = new Point3d(pt.Location.X, pt.Location.Y, newZ);
            }

            // Real Output
            // var pointsList = gridPts.Select(item => item.Location).ToList(); ONLY for visual studio
            // Replacement for C#:
            var pointsList = new List<Point3d>(xGridExtents * yGridExtents);
            var size1 = gridPts.GetLength(1);
            var size0 = gridPts.GetLength(0);
            for (var i = 0; i < size0; i++)
            for (var j = 0; j < size1; j++)
                pointsList.Add(gridPts[i, j].Location);

            var fieldSrf =
                NurbsSurface.CreateFromPoints(pointsList, xGridExtents, yGridExtents, 3,
                    3); // points, x/y counts, u/v degrees

            // Assign variables to output parameters
            DA.SetData(0, fieldSrf);
        }

        private void RegionOverlapCallback(object sender, RTreeEventArgs e)
        {
            var dataIndex = e.Id; // The full-grid index of the point found

            // Reconstruct variables from the data tag
            var data = e.Tag as Tuple<int, GridPt[,], List<Curve>, int, int, double>;
            var regionIndex = data.Item1;
            var gridPoints = data.Item2;
            var overlapRegion = data.Item3[regionIndex];
            var xExtents = data.Item4;
            var yExtents = data.Item5;
            var tolerance = data.Item6;

            // Using the known GridPt 2D array bounds, locate the 2D indices from the 1D dataIndex
            //Print("regionOverlapCallback (start) xExtents={0} yExtents={1}", xExtents, yExtents);
            var xIndex = dataIndex / yExtents;
            var yIndex = dataIndex % yExtents;
            //Print("\t\tPt where dataIndex={0} is at {1},{2}", dataIndex, xIndex, yIndex);

            // Increment overlaps after checking if the curve actually overlaps
            var overlappingPt = gridPoints[xIndex, yIndex];
            //Print("\t\tPt where gridIndex={0} is at {1}", overlappingPt.GridIndex, overlappingPt.Location);

            var worldXY = Plane.WorldXY;
            var isOutside = PointContainment.Outside;
            var containmentTest = overlapRegion.Contains(overlappingPt.Location, worldXY, tolerance);
            if (containmentTest != isOutside)
                overlappingPt.BaseOverlaps += 1;
            //Print("\n");
        }

        private bool IsBoundaryRect(Curve BOUNDARY, double TOLERANCE)
        {
            var isRect = false;
            if (BOUNDARY.IsPolyline())
            {
                Polyline BOUNDARY_PLINE;
                BOUNDARY.TryGetPolyline(out BOUNDARY_PLINE);
                if (BOUNDARY_PLINE != null && BOUNDARY_PLINE.SegmentCount == 4)
                {
                    var edges = BOUNDARY_PLINE.GetSegments();
                    // Is each pair of segments the same length?
                    if (Math.Abs(edges[0].Length - edges[1].Length) < TOLERANCE)
                        if (Math.Abs(edges[0].Length - edges[1].Length) < TOLERANCE)
                        {
                            // Are each of the diagonals the same length?
                            var diagonalA = edges[0].From.DistanceTo(edges[2].From);
                            var diagonalB = edges[1].From.DistanceTo(edges[3].From);
                            if (Math.Abs(diagonalA - diagonalB) < TOLERANCE)
                                isRect = true;
                        }
                }
            }
            return isRect;
        }

        private int[] Range(int start, int times, int step)
        {
            // Oh for python
            var sequence = new int[times];
            for (var i = 0; i < times; i = i + 1)
                sequence[i] = start + i * step;
            return sequence;
        }

        private double InterpolateOverlaps(int x, int y, GridPt fromPt, GridPt[,] searchPts)
        {
            var xExtents = searchPts.GetLength(0);
            var yExtents = searchPts.GetLength(1);
            double maximumProximity = Math.Max(xExtents, yExtents); // Farthest distance possible
            double maximumOverlaps = 0; // The number of overlaps for the closest neighbour
            double interpolatedOverlaps = 0; // The inerpolated overlap value

            // Here i is the offset from the base position
            for (var i = 1; i < maximumProximity; i++)
            {
                //Print("#[{0},{1} iteration {2}]_______", x, y, i);

                var travelDistance = i * 2 + 1; // The length of the rows to find
                var offset = (travelDistance - 1) / 2; // The amount to go in x/y directions from centerpts
                var totalNeighbours = travelDistance * 4 - 4;

                var neighbourIndices = new List<Point>();

                for (var u = 0; u < travelDistance; u++)
                    neighbourIndices.Add(new Point(x - offset + u, y + offset)); // Tops
                for (var u = 0; u < travelDistance; u++)
                    neighbourIndices.Add(new Point(x - offset + u, y - offset)); // Bottoms
                for (var u = 0; u < travelDistance - 2; u++)
                    neighbourIndices.Add(new Point(x - offset, y - offset + u + 1)); // Lefts
                for (var u = 0; u < travelDistance - 2; u++)
                    neighbourIndices.Add(new Point(x + offset, y - offset + u + 1)); // Rights

                //neighbourIndices.RemoveAll(pt => pt.X < 0);
                //neighbourIndices.RemoveAll(pt => pt.Y < 0);
                //neighbourIndices.RemoveAll(pt => pt.X > xExtents);
                //neighbourIndices.RemoveAll(pt => pt.Y > yExtents);


                // Find the maximum hit value
                for (var j = 0; j < neighbourIndices.Count; j++)
                {
                    var pt = neighbourIndices[j];
                    //Print("{0},{1}", pt.X, pt.Y);
                    // TODO: index out of range here; why?
                    //double searchPtOverlaps = searchPts[pt.X, pt.Y].BaseOverlaps;
                    //if (searchPtOverlaps > maximumOverlaps) {
                    //  maximumOverlaps = searchPtOverlaps;
                    //  Print("    Found a maximum overlap of {0} at position {1}", maximumOverlaps, neighbourIndices[j]);
                    //}
                }

                if (maximumOverlaps > 0)
                {
                    interpolatedOverlaps = (maximumProximity - i) / maximumProximity * maximumOverlaps;
                    //Print("    Setting max overlap to {0}", interpolatedOverlaps);
                    break; // Found the closest neighbour so we break
                }
            }
            return interpolatedOverlaps;
        }

        private Tuple<GridPt[,], double, int, int> CreateGridPts(Curve BOUNDARY, BoundingBox boundaryBox,
            bool BOUNDARY_IS_RECT, int FIDELITY,
            double zRange)
        {
            // Determine boundary dimensions
            var corners = boundaryBox.GetCorners();
            var xLength = corners[1][0] - corners[0][0];
            var yLength = corners[2][1] - corners[0][1];
            var BOUNDARYZ = corners[0][2];
            var maxLength = Math.Sqrt(Math.Pow(xLength, 2) + Math.Pow(yLength, 2));

            // Use these to determine the number of grid points
            var gridSpacing = Math.Max(xLength, yLength) / FIDELITY;
            var xGridExtents = (int) Math.Floor(xLength / gridSpacing);
            var yGridExtents = (int) Math.Floor(yLength / gridSpacing);

            var middlePt = new Point3d(
                corners[0][0] + xLength / 2,
                corners[0][1] + yLength / 2,
                BOUNDARYZ);

            // Start bottom left (as measured from center point)
            var startPt = new Point3d(
                middlePt[0] - gridSpacing * (xGridExtents / 2),
                middlePt[1] - gridSpacing * (yGridExtents / 2),
                BOUNDARYZ);

            double xSpacingBump;
            if (xGridExtents % 2 == 0)
                xSpacingBump = gridSpacing / 2;
            else
                xSpacingBump = 0;

            double ySpacingBump;
            if (yGridExtents % 2 == 0)
                ySpacingBump = gridSpacing / 2;
            else
                ySpacingBump = 0;

            if (zRange == 0)
                zRange = Math.Max(xLength, yLength) * 0.05;
            //Print("createGridPts: with a Z range of {0}", zRange);

            // Make grid points
            var gridPts = new GridPt[xGridExtents, yGridExtents];

            var i = 0;
            for (var x = 0; x < xGridExtents; x++)
            for (var y = 0; y < yGridExtents; y++)
            {
                gridPts[x, y] = new GridPt
                {
                    Location = new Point3d(
                        startPt[0] + gridSpacing * x + xSpacingBump,
                        startPt[1] + gridSpacing * y + ySpacingBump,
                        BOUNDARYZ),
                    GridIndex = i,
                    BaseOverlaps = 0,
                    InterpolatedOverlaps = 0,
                    InsideBoundary = true
                };
                i++;
            }

            //Print("createGridPts: Making {0} columns", gridPts.GetLength(0));
            //Print("createGridPts: Making {0} rows", gridPts.GetLength(1));

            // If out boundary is not rectangular cull the points outside of it
            if (!BOUNDARY_IS_RECT)
            {
                var isOutside = PointContainment.Outside;
                for (var x = 0; x < gridPts.GetLength(0); x++)
                for (var y = 0; y < gridPts.GetLength(1); y++)
                    if (BOUNDARY.Contains(gridPts[x, y].Location) == isOutside)
                        gridPts[x, y].InsideBoundary = false;
            }

            return Tuple.Create(gridPts, zRange, xGridExtents, yGridExtents);
        }

        private class GridPt
        {
            public Point3d Location { get; set; }
            public int GridIndex { get; set; }
            public bool InsideBoundary { get; set; }

            public double BaseOverlaps { get; set; } // Records number of times it overlaps with a data region
            public double InterpolatedOverlaps { get; set; } // Nearest neighbour interpolation of overlaps

            public double LargestOverlap()
            {
                return Math.Max(BaseOverlaps, InterpolatedOverlaps);
            }
        }
    }
}