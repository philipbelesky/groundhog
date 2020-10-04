namespace ShortestWalk.Geometry
{
    using System;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    ///     A node, or the location where edges are attached to each other.
    ///     A NodeAddress does not store any Node index, it merely
    ///     bookmarks the positions where to look for Node indices.
    /// </summary>
    public struct NodeAddress
    {
        private int _edgeStart;

        private int _edgeCount;

        /// <summary>
        ///     Constructs a node from start location inside a topology and count.
        /// </summary>
        /// <param name="edgeStart"></param>
        /// <param name="edgeCount"></param>
        public NodeAddress(int edgeStart, int edgeCount)
        {
            _edgeStart = edgeStart;
            _edgeCount = edgeCount;

#if DEBUG
            Topology = null;
#endif
        }

        internal int EdgeStart
        {
            get => _edgeStart;
            set
            {
                DoNotAllowNegativeIndices(value);
                _edgeStart = value;
            }
        }

        /// <summary>
        ///     The number of edges at this Node.
        /// </summary>
        public int EdgeCount
        {
            get => _edgeCount;
            internal set
            {
                DoNotAllowNegativeIndices(value);
                _edgeCount = value;
            }
        }

        [Conditional("DEBUG")]
        private void DoNotAllowNegativeIndices(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("value");
        }

        /// <summary>
        ///     Retrieves the Topology edge index of the Edge at position i at this Node.
        /// </summary>
        /// <param name="i">An index smaller than EdgeCount and larger than 0.</param>
        /// <param name="top">The reference topology.</param>
        /// <returns>The Topology edge index.</returns>
        public int EdgeIndexAt(int i, CurvesTopology top)
        {
            if (i < 0 || i >= EdgeCount)
                throw new ArgumentOutOfRangeException("i", "index must be smaller than EdgeCount and larger than 0");

            return top.GetVertexToEdgeIndexFromArrayAt(_edgeStart + i);
        }

        /// <summary>
        ///     Retrieves the Topology edge of the Edge at position i at this Node.
        /// </summary>
        /// <param name="i">An edge index smaller than EdgeCount and larger than 0.</param>
        /// <param name="top">The reference topology.</param>
        /// <returns>The Topology edge.</returns>
        public EdgeAddress EdgeAt(int i, CurvesTopology top)
        {
            if (i < 0 || i >= EdgeCount)
                throw new ArgumentOutOfRangeException("i", "index must be smaller than EdgeCount and larger than 0");

            return top.EdgeAt(EdgeIndexAt(i, top));
        }

        /// <summary>
        ///     Retrieves the direction of the Edge at position i at this Node.
        /// </summary>
        /// <param name="i">An edge index smaller than EdgeCount and larger than 0.</param>
        /// <param name="top">The reference topology.</param>
        /// <returns>A boolean indicating if the edge is oriented frontal or backwards.</returns>
        public bool RevAt(int i, CurvesTopology top)
        {
            if (i < 0 || i >= EdgeCount)
                throw new ArgumentOutOfRangeException("i", "index must be smaller than EdgeCount and larger than 0");

            return top.GetIsVertexEdgeRevOrientedFromArray(_edgeStart + i);
        }

#if DEBUG

        public CurvesTopology Topology { get; set; }

        public override string ToString()
        {
            if (Topology != null)
            {
                var sb = new StringBuilder();
                sb.Append(EdgeIndexAt(0, Topology));
                for (var i = 1; i < _edgeCount; i++)
                {
                    sb.Append(", ");
                    sb.Append(EdgeIndexAt(i, Topology));
                }

                return sb.ToString();
            }

            return "Set topology for preview";
        }
#else
        /// <summary>
        /// The string representation of this edge
        /// </summary>
        /// <returns>A string with the amount of vertices at this node</returns>
        public override string ToString()
        {
            return string.Format("Edge with {2} vertices",
                _edgeCount.ToString()
                );
        }
#endif
    }
}