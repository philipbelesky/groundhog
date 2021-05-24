namespace Groundhog
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Grasshopper.Kernel;
    using Groundhog.Properties;
    using Rhino.Geometry;

    public class PShowerMeshCanopyComponent : PShowerBase
    {
        public List<Mesh> canopyMeshes;

        public PShowerMeshCanopyComponent()
            : base("Plant Appearance (canopy mesh)", "Shower (canopy mesh)",
            "Simulate the appearance of a particular plant instance's canopy using a mesh between canopy and trunk")
        {
        }

        protected override Bitmap Icon => Resources.icon_pshower;

        public override Guid ComponentGuid => new Guid("{d5df9bcc-b4a9-48df-8c88-c0b7e4322668}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            base.RegisterInputParams(pManager);
            pManager.AddIntegerParameter("Sides", "S",
                "The number of polygon sides for each mesh. Higher numbers will create more complex geometry",
                GH_ParamAccess.item, PLANT_SIDES);
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Canopies", "Cr", "The mesh of each plant's canopy spread'", GH_ParamAccess.list);
            base.RegisterOutputParams(pManager);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            if (!SetupSharedVariables(DA))
                return;

            DA.GetData(3, ref PLANT_SIDES);
            // Negative time values mean don't calculate/show plants (useful for successional schemes)
            if (this.PLANT_SIDES < 3)
            {
                AddRuntimeMessage(
                    GH_RuntimeMessageLevel.Remark, "The specified plant sides were less than 3, so have been set to 3.");
                PLANT_SIDES = 3;
            }

            // Create holder variables for output parameters
            this.canopyMeshes = new List<Mesh>();
            for (var i = 0; i < PLANT_SPECIES.Count; i++)
            {
                var plantInstance = GetPlantInstance(PLANT_SPECIES, i, this.allLabels, allColours);
                canopyMeshes.Add(plantInstance.GetCrownMesh(PLANT_LOCATIONS[i], PLANT_TIME, PLANT_SIDES));
            }

            // Assign variables to output parameters
            DA.SetDataList(0, canopyMeshes);
            DA.SetDataList(1, allColours);
            DA.SetDataList(2, allLabels);
        }
    }
}