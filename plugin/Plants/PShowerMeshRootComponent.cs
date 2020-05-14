using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using groundhog.Properties;
using Rhino.Geometry;

namespace groundhog
{
    public class GroundhogShowerRootMeshComponent : PShowerBase
    {
        public GroundhogShowerRootMeshComponent() : base(
            "Plant Appearance (root mesh)", "Shower (root mesh)",
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
            var plantTimeTemp = GetSimulatedTime(DA);
            if (plantTimeTemp == null)
                return;
            var plantTime = plantTimeTemp.Value;

            var plantLocations = GetSpeciesLocations(DA);
            if (plantLocations == null) return;
            var plantSpecies = GetSpeciesInputs(DA, plantLocations);
            if (plantSpecies == null) return;
            var plantSides = GetPlantSides(DA);

            // Create holder variables for output parameters
            var rootMeshes = new List<Mesh>();
            var allColours = new List<Color>();
            var allLabels = new List<GH_String>();

            var rand = new Random(); // Random seed for plant variances
            for (var i = 0; i < plantSpecies.Count; i++)
            {
                var plantInstance = GetPlantInstance(plantSpecies, i, rand, allLabels, allColours);
                rootMeshes.Add(plantInstance.GetRootMesh(plantLocations[i], plantTime, plantSides));
            }

            // Assign variables to output parameters
            DA.SetDataList(0, rootMeshes);
            DA.SetDataList(1, allColours);
            DA.SetDataList(2, allLabels);
        }
    }
}