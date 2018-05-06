using System;
using System.Collections.Generic;
using System.Drawing;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace ShortestWalk.Geometry
{
    /// <summary>
    /// Constains an immutable instance of a non-oriented curve network
    /// </summary>
    public class CurvesTopology
    {
        readonly double _tolerance;
        readonly Curve[] _curves;

        readonly NodeAddress[] _vertices;
        readonly EdgeAddress[] _edges;

        readonly Point3d[] _vPositions;
        readonly int[] _vEdges;
        readonly bool[] _vEdgesRev;

        /// <summary>
        /// Constructs a single topology
        /// </summary>
        /// <param name="curves">The array or list of curves that will be shallow copied. Modifying curves invalidates this class.</param>
        public CurvesTopology(IList<Curve> curves)
            : this(curves, 0)
        { }

        /// <summary>
        /// Constructs a single topology with tolerance
        /// </summary>
        /// <param name="curves">The array or list of curves that will be shallow copied. Modifying curves invalidates this class.</param>
        /// <param name="tolerance">A positive tolerance value, measured in document units</param>
        public CurvesTopology(IList<Curve> curves, double tolerance)
        {
            if (curves == null)
                throw new ArgumentNullException("curves");
            if (tolerance < 0)
                throw new ArgumentOutOfRangeException("tolerance", "tolerance cannot be negative");
            if (curves.Count < 1)
                throw new ArgumentException("there are 0 curves in the topology", "curves");
            if (curves.Contains(null))
                throw new ArgumentException("curves contains a null reference", "curves");

            _tolerance = tolerance;
            _curves = new Curve[curves.Count];
            curves.CopyTo(_curves, 0);

            VertexOnCurve[] vls = CopyVertexEndings(_curves);

            Point3d[] tmpUnweldedCopies = new Point3d[vls.Length];
            for (int i = 0; i < vls.Length; i++)
                tmpUnweldedCopies[i] = vls[i].Locate(_curves);

            var sorter = new VertexEndsPositionalScale(tmpUnweldedCopies);
            Array.Sort<VertexOnCurve>(vls, sorter);

            int[] verticesToWeldedVertices;
            IList<int> weldedVertexBounds;
            if (tolerance == 0)
            {
                weldedVertexBounds = RepetitionCounts(vls, _curves, out verticesToWeldedVertices);

                _vPositions = new Point3d[weldedVertexBounds.Count];
                SetupVerticesPositions(vls, weldedVertexBounds);
            }
            else
            {
                for (int i = 0; i < vls.Length; i++)
                    tmpUnweldedCopies[i] = vls[i].Locate(_curves);
                weldedVertexBounds = RepetitionCounts(tmpUnweldedCopies, out verticesToWeldedVertices, _tolerance);

                _vPositions = new Point3d[weldedVertexBounds.Count];
                SetupVerticesPositionsThatNeedToBeMapped(vls, verticesToWeldedVertices);
            }

            _edges = new EdgeAddress[_curves.Length];
            SetupVerticesEndings(vls, verticesToWeldedVertices);

            // we might add a method to remove loose lines
            // we might add a method to remove completely identical vEdges

            _vertices = new NodeAddress[weldedVertexBounds.Count];
            SetupEdgesToVerticesCounts(weldedVertexBounds);

            _vEdges = new int[_edges.Length * 2];
            _vEdgesRev = new bool[_edges.Length * 2];

            SetupVerticesToEdges();
        }

        #region Setup constructor methods

        private VertexOnCurve[] CopyVertexEndings(Curve[] curves)
        {
            VertexOnCurve[] vls = new VertexOnCurve[_curves.Length * 2];
            int c = 0;
            for (int i = 0; i < _curves.Length; i++)
            {
                vls[c] = new VertexOnCurve(c);
                c++;
                vls[c] = new VertexOnCurve(c);
                c++;
            }
            return vls;
        }

        private void SetupVerticesPositions(VertexOnCurve[] vls, IList<int> weldedVertexBounds)
        {
            int interim = 0;
            for (int i = 0; i < weldedVertexBounds.Count; i++)
            {
                var v = vls[interim];
                _vPositions[i] = v.Locate(_curves);

                interim += weldedVertexBounds[i];
            }
        }

        private void SetupVerticesPositionsThatNeedToBeMapped(VertexOnCurve[] vls, int[] map)
        {
            //This might be improved to avarage all different locations that are pointed to by the same node.
            //Now the last one wins.
            for (int i = 0; i < map.Length; i++)
            {
                var v = vls[i];
                _vPositions[map[i]] = v.Locate(_curves);
            }
        }

        private void SetupVerticesEndings(VertexOnCurve[] vls, int[] verticesToWeldedVertices)
        {
            for (int i = 0; i < vls.Length; i++)
            {
                VertexOnCurve v = vls[i];
                _edges[v.LinePosition].SetStartOrEnd(v.IsStart, verticesToWeldedVertices[i]);
            }
        }

        private void SetupEdgesToVerticesCounts(IList<int> weldedVertexBounds)
        {
            int vInterimCount = 0;
            for (int i = 0; i < weldedVertexBounds.Count; i++)
            {
                _vertices[i].EdgeStart = vInterimCount;
                vInterimCount += weldedVertexBounds[i];

#if DEBUG
                // This backreference is only needed for debugging purposes - to ease toString representation.
                // It increases the workset
                _vertices[i].Topology = this;
#endif
            }
        }

        private void SetupVerticesToEdges()
        {
            for (int i = 0; i < _edges.Length; i++)
            {
                int aLoc = _edges[i].A;
                int pos = _vertices[aLoc].EdgeCount + _vertices[aLoc].EdgeStart;
                _vEdges[pos] = i;
                _vEdgesRev[pos] = true;
                _vertices[aLoc].EdgeCount++;

                aLoc = _edges[i].B;
                pos = _vertices[aLoc].EdgeCount + _vertices[aLoc].EdgeStart;
                _vEdges[pos] = i;
                _vertices[aLoc].EdgeCount++;
            }
        }

        private static int CountSubsequentDifferent(VertexOnCurve[] vls, Curve[] lines)
        {
            int count = 0;

            if (vls.Length > 0)
            {
                Point3d value = vls[0].Locate(lines);
                for (int i = 1; i < vls.Length; i++)
                {
                    Point3d current = vls[i].Locate(lines);
                    if (value != current)
                    {
                        value = current;
                        count++;
                    }
                }
            }
            return count;
        }

        // If you feel like optimizing and improving the whole topology algorithm, start here
        private static IList<int> RepetitionCounts(Point3d[] pts, out int[] map, double tolerance)
        {
            double sqTolerance = tolerance * tolerance;
            int count = pts.Length;

            map = new int[count];
            for (int i = 0; i < count; i++)
                map[i] = -1;

            List<int> repetitionCounts = new List<int>(count / 4);

            int interim = 0;
            for (int i = 0; i < count; i++)
            {
                if (map[i] == -1)
                {
                    int cCount = 1;
                    Point3d pi = pts[i];

                    for (int j = i + 1; j < count; j++)
                    {
                        var tol = ArePointsInTolerance(pi, pts[j], tolerance, sqTolerance);
                        if (tol == ToleranceState.InTolerance)
                        {
                            map[j] = interim;
                            cCount++;
                        }
                        if (tol == ToleranceState.OutOfToleranceAndStop)
                        {
                            break;
                        }
                    }
                    repetitionCounts.Add(cCount);
                    map[i] = interim++;
                }
            }

            return repetitionCounts;
        }

        private enum ToleranceState
        {
            InTolerance = 1,
            OutOfToleranceAndContinue = 0,
            OutOfToleranceAndStop = 2
        }

        private static ToleranceState ArePointsInTolerance(Point3d p0, Point3d p1, double tolerance, double sqTolerance)
        {
            double dx = p1.X - p0.X;
            if (dx > tolerance)
            {
                return ToleranceState.OutOfToleranceAndStop; //points are sorted by X
            }

            double dy = p1.Y - p0.Y;
            if (Math.Abs(dy) > tolerance)
                return ToleranceState.OutOfToleranceAndContinue;

            double dz = p1.Z - p0.Z;

            return dx * dx + dy * dy + dz * dz <= sqTolerance ?
                ToleranceState.InTolerance : ToleranceState.OutOfToleranceAndContinue;
        }

        private static int[] RepetitionCounts(VertexOnCurve[] vls, Curve[] crvs, out int[] map)
        {
            int[] repetitionsCounts = new int[CountSubsequentDifferent(vls, crvs) + 1];
            for (int i = 0; i < repetitionsCounts.Length; i++)
                repetitionsCounts[i] = 1;

            map = new int[vls.Length];

            if (repetitionsCounts.Length > 2)
            {
                int interim = 0;
                Point3d value = vls[0].Locate(crvs);
                for (int i = 1; i < vls.Length; i++)
                {
                    Point3d current = vls[i].Locate(crvs);
                    if (value == current)
                        repetitionsCounts[interim]++;
                    else
                    {
                        value = current;
                        interim++;
                    }
                    map[i] = interim;
                }
            }
            return repetitionsCounts;
        }

        #endregion

        internal int GetVertexToEdgeIndexFromArrayAt(int i)
        {
            return _vEdges[i];
        }

        internal bool GetIsVertexEdgeRevOrientedFromArray(int i)
        {
            return _vEdgesRev[i];
        }

        /// <summary>
        /// Retrieves a topological edge
        /// </summary>
        /// <param name="i">The ordered index. Edges have the exact same order and indices as the input curves, that you can access through CurveAt(i)</param>
        /// <returns>The topological edge</returns>
        public EdgeAddress EdgeAt(int i)
        {
            return _edges[i];
        }

        /// <summary>
        /// Retrieves an input curve at an edge index
        /// </summary>
        /// <param name="i">The ordered index. Curves have the exact same order and indices as edges: you can access the edges through EdgeAt(i)</param>
        /// <returns>The curve</returns>
        public Curve CurveAt(int i)
        {
            return _curves[i];
        }

        /// <summary>
        /// Retrieves a node (the idea of a location where curves connect)
        /// </summary>
        /// <param name="i">The ordered vertex index. You can access the location with the same index through VertexAt()</param>
        /// <returns></returns>
        public NodeAddress NodeAt(int i)
        {
            return _vertices[i];
        }

        /// <summary>
        /// Retrieves a location of a vertex within tolerance at the specified node index.
        /// </summary>
        /// <param name="i">The position where a Node exists (might be one of the points within the chosen tolerance).
        /// It has the exact same ordering as NodeAt()</param>
        /// <returns></returns>
        public Point3d VertexAt(int i)
        {
            return _vPositions[i];
        }

        /// <summary>
        /// The total amount of vertices
        /// </summary>
        public int VertexLength
        {
            get
            {
                return _vertices.Length;
            }
        }

        /// <summary>
        /// The total amount of edges
        /// </summary>
        public int EdgeLength
        {
            get
            {
                return _edges.Length;
            }
        }

        /// <summary>
        /// Iterate through vertices and find the one index within tolerance
        /// </summary>
        /// <param name="position">The cartesian location of the wanted point</param>
        /// <returns>The index of the vertex, or -1 if no suitable vertex was found</returns>
        public int GetVertexIndexOf(Point3d position)
        {
            double sqT = _tolerance * _tolerance;
            for (int i = 0; i < VertexLength; i++)
            {
                if ((VertexAt(i) - position).SquareLength <= sqT)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Iterate through vertices and find the closest index, no matter of the distance
        /// </summary>
        /// <param name="position">The cartesian location of the wanted point</param>
        /// <returns>The index of the vertex</returns>
        public int GetClosestNode(Point3d position)
        {
            int val = 0;
            double dist = (VertexAt(0) - position).SquareLength;

            for (int i = 1; i < VertexLength; i++)
            {
                double currDist = (VertexAt(i) - position).SquareLength;
                if (currDist < dist)
                {
                    dist = currDist;
                    val = i;
                }
            }
            return val;
        }

        /// <summary>
        /// Get the physical length of all input curves
        /// </summary>
        /// <returns>The ordered physical length of all input curves</returns>
        public double[] MeasureAllEdgeLengths()
        {
            double[] distances = new double[EdgeLength];
            for (int i = 0; i < EdgeLength; i++)
                distances[i] = CurveAt(i).GetLength();
            return distances;
        }

        /// <summary>
        /// Get the physical length of all input curves
        /// </summary>
        /// <returns>The ordered physical length of all input curves</returns>
        public double[] MeasureAllEdgeLinearDistances()
        {
            double[] distances = new double[EdgeLength];
            for (int i = 0; i < EdgeLength; i++)
            {
                var edge = EdgeAt(i);
                distances[i] = LinearDistanceAt(edge);
            }
            return distances;
        }

        /// <summary>
        /// Returns the linear distance of an edge
        /// </summary>
        /// <param name="edge">The edgeaddress</param>
        /// <returns>The length</returns>
        public double LinearDistanceAt(EdgeAddress edge)
        {
            return (VertexAt(edge.A) - VertexAt(edge.B)).Length;
        }

        /// <summary>
        /// Returns the linear distance of an edge
        /// </summary>
        /// <param name="edgeIndex">The edge index</param>
        /// <returns>The length</returns>
        public double LinearDistanceAt(int edgeIndex)
        {
            return LinearDistanceAt(EdgeAt(edgeIndex));
        }

        private struct VertexOnCurve
        {
            //public readonly int LinePosition;
            //public readonly bool IsStart;
            readonly int _vertexPosition;

            public VertexOnCurve(int vertexPosition)
            {
                _vertexPosition = vertexPosition;
            }

            public VertexOnCurve(int curvePosition, bool start)
            {
                _vertexPosition = (curvePosition << 1) | (start ? 0 : 1);
            }

            public int LinePosition
            {
                get
                {
                    return _vertexPosition >> 1;
                }
            }

            public bool IsStart
            {
                get
                {
                    return (_vertexPosition & 1) == 0;
                }
            }

            public int ConsecutiveVertex
            {
                get
                {
                    return _vertexPosition;
                }
            }

            public override string ToString()
            {
                return string.Format("curves[{0}].{1}", LinePosition.ToString(), IsStart ? "Start" : "End");
            }

            public Point3d Locate(Curve[] input)
            {
                if (IsStart)
                    return input[LinePosition].PointAtStart;
                else
                    return input[LinePosition].PointAtEnd;
            }
        }

        private sealed class VertexEndsPositionalScale : IComparer<VertexOnCurve>
        {
            private readonly Point3d[] _endings;

            public VertexEndsPositionalScale(Point3d[] endings)
            {
                _endings = endings;
            }

            public int Compare(VertexOnCurve left, VertexOnCurve right)
            {
                Point3d pLeft = _endings[left.ConsecutiveVertex];
                Point3d pRight = _endings[right.ConsecutiveVertex];

                if (pLeft.X < pRight.X)
                    return -1;
                if (pLeft.X > pRight.X)
                    return 1;
                if (pLeft.Y < pRight.Y)
                    return -1;
                if (pLeft.Y > pRight.Y)
                    return 1;
                if (pLeft.Z < pRight.Z)
                    return -1;
                if (pLeft.Z > pRight.Z)
                    return 1;
                return 0;
            }
        }
    }

    /// <summary>
    /// An helper class to quickly show a topology
    /// </summary>
    public static class CurvesTopologyPreview
    {
        /// <summary>
        /// Marks all edges and nodes of this graph and returns the result as an array of Guids
        /// </summary>
        /// <returns>The resulting array of Guids</returns>
        public static Guid[] Mark(CurvesTopology top, Color verticesColor, Color edgesColor)
        {
            Guid[] dots = new Guid[top.VertexLength + top.EdgeLength];
            ObjectAttributes oa = RhinoDoc.ActiveDoc.CreateDefaultAttributes();
            oa.ColorSource = ObjectColorSource.ColorFromObject;

            //Graph vertices color
            oa.ObjectColor = verticesColor;
            for (int i = 0; i < top.VertexLength; i++)
            {
                dots[i] = RhinoDoc.ActiveDoc.Objects.AddTextDot(i.ToString(), top.VertexAt(i), oa);
                var sette = top.VertexAt(i);
            }

            //Graph edges
            oa.ObjectColor = edgesColor;
            for (int i = 0; i < top.EdgeLength; i++)
            {
                var p = top.CurveAt(i).PointAtNormalizedLength(0.5);
                dots[i + top.VertexLength] = RhinoDoc.ActiveDoc.Objects.AddTextDot(i.ToString(), p, oa);
            }

            RhinoDoc.ActiveDoc.Views.Redraw();
            return dots;
        }

        /// <summary>
        /// Deletes an array of Guids
        /// </summary>
        /// <param name="dots">The Guid array to delete</param>
        public static void Unmark(Guid[] dots)
        {
            if (dots != null)
            {
                for (int i = 0; i < dots.Length; i++)
                    RhinoDoc.ActiveDoc.Objects.Delete(dots[i], true);
            }
        }
    }
}
