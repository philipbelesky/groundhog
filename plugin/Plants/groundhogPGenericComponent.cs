using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace groundhog
{
    public class groundhogPGenericComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear,
        /// Subcategory the panel. If you use non-existing tab or panel names,
        /// new tabs/panels will automatically be created.
        /// </summary>
        public groundhogPGenericComponent()
            : base("Generic Species Attributes", "Generic Species",
                "Output plant objects from pre-define generic types",
                "Groundhog", "Flora")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            var sHelp = "Placeholder description 1";
            pManager.AddGenericParameter("ShrubP", "ShrubP", sHelp, GH_ParamAccess.item);
            var gHelp = "Placeholder description 2";
            pManager.AddGenericParameter("GrassP", "GrassP", gHelp, GH_ParamAccess.item);
            var tHelp = "Placeholder description 3";
            pManager.AddGenericParameter("TreeP", "TreeP", tHelp, GH_ParamAccess.item);
            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            //pManager.HideParameter(0);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<PlantSpecies> csvPlantSpecies = new List<PlantSpecies>();

            string[] csvStrings = Properties.Resources.generic_plants.Split('\n');
            List<string> csvContents = new List<string>(csvStrings);

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

            // Assign variables to output parameters
            DA.SetData(0, csvPlantSpecies.Find(i => i.speciesName == "Generic Shrub"));
            DA.SetData(1, csvPlantSpecies.Find(i => i.speciesName == "Generic Grass"));
            DA.SetData(2, csvPlantSpecies.Find(i => i.speciesName == "Generic Tree"));

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
                return groundhog.Properties.Resources.icon_pgeneric;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it.
        /// It is vital this Guid doesn't change otherwise old ghx files
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{2d268bdc-ecaa-4cf7-815a-c8111d1798d3}"); }
        }
    }
}
