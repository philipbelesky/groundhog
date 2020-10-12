namespace Groundhog.Hydro
{
    using System.Collections.Generic;
    using System.Linq;
    using Grasshopper.Kernel;
    using Rhino;
    using Rhino.Geometry;

    public abstract class FlowPathBase : GroundHogComponent
    {
        public double FLOW_FIDELITY = 1000.0;
        public int FLOW_LIMIT;
        public List<Point3d> FLOW_ORIGINS;
        public Point3d[] startPoints;
        public bool THREAD;        
        private double basinTreshold; // If the distance between each point is less than this amount then terminate the path

        // Pass the constructor parameters up to the main GH_Component abstract class
        protected FlowPathBase(string name, string nickname, string description)
            : base(name, nickname, description, "Groundhog", "Hydro")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "P",
                "Start points for the flow paths. These should be above the mesh (they will be projected on to it)",
                GH_ParamAccess.list);
            pManager.AddNumberParameter("Jump", "J",
                "Amount to move for each flow iteration. Small numbers may take a long time to compute and negative values go uphill. If not specified or set to 0 a (hopefully) sensible step size will be calculated.",
                GH_ParamAccess.item, 0);
            pManager[2].Optional = true;
            pManager.AddIntegerParameter("Steps", "L",
                "A limit to the number of flow iterations. Leave unset or to 0 for an unlimited set of iterations",
                GH_ParamAccess.item, 0);
            pManager[3].Optional = true;
            pManager.AddBooleanParameter("Thread", "T",
                "Whether to multithread the solution (this can speed up long calculations)", GH_ParamAccess.item,
                false);
            pManager[4].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Flow Points", "F", "The points of each simulated flow path 'jump'", GH_ParamAccess.tree);
            pManager.AddCurveParameter("Flow Paths", "C", "A polyline linking each of the flow points into a path", GH_ParamAccess.list);
        }

        protected bool SetupSharedVariables(IGH_DataAccess DA, BoundingBox bbox)
        {
            FLOW_ORIGINS = new List<Point3d>();
            if (!DA.GetDataList(1, this.FLOW_ORIGINS)) return false;

            // If referenced points get deleted in Rhino they default to 0,0,0 thus make a list that excludes these point types first
            var nullPoint = new Point3d(0, 0, 0);
            var validFlowPathPoints = from point in this.FLOW_ORIGINS where point != nullPoint select point;

            if (!validFlowPathPoints.Any())
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No valid points have been provided; perhaps check that you have not provided null or invalid points?");
                return false;
            }

            this.startPoints = validFlowPathPoints.ToArray(); // Array for multithreading

            DA.GetData(2, ref this.FLOW_FIDELITY);
            if (this.FLOW_FIDELITY == 0) this.FLOW_FIDELITY = FlowPathCalculations.GetSensibleFidelity(this.startPoints, bbox);
            this.basinTreshold = this.FLOW_FIDELITY * 0.25;

            DA.GetData(3, ref FLOW_LIMIT);
            DA.GetData(4, ref this.THREAD);

            return true;
        }

        protected List<Point3d> DispatchFlowPoints(bool isMesh, Mesh flowMesh, Brep flowBrep, Point3d initialStartPoint)
        {
            var flowPoints = new List<Point3d>(); // Holds each step

            if (isMesh)
                flowPoints.Add(flowMesh.ClosestPoint(initialStartPoint));
            else
                flowPoints.Add(flowBrep.ClosestPoint(initialStartPoint));

            var startPoint = flowPoints[flowPoints.Count - 1];
            while (true)
            {
                Point3d nextPoint;
                if (isMesh)
                    nextPoint = this.GetNextFlowStepOnMesh(flowMesh, startPoint);
                else
                    nextPoint = this.GetNextFlowStepOnSurface(flowBrep, startPoint);

                if (nextPoint.DistanceTo(startPoint) <= RhinoDoc.ActiveDoc.ModelAbsoluteTolerance)
                    break; // Test the point has actully moved
                else if (nextPoint.Z >= startPoint.Z && this.FLOW_FIDELITY > 0) // When going downhill; break on moving up
                    break; // Test this point is actually lower
                else if (nextPoint.Z <= startPoint.Z && this.FLOW_FIDELITY < 0) // When going uphill; break on moving down
                    break; // Test this point is actually lower
                else if (this.FLOW_LIMIT != 0 && this.FLOW_LIMIT < flowPoints.Count)
                    break; // Stop if iteration limit reached
                else if (nextPoint.DistanceTo(startPoint) < this.basinTreshold)
                    break; // Distance between points can be less than FLOW_FIDELITY when the downhill vector is off of the surface/mesh 
                           // so upon finding the closest point it snaps back to a closer position on the surface/mesh
                           // These points tend to get hyper compressed and throw off other calculations

                flowPoints.Add(nextPoint);
                startPoint = nextPoint; // Checks out; iterate on
            }

            return flowPoints;
        }

        private Point3d GetNextFlowStepOnMesh(Mesh flowMesh, Point3d startPoint)
        {
            double maximumDistance = 0; // TD: setting this as +ve speeds up the search?
            Vector3d closestNormal;
            Point3d closestPoint;

            // Get closest point
            flowMesh.ClosestPoint(startPoint, out closestPoint, out closestNormal, maximumDistance);
            // Get the next point following the vector
            var nextFlowPoint = FlowPathCalculations.MoveFlowPoint(closestNormal, closestPoint, this.FLOW_FIDELITY);
            // Need to snap back to the surface (the vector may be pointing off the edge)
            return flowMesh.ClosestPoint(nextFlowPoint);
        }

        private Point3d GetNextFlowStepOnSurface(Brep flowSurface, Point3d startPoint)
        {
            double closestS, closestT;
            double maximumDistance = 0; // TD: setting this as +ve speeds up the search?
            Vector3d closestNormal;
            ComponentIndex closestCI;
            Point3d closestPoint;

            // Get closest point
            flowSurface.ClosestPoint(startPoint, out closestPoint, out closestCI, out closestS, out closestT,
                maximumDistance, out closestNormal);
            // Get the next point following the vector
            var nextFlowPoint = FlowPathCalculations.MoveFlowPoint(closestNormal, closestPoint, FLOW_FIDELITY);
            // Need to snap back to the surface (the vector may be pointing off the edge)
            return flowSurface.ClosestPoint(nextFlowPoint);
        }
    }
}