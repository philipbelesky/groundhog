using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace badger
{
    public class badgerPImportComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public badgerPImportComponent()
            : base("Species Attribute Importer", "PImport",
                "Create plant attributes from an imported spreadsheet",
                "Badger", "Flora")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("CSV File", "C", "The output of a CSV set to a Read File component", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // Generic is its a GH_ObjectWrapper wrapper for our custom class
            pManager.Register_GenericParam("Plants", "P", "The resulting plant objects");
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Create holder variables for input parameters
            List<string> csvContents = new List<string>();

            // Access and extract data from the input parameters individually
            if (!DA.GetDataList(0, csvContents)) return;

            // Create holder variables for output parameters
            List<PlantSpecies> csvPlantSpecies = new List<PlantSpecies>();

            // Format
            string csvHeaders = csvContents[0];
            csvContents.Remove(csvHeaders);

            foreach (string csvValue in csvContents)
            {
                Dictionary<string, string> instanceDictionary = PlantFactory.parseToDictionary(csvHeaders, csvValue);
                var createSpecies = PlantFactory.parseFromDictionary(instanceDictionary);

                PlantSpecies instanceSpecies = createSpecies.Item1;
                string instanceWarnings = createSpecies.Item2;
                if (instanceWarnings.Length > 0)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Species " + instanceSpecies.speciesName + " has " + instanceWarnings);
                }

                csvPlantSpecies.Add(instanceSpecies);
            }

            // Need to add each species instance to a generic wrapper so they can be output
            List<GH_ObjectWrapper> wrappedSpecies = new List<GH_ObjectWrapper>();
            foreach (PlantSpecies species in csvPlantSpecies)
            {
                wrappedSpecies.Add(new GH_ObjectWrapper(species));
            }
            // Assign variables to output parameters
            DA.SetDataList(0, wrappedSpecies);

        }

 

           /// <summary>
        /// The Exposure property controls where in the panel a component icon 
        /// will appear. There are seven possible locations (primary to septenary), 
        /// each of which can be combined with the GH_Exposure.obscure flag, which 
        /// ensures the component will only be visible on panel dropdowns.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return badger.Properties.Resources.icon_pimport;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{2d268bdc-ecaa-4cf7-815a-c8111d1798d4}"); }
        }
    }
}
