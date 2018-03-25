using System;
using System.Collections.Generic;
using System.Drawing;
using groundhog.Properties;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace groundhog
{
    public class groundhogPImportComponent : GroundHog_Component
    {

        public groundhogPImportComponent()
            : base("Species Attribute Importer", "PImport", "Create plant attributes from an imported spreadsheet", "Groundhog", "Flora")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.icon_pimport;

        public override Guid ComponentGuid => new Guid("{2d268bdc-ecaa-4cf7-815a-c8111d1798d4}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("CSV File", "C", "The contents of a CSV file (use the output of a Read File component", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            // Generic is its a GH_ObjectWrapper wrapper for our custom class
            pManager.Register_GenericParam("Plants", "P", "The resulting plant objects");
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Create holder variables for input parameters
            var csvContents = new List<string>();

            // Access and extract data from the input parameters individually
            if (!DA.GetDataList(0, csvContents)) return;

            // Create holder variables for output parameters
            var csvPlantSpecies = new List<PlantSpecies>();

            // Format
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

            // Need to add each species instance to a generic wrapper so they can be output
            var wrappedSpecies = new List<GH_ObjectWrapper>();
            foreach (var species in csvPlantSpecies)
                wrappedSpecies.Add(new GH_ObjectWrapper(species));
            // Assign variables to output parameters
            DA.SetDataList(0, wrappedSpecies);
        }
    }
}