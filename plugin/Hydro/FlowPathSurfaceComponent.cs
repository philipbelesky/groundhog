using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using groundhog.Hydro;
using groundhog.Properties;
using Rhino.Geometry;

namespace groundhog
{
    public class GroundhogFlowSurfaceComponent : FlowPathBase
    {
        public GroundhogFlowSurfaceComponent()
            : base("Flow Projection (Surface)", "Srf Flows", "Construct flow paths over a surface")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.icon_flows_srf;

        public override Guid ComponentGuid => new Guid("{2d268bdc-ecaa-4cf7-815a-c8111d1798d1}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "Base landscape form (as a surface) for the flow calculation",
                GH_ParamAccess.item);
            base.RegisterInputParams(pManager);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            var FLOW_SURFACE = default(Surface);
            DA.GetData(0, ref FLOW_SURFACE);
            if (FLOW_SURFACE == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                    "A null item has been provided as the Surface input; please correct this input.");
                return;
            }

            var FLOW_BREP = default(Brep);
            if (FLOW_SURFACE != default(Surface)) FLOW_BREP = FLOW_SURFACE.ToBrep();

            if (!SetupSharedVariables(DA, FLOW_SURFACE.GetBoundingBox(false)))
                return;

            // End initial variable setup

            var allFlowPathPoints = new List<Point3d>[startPoints.Length]; // Array of all the paths
            if (THREAD)
                Parallel.For(0, startPoints.Length, i => // Shitty multithreading
                    {
                        allFlowPathPoints[i] = DispatchFlowPoints(false, null, FLOW_BREP, startPoints[i]);
                    }
                );
            else
                for (var i = 0; i < startPoints.Length; i = i + 1)
                    allFlowPathPoints[i] = DispatchFlowPoints(false, null, FLOW_BREP, startPoints[i]);

            var outputs = FlowCalculations.MakeOutputs(allFlowPathPoints);

            // Assign variables to output parameters
            DA.SetDataTree(0, outputs.Item1);
            DA.SetDataList(1, outputs.Item2);
        }
    }
}