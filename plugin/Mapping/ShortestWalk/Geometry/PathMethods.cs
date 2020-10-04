namespace ShortestWalk.Geometry
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Rhino.Geometry;

    /// <summary>
    ///     Defines a base class for methods to search paths in networks.
    /// </summary>
    public abstract class PathMethods
    {
        protected readonly IList<double> m_dist;
        protected readonly CurvesTopology m_top;

        /// <summary>
        ///     Set up the search.
        /// </summary>
        /// <param name="top">An input topology.</param>
        /// <param name="dist">
        ///     A series of distances. These cannot be less than the physical distance between starts and ends, but
        ///     might be suitably longer.
        /// </param>
        public PathMethods(CurvesTopology top, IList<double> dist)
        {
            if (top == null)
                throw new ArgumentNullException("top");
            m_top = top;

            if (dist == null)
                throw new ArgumentNullException("dist");
            if (dist.Count < m_top.EdgeLength)
                throw new ArgumentOutOfRangeException("dist", "There should be one distance for each edge");
            m_dist = dist;
        }

        /// <summary>
        ///     Searches the graph with the current algorithm.
        /// </summary>
        /// <param name="fromIndex">The vertex index of departure.</param>
        /// <param name="toIndex">The vertex index of arrival.</param>
        /// <returns>A Curve that shows the entire walk, or null if nodes are isolated.</returns>
        public virtual Curve Cross(int fromIndex, int toIndex)
        {
            int[] nodes;
            int[] edges;
            bool[] eDirs;
            double totLength;
            return Cross(fromIndex, toIndex, out nodes, out edges, out eDirs, out totLength);
        }

        /// <summary>
        ///     Searches the graph with the current algorithm. Retuns more information.
        /// </summary>
        /// <param name="fromIndex">The vertex index of departure.</param>
        /// <param name="toIndex">The vertex index of arrival.</param>
        /// <param name="nodes">Output parameter. The specific walked nodes, or null on error.</param>
        /// <param name="edges">Output parameter. The walked edges indices, or null on error.</param>
        /// <param name="eDirs">
        ///     Output parameter. Whether the edges were walked from front to end or in the opposite direction, or
        ///     null on error.
        /// </param>
        /// <returns>A Curve that shows the entire walk, or null if nodes are isolated.</returns>
        public abstract Curve Cross(int fromIndex, int toIndex, out int[] nodes, out int[] edges, out bool[] eDirs,
            out double totLength);

        protected Curve ReconstructPath(int[] cameFrom, int currentNode, out int[] nodes, out int[] edges,
            out bool[] edgeDir, out double totLength)
        {
            var resultNodes = new List<int>();
            for (;;)
            {
                if (currentNode == -1)
                    break;
                resultNodes.Add(currentNode);
                currentNode = cameFrom[currentNode];
            }

            resultNodes.Reverse();
            nodes = resultNodes.ToArray();

            var resultEdges = new List<int>();
            var resultEdgesRev = new List<bool>();
            currentNode = nodes[0];
            for (var i = 1; i < nodes.Length; i++)
            {
                var nxt = nodes[i];
                bool rev;
                var edgeIndex = FindEdge(currentNode, nxt, out rev);
                resultEdges.Add(edgeIndex);
                resultEdgesRev.Add(rev);
                currentNode = nxt;
            }

            edges = resultEdges.ToArray();
            edgeDir = resultEdgesRev.ToArray();

            totLength = 0;
            var pc = new PolyCurve();

            for (var i = 0; i < resultEdges.Count; i++)
            {
                var ei = resultEdges[i];
                var cv = m_top.CurveAt(ei).DuplicateCurve();
                if (!resultEdgesRev[i])
                    cv.Reverse();
                pc.Append(cv);
                totLength += m_dist[ei];
            }

            pc.RemoveNesting();

            return pc;
        }

        protected int FindEdge(int currentNode, int nxt, out bool rev)
        {
            var node = m_top.NodeAt(currentNode);
            var minDist = double.MaxValue;
            var bestEi = -1;
            rev = false;

            for (var j = 0; j < node.EdgeCount; j++)
            {
                var ei = node.EdgeIndexAt(j, m_top);
                var edge = m_top.EdgeAt(ei);
                if (edge.OtherVertex(currentNode) == nxt)
                    if (m_dist[ei] < minDist)
                    {
                        rev = node.RevAt(j, m_top);
                        bestEi = ei;
                        minDist = m_dist[ei];
                    }
            }

            if (bestEi == -1)
                throw new KeyNotFoundException("Vertex currentNode is not linked to nxt");

            return bestEi;
        }

        protected void CheckArguments(int from, int to)
        {
            if (from < 0)
                throw new ArgumentOutOfRangeException("from", "from is less than 0");
            if (to < 0)
                throw new ArgumentOutOfRangeException("to", "to is less than 0");
            if (from >= m_top.VertexLength)
                throw new ArgumentOutOfRangeException("from", "from is more than vertex length");
            if (to >= m_top.VertexLength)
                throw new ArgumentOutOfRangeException("to", "to is more than vertex length");
            if (from == to)
                throw new ArgumentException("Walking indices from and to are the same");
        }

        public static PathMethods FromMode(SearchMode sm, CurvesTopology crvTopology, double[] distances)
        {
            PathMethods pathSearch;
            switch (sm)
            {
                case SearchMode.CurveLength:
                    pathSearch = new AStar(crvTopology, distances);
                    break;
                case SearchMode.LinearDistance:
                    pathSearch = new AStar(crvTopology, distances);
                    break;
                case SearchMode.Links:
                    if (distances != null)
                        throw new ArgumentException(
                            "If you use Links mode, then distances must be null as it will be ignored",
                            "distances");
                    pathSearch = new Dijkstra(crvTopology);
                    break;
                default:
                    throw new ApplicationException("No behviour is defined for this enum value");
            }

            return pathSearch;
        }

        protected static int FindMinimumScoreAmongOpen(IList<int> open, double[] fScore)
        {
            var n = open[0];
            var cc = fScore[n];
            for (var i = 1; i < open.Count; i++)
            {
                var possibleBetterIndex = open[i];
                var current = fScore[possibleBetterIndex];
                if (current < cc)
                {
                    n = possibleBetterIndex;
                    cc = current;
                }
            }

            return n;
        }
    }

    public class AStar : PathMethods
    {
        /// <summary>
        ///     The A* search algorithm.
        ///     See http://en.wikipedia.org/wiki/A*_search_algorithm for description.
        /// </summary>
        /// <param name="top">An input topology.</param>
        /// <param name="dist">
        ///     A series of distances. These cannot be less than the physical distance between starts and ends, but
        ///     might be suitably longer.
        /// </param>
        public AStar(CurvesTopology top, IList<double> dist)
            : base(top, dist)
        {
        }

        public override Curve Cross(int fromIndex, int toIndex, out int[] nodes, out int[] edges, out bool[] eDirs,
            out double totLength)
        {
            CheckArguments(fromIndex, toIndex);

            var closed = new Dictionary<int, byte>(m_top.EdgeLength / 4);
            var open = new SortedList<int, byte>(m_top.EdgeLength / 5);

            // We do not need the byte generic type in either collections. List<> works and
            // is smaller in memory but is much slower for large datasets. HashSet<T> might work in .Net 3.5+
            open.Add(fromIndex, default(byte));

            var gScore = new double[m_top.VertexLength];
            var hScore = new double[m_top.VertexLength];
            var fScore = new double[m_top.VertexLength];
            var cameFrom = new int[m_top.VertexLength];
            for (var i = 0; i < cameFrom.Length; i++)
                cameFrom[i] = -1;

            hScore[fromIndex] = HeuristicEstimateDistance(m_top, fromIndex, toIndex);
            fScore[fromIndex] = hScore[fromIndex];

            while (open.Count > 0)
            {
                var n = FindMinimumScoreAmongOpen(open.Keys, fScore);
                if (n == toIndex)
                    return ReconstructPath(
                        cameFrom, toIndex, out nodes, out edges, out eDirs, out totLength); // Found the path

                open.Remove(n);
                closed.Add(n, default(byte));

                var node = m_top.NodeAt(n);
                for (var i = 0; i < node.EdgeCount; i++)
                {
                    var ei = node.EdgeIndexAt(i, m_top);
                    var y = m_top.EdgeAt(ei).OtherVertex(n);

                    if (closed.ContainsKey(y))
                        continue;
                    var tentativeGscore = gScore[n] + m_dist[ei];

                    bool tentativeIsBetter;
                    if (!open.ContainsKey(y))
                    {
                        open.Add(y, default(byte));
                        tentativeIsBetter = true;
                    }
                    else if (tentativeGscore < gScore[y])
                    {
                        tentativeIsBetter = true;
                    }
                    else
                    {
                        tentativeIsBetter = false;
                    }

                    if (tentativeIsBetter)
                    {
                        cameFrom[y] = n;

                        gScore[y] = tentativeGscore;
                        hScore[y] = HeuristicEstimateDistance(m_top, y, toIndex);
                        fScore[y] = gScore[y] + hScore[y];
                    }
                }
            }

            nodes = edges = null;
            eDirs = null;
            totLength = double.NaN;
            return null; // no path found. Isolation
        }

        protected static double HeuristicEstimateDistance(CurvesTopology top, int to, int y)
        {
            return top.VertexAt(y).DistanceTo(top.VertexAt(to));
        }
    }

    /// <summary>
    ///     The Dijkstra algorithm.
    ///     See http://en.wikipedia.org/wiki/Dijkstra's_algorithm for description.
    /// </summary>
    public class Dijkstra : PathMethods
    {
        /// <summary>
        ///     The Dijkstra algorithm.
        ///     Each edge is given length 1. The graph is evaluated by link counts.
        /// </summary>
        /// <param name="top">An input topology.</param>
        public Dijkstra(CurvesTopology top)
            : this(top, new AlwaysFixed(1, top.EdgeLength))
        {
        }

        /// <summary>
        ///     The Dijkstra algorithm.
        ///     Each edge is given length [value]. The graph is evaluated by link counts.
        /// </summary>
        /// <param name="top">An input topology.</param>
        public Dijkstra(CurvesTopology top, double value)
            : this(top, new AlwaysFixed(value, top.EdgeLength))
        {
        }

        /// <summary>
        ///     The Dijkstra algorithm.
        ///     Each edge is given the length as set in "dist" parameter.
        /// </summary>
        /// <param name="top">An input topology.</param>
        /// <param name="dist">A series of distances. These cannot be less than 0.</param>
        public Dijkstra(CurvesTopology top, IList<double> dist)
            : base(top, dist)
        {
        }

        public override Curve Cross(
            int fromIndex, int toIndex, out int[] nodes, out int[] edges, out bool[] eDirs, out double totLength)
        {
            CheckArguments(fromIndex, toIndex);

            var countDist = new double[m_top.VertexLength];
            var cameFrom = new int[m_top.VertexLength];
            for (var i = 0; i < cameFrom.Length; i++)
            {
                countDist[i] = double.PositiveInfinity;
                cameFrom[i] = -1;
            }

            var open = new List<int>(cameFrom.Length);
            for (var i = 0; i < cameFrom.Length; i++)
                open.Add(i);

            countDist[fromIndex] = 0;

            while (open.Count > 0)
            {
                var u = FindMinimumScoreAmongOpen(open, countDist);

                if (double.IsPositiveInfinity(countDist[u]))
                {
                    nodes = edges = null;
                    eDirs = null;
                    totLength = double.NaN;
                    return null; // no path found. Island
                }

                if (u == toIndex)
                    return ReconstructPath(cameFrom, toIndex, out nodes, out edges, out eDirs,
                        out totLength); // Found the path

                open.Remove(u);

                var node = m_top.NodeAt(u);
                for (var i = 0; i < node.EdgeCount; i++)
                {
                    var edgeIndex = node.EdgeIndexAt(i, m_top);
                    var v = m_top.EdgeAt(edgeIndex).OtherVertex(u);

                    if (!open.Contains(v))
                        continue;

                    var tentativeScore = countDist[u] + m_dist[edgeIndex];

                    if (tentativeScore < countDist[v])
                    {
                        countDist[v] = tentativeScore;
                        cameFrom[v] = u;
                    }
                }
            }

            throw new ApplicationException("Error in topoogy.");
        }

        private class AlwaysFixed : IList<double>
        {
            private readonly double _value;

            public AlwaysFixed(int count)
                : this(1, count)
            {
            }

            public AlwaysFixed(double value, int count)
            {
                Count = count;
                _value = value;
            }

            public int IndexOf(double item)
            {
                return item == _value && Count > 0 ? 0 : -1;
            }

            public void Insert(int index, double item)
            {
                throw new NotImplementedException();
            }

            void IList<double>.RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            public double this[int index]
            {
                get => _value;
                set => throw new NotSupportedException();
            }

            public IEnumerator<double> GetEnumerator()
            {
                for (var i = 0; i < Count; i++)
                    yield return _value;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            void ICollection<double>.Add(double item)
            {
                throw new NotSupportedException();
            }

            void ICollection<double>.Clear()
            {
                throw new NotSupportedException();
            }

            bool ICollection<double>.Contains(double item)
            {
                return Count > 0 && item == _value;
            }

            void ICollection<double>.CopyTo(double[] array, int arrayIndex)
            {
                throw new NotSupportedException();
            }

            public int Count { get; }

            bool ICollection<double>.IsReadOnly => true;

            bool ICollection<double>.Remove(double item)
            {
                throw new NotSupportedException();
            }
        }
    }

    public enum SearchMode
    {
        CurveLength = 1,
        // AdjustedCurveLength = 2,
        LinearDistance = 3,
        Links = 4,
    }
}