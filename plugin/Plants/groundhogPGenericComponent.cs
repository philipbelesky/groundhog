using System;
using System.Collections.Generic;
using System.Drawing;
using groundhog.Properties;
using Grasshopper.Kernel;

namespace groundhog
{
    public class groundhogPGenericComponent : GroundHog_Component
    {

        public groundhogPGenericComponent()
            : base("Generic Species", "PGeneric", "Output plant objects from pre-define generic types ", "Groundhog", "Flora")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.icon_pgeneric;

        public override Guid ComponentGuid => new Guid("{2d268bdc-ecaa-4cf7-815a-c8111d1798d3}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Shrub", "S", "Generic Shrub (placeholder data)", GH_ParamAccess.item);
            pManager.AddGenericParameter("Grass", "G", "Generic Grass (placeholder data)", GH_ParamAccess.item);
            pManager.AddGenericParameter("Tree", "T", "Generic Tree (placeholder data)", GH_ParamAccess.item);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            var csvPlantSpecies = new List<PlantSpecies>();

            var csvStrings = Resources.generic_plants.Split('\n');
            var csvContents = new List<string>(csvStrings);

            var csvHeaders = csvContents[0];
            csvContents.Remove(csvHeaders);

            foreach (var csvValue in csvContents)
            {
                var instanceDictionary = PlantFactory.ParseToDictionary(csvHeaders, csvValue);
                var createSpecies = PlantFactory.ParseFromDictionary(instanceDictionary);

                var instanceSpecies = createSpecies.Item1;
                var instanceWarnings = createSpecies.Item2;
                if (instanceWarnings.Length > 0)
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                        "Species " + instanceSpecies.speciesName + " has " + instanceWarnings);

                csvPlantSpecies.Add(instanceSpecies);
            }

            // Assign variables to output parameters
            var shrub = csvPlantSpecies.Find(i => i.speciesName == "Generic Shrub");
            DA.SetData(0, shrub);

            var grass = csvPlantSpecies.Find(i => i.speciesName == "Generic Grass");
            DA.SetData(1, grass);

            var tree = csvPlantSpecies.Find(i => i.speciesName == "Generic Tree");
            DA.SetData(2, tree);
        }
    }
}