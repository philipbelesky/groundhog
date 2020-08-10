namespace ShortestWalk.Geometry
{
    using System;

    /// <summary>
    ///     An edge, or the connection between two nodes or endpoints.
    ///     A and B are the indices as in CurveTopology NodeAt() or VertexAt().
    /// </summary>
    public struct EdgeAddress
    {
        /// <summary>
        ///     Constructs an edge from start and end.
        /// </summary>
        /// <param name="a">Start index.</param>
        /// <param name="b">End index.</param>
        public EdgeAddress(int a, int b)
        {
            A = a;
            B = b;
        }

        /// <summary>
        ///     The start index of this edge.
        /// </summary>
        public int A { get; private set; }

        /// <summary>
        ///     The end index of this edge.
        /// </summary>
        public int B { get; private set; }

        /// <summary>
        ///     Returns the A index (start, true) or the B index (end, false) of this vertex.
        /// </summary>
        /// <param name="start">A if start is true, B if start is false.</param>
        /// <returns>The Node index as in the edge.</returns>
        public int this[bool start] => start ? A : B;

        internal void SetStartOrEnd(bool start, int value)
        {
            if (start)
                A = value;
            else
                B = value;
        }

        /// <summary>
        ///     Discovers the other index given one end of this edge.
        /// </summary>
        /// <param name="eitherAOrB">The known index.</param>
        /// <returns>The opposite index (e.g., A if B was input).</returns>
        /// <exception cref="System.InvalidOperationException">If neither A or B were given as inputs.</exception>
        public int OtherVertex(int eitherAOrB)
        {
            if (eitherAOrB == A)
                return B;
            if (eitherAOrB == B)
                return A;
            throw new InvalidOperationException(
                string.Format("oneOfAOrB - currently {0} but only {1} and {2} present", eitherAOrB.ToString(), A.ToString(), B.ToString()));
        }

        /// <summary>
        ///     A textual representation of the edge.
        /// </summary>
        /// <returns>a string with "{A}, {B}".</returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}", A.ToString(), B.ToString());
        }
    }
}