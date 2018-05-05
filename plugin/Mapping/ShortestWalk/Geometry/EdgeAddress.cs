using System;

namespace ShortestWalk.Geometry
{
    /// <summary>
    /// An edge, or the connection between two nodes or endpoints.
    /// A and B are the indices as in CurveTopology NodeAt() or VertexAt()
    /// </summary>
    public struct EdgeAddress
    {
        private int _a;
        private int _b;

        /// <summary>
        /// Constructs an edge from start and end
        /// </summary>
        /// <param name="a">Start index</param>
        /// <param name="b">End index</param>
        public EdgeAddress(int a, int b)
        {
            _a = a;
            _b = b;
        }

        /// <summary>
        /// The start index of this edge
        /// </summary>
        public int A
        {
            get
            {
                return _a;
            }
        }

        /// <summary>
        /// The end index of this edge
        /// </summary>
        public int B
        {
            get
            {
                return _b;
            }
        }

        /// <summary>
        /// Returns the A index (start, true) or the B index (end, false) of this vertex
        /// </summary>
        /// <param name="start">A if start is true, B if start is false</param>
        /// <returns>The Node index as in the edge</returns>
        public int this[bool start]
        {
            get
            {
                return start ? _a : _b;
            }
        }

        internal void SetStartOrEnd(bool start, int value)
        {
            if (start)
                _a = value;
            else
                _b = value;
        }

        /// <summary>
        /// Discovers the other index given one end of this edge
        /// </summary>
        /// <param name="eitherAOrB">The known index</param>
        /// <returns>The opposite index (e.g., A if B was input)</returns>
        /// <exception cref="System.InvalidOperationException">If neither A or B were given as inputs</exception>
        public int OtherVertex(int eitherAOrB)
        {
            if (eitherAOrB == _a)
                return _b;
            else if (eitherAOrB == _b)
                return _a;
            else
                throw new InvalidOperationException(
                    string.Format("oneOfAOrB - currently {0} but only {1} and {2} present",
                    eitherAOrB.ToString(), _a.ToString(), _b.ToString()));
        }
        
        /// <summary>
        /// A textual representation of the edge
        /// </summary>
        /// <returns>a string with "{A}, {B}"</returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}", _a.ToString(), _b.ToString());
        }
    }
}
