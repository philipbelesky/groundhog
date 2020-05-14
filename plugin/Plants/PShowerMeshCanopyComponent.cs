using System;
using System.Collections.Generic;
using System.Drawing;
using groundhog.Properties;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace groundhog
{
    public class GroundhogShowerCanopyMeshComponent : PShowerBase
    {
        public List<Mesh> canopyMeshes;

        public GroundhogShowerCanopyMeshComponent() : base(
            "Plant Appearance (canopy mesh)", "Shower (canopy mesh)", "Simulate the appearance of a particular plant instance's canopy using a mesh between canopy and trunk")
        {
        }
        
        protected override Bitmap Icon => Resources.icon_pshower;

        public override Guid ComponentGuid => new Guid("{d5df9bcc-b4a9-48df-8c88-c0b7e4322668}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            base.RegisterInputParams(pManager);
            pManager.AddIntegerParameter("Sides", "S", "The number of polygon sides for each mesh. Higher numbers will create more complex geometry", GH_ParamAccess.item);
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Canopies", "Cr", "The mesh of each plant's canopy spread'", GH_ParamAccess.list);
            base.RegisterOutputParams(pManager);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            var plantTimeTemp = GetSimulatedTime(DA);
            if (plantTimeTemp == null)
                return;
            double plantTime = plantTimeTemp.Value;

            var plantLocations = GetSpeciesLocations(DA);
            if (plantLocations == null) return;
            var plantSpecies = GetSpeciesInputs(DA, plantLocations);
            if (plantSpecies == null) return;
            var plantSides = GetPlantSides(DA);
            
            // Create holder variables for output parameters
            canopyMeshes = new List<Mesh>();
            var allColours = new List<Color>();
            var allLabels = new List<GH_String>();

            Random rand = new Random(); // Random seed for plant variances
            for (var i = 0; i < plantSpecies.Count; i++)
            {
                var plantInstance = GetPlantInstance(plantSpecies, i, rand, allLabels, allColours);
                canopyMeshes.Add(plantInstance.GetCrownMesh(plantLocations[i], plantTime, plantSides));
            }

            // Assign variables to output parameters
            DA.SetDataList(0, canopyMeshes);
            DA.SetDataList(1, allColours);
            DA.SetDataList(2, allLabels);
        }

    }
}