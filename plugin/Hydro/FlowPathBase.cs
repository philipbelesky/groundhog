using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace groundhog.Hydro
{
    public abstract class FlowPathBase : GroundHogComponent
    {
        protected double FLOW_FIDELITY = 1000.0;
        protected int FLOW_LIMIT = 0;
        protected bool THREAD = false;
        protected Point3d[] startPoints;
        protected List<Point3d> FLOW_ORIGINS = new List<Point3d>();

        // Pass the constructor parameters up to the main GH_Component abstract class
        protected FlowPathBase(string name, string nickname, string description)
            : base(name, nickname, description, "Groundhog", "Hydro")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "P", "Start points for the flow paths. These should be above the mesh (they will be projected on to it)", GH_ParamAccess.list);
            pManager.AddNumberParameter("Jump", "J", "Amount to move for each flow iteration. Small numbers may take a long time to compute and negative values go uphill. If not specified or set to 0 a (hopefully) sensible step size will be calculated.", GH_ParamAccess.item, 0);
            pManager[2].Optional = true;
            pManager.AddIntegerParameter("Steps", "L", "A limit to the number of flow iterations. Leave unset or to 0 for an unlimited set of iterations", GH_ParamAccess.item, 0);
            pManager[3].Optional = true;
            pManager.AddBooleanParameter("Thread", "T", "Whether to multithread the solution (this can speed up long calculations)", GH_ParamAccess.item, false);
            pManager[4].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Flow Points", "F", "The points of each simulated flow path 'jump'", GH_ParamAccess.tree);
            pManager.AddCurveParameter("Flow Paths", "C", "A polyline linking each of the flow points into a path", GH_ParamAccess.list);
        }

        protected bool SetupSharedVariables(IGH_DataAccess DA, BoundingBox bbox)
        {
            if (!DA.GetDataList(1, FLOW_ORIGINS)) return false;

            // If referenced points get deleted in Rhino they default to 0,0,0 thus make a list that excludes these point types first
            var nullPoint = new Point3d(0, 0, 0);
            IEnumerable<Point3d> validFlowPathPoints = from point in FLOW_ORIGINS where point != nullPoint select point;

            if (validFlowPathPoints.Count() < 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No valid points have been provided; perhaps check that you have not provided null or invalid points?");
                return false;
            }

            startPoints = validFlowPathPoints.ToArray(); // Array for multithreading
            
            DA.GetData(2, ref FLOW_FIDELITY);
            if (FLOW_FIDELITY == 0)
            {
                FLOW_FIDELITY = FlowCalculations.getSensibleFidelity(startPoints, bbox);
            }

            DA.GetData(3, ref FLOW_LIMIT);
            DA.GetData(4, ref THREAD);

            return true;
        }


    }
}
