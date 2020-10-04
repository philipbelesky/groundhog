namespace Groundhog
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading.Tasks;
    using Grasshopper.Kernel;
    using Groundhog.Hydro;
    using Groundhog.Properties;
    using Rhino.Geometry;

    public class FlowPathMeshComponent : FlowPathBase
    {
        public FlowPathMeshComponent()
            : base("Flow Projection (Mesh)", "Mesh Flows", "Construct flow paths over a mesh")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.icon_flows_mesh;

        public override Guid ComponentGuid => new Guid("{2d218bdc-ecaa-2cf7-815a-c8111d1798d3}");

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Base landscape form (as a mesh) for the flow calculation",
                GH_ParamAccess.item);
            base.RegisterInputParams(pManager);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            var FLOW_MESH = default(Mesh);
            DA.GetData(0, ref FLOW_MESH);
            if (FLOW_MESH == null)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                    "A null item has been provided as the Mesh input; please correct this input.");
                return;
            }

            if (!SetupSharedVariables(DA, FLOW_MESH.GetBoundingBox(false)))
                return;

            // End initial variable setup
            var allFlowPathPoints = new List<Point3d>[this.startPoints.Length]; // Array of all the paths
            if (this.THREAD)
                Parallel.For(0, this.startPoints.Length, i => // Shitty multithreading
                {
                    allFlowPathPoints[i] = this.DispatchFlowPoints(true, FLOW_MESH, null, this.startPoints[i]);
                });
            else
                for (var i = 0; i < this.startPoints.Length; i = i + 1)
                    allFlowPathPoints[i] = this.DispatchFlowPoints(true, FLOW_MESH, null, this.startPoints[i]);

            var outputs = FlowPathCalculations.MakeOutputs(allFlowPathPoints);

            // Assign variables to output parameters
            DA.SetDataTree(0, outputs.Item1);
            DA.SetDataList(1, outputs.Item2);
        }
    }
}