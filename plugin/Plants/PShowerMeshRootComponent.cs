namespace Groundhog
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Grasshopper.Kernel;
    using Grasshopper.Kernel.Types;
    using Groundhog.Properties;
    using Rhino.Geometry;

    public class PShowerMeshRootComponent : PShowerBase
    {
        public PShowerMeshRootComponent()
            : base("Plant Appearance (root mesh)", "Shower (root mesh)",
            "Simulate the appearance of a particular plant instance's root using a mesh between canopy and trunk")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.icon_pshower;

        public override Guid ComponentGuid => new Guid("{d310a8ed-bed6-4eac-9cfb-6436bb918d98}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            base.RegisterInputParams(pManager);
            pManager.AddIntegerParameter("Sides", "S",
                "The number of polygon sides for each mesh. Higher numbers will create more complex geometry",
                GH_ParamAccess.item);
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Roots", "Ro", "The mesh of each plant's root spread'", GH_ParamAccess.list);
            base.RegisterOutputParams(pManager);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            if (!this.SetupSharedVariables(DA))
                return;

            DA.GetData(3, ref this.PLANT_SIDES);
            // Negative time values mean don't calculate/show plants (useful for successional schemes)
            if (this.PLANT_SIDES < 3)
            {
                this.AddRuntimeMessage(
                    GH_RuntimeMessageLevel.Remark, "The specified plant sides were less than 3, so have been set to 3.");
                this.PLANT_SIDES = 3;
            }

            // Create holder variables for output parameters
            var rootMeshes = new List<Mesh>();
            for (var i = 0; i < this.PLANT_SPECIES.Count; i++)
            {
                var plantInstance = GetPlantInstance(this.PLANT_SPECIES, i, this.allLabels, this.allColours);
                rootMeshes.Add(plantInstance.GetRootMesh(this.PLANT_LOCATIONS[i], this.PLANT_TIME, this.PLANT_SIDES));
            }

            // Assign variables to output parameters
            DA.SetDataList(0, rootMeshes);
            DA.SetDataList(1, this.allColours);
            DA.SetDataList(2, this.allLabels);
        }
    }
}