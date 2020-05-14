using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using groundhog.Properties;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using groundhog.Hydro;
using Rhino;
using Rhino.Geometry;

namespace groundhog
{
    public class GroundhogFlowMeshComponent : FlowPathBase
    {
        public GroundhogFlowMeshComponent()
            : base("Flow Projection (Mesh)", "Mesh Flows", "Construct flow paths over a mesh")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.icon_flows_mesh;

        public override Guid ComponentGuid => new Guid("{2d218bdc-ecaa-2cf7-815a-c8111d1798d3}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Base landscape form (as a mesh) for the flow calculation", GH_ParamAccess.item);
            base.RegisterInputParams(pManager);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            var FLOW_MESH = default(Mesh);
            DA.GetData(0, ref FLOW_MESH);
            if (FLOW_MESH == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "A null item has been provided as the Mesh input; please correct this input.");
                return;
            }

            if (!SetupSharedVariables(DA, FLOW_MESH.GetBoundingBox(false)))
                return;

            // End initial variable setup

            var allFlowPathPoints = new List<Point3d>[startPoints.Length]; // Array of all the paths
            var flowPoints = new List<Point3d>();

            if (THREAD)
                Parallel.For(0, startPoints.Length, i => // Shitty multithreading
                    {
                        allFlowPathPoints[i] = DispatchFlowPoints(FLOW_MESH, startPoints[i], FLOW_FIDELITY, FLOW_LIMIT);
                    }
                );
            else
                for (var i = 0; i < startPoints.Length; i = i + 1)
                    allFlowPathPoints[i] = DispatchFlowPoints(FLOW_MESH, startPoints[i], FLOW_FIDELITY, FLOW_LIMIT);

            var outputs = FlowCalculations.MakeOutputs(allFlowPathPoints);
            // Assign variables to output parameters
            DA.SetDataTree(0, outputs.Item1);
            DA.SetDataList(1, outputs.Item2);
        }

        private List<Point3d> DispatchFlowPoints(Mesh FLOW_MESH, Point3d initialStartPoint,
                                                 double MOVE_DISTANCE, int FLOW_LIMIT)
        {
            var flowPoints = new List<Point3d>(); // Holds each step

            var startPoint = FLOW_MESH.ClosestPoint(initialStartPoint);
            flowPoints.Add(startPoint);

            while (true)
            {
                Point3d nextPoint;
                nextPoint = GetNextFlowStepOnMesh(FLOW_MESH, startPoint, MOVE_DISTANCE);

                if (nextPoint.DistanceTo(startPoint) <= RhinoDoc.ActiveDoc.ModelAbsoluteTolerance)
                    break; // Test the point has actully moved
                if (nextPoint.Z >= startPoint.Z && MOVE_DISTANCE > 0) // When going downhill; break on moving up
                    break; // Test this point is actually lower
                if (nextPoint.Z <= startPoint.Z && MOVE_DISTANCE < 0) // When going uphill; break on moving down
                    break; // Test this point is actually lower
                flowPoints.Add(nextPoint);
                if (FLOW_LIMIT != 0 && FLOW_LIMIT <= flowPoints.Count)
                    break; // Stop if iteration limit reached
                startPoint = nextPoint; // Checks out; iterate on
            }

            return flowPoints;
        }

        private Point3d GetNextFlowStepOnMesh(Mesh FLOW_MESH, Point3d startPoint, double MOVE_DISTANCE)
        {
            double maximumDistance = 0; // TD: setting this as +ve speeds up the search?
            Vector3d closestNormal;
            Point3d closestPoint;

            // Get closest point
            FLOW_MESH.ClosestPoint(startPoint, out closestPoint, out closestNormal, maximumDistance);
            // Get the next point following the vector
            var nextFlowPoint = FlowCalculations.MoveFlowPoint(closestNormal, closestPoint, MOVE_DISTANCE);
            // Need to snap back to the surface (the vector may be pointing off the edge)
            return FLOW_MESH.ClosestPoint(nextFlowPoint);
        }
    }
}