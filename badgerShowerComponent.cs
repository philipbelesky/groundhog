using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace badger
{
    public class badgerShowerComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public badgerShowerComponent()
            : base("Plant Appearance Simulator", "Shower",
                "Simulate the appearance of a particular plant instance",
                "Badger", "Flora")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Plants", "P", "The plant objects to simulate", GH_ParamAccess.list);
            pManager.AddPointParameter("Locations", "L", "The plant locations to simulate", GH_ParamAccess.list);
            pManager.AddNumberParameter("Times", "T", "The time (in years) since planting to display", GH_ParamAccess.list);
            pManager[2].Optional = true;
            //pManager.AddBooleanParameter("Visualisations", "V", "Whether to show a full L-system visualisation (true) or just the base geoemtries (false)", GH_ParamAccess.item, false);
            //pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCircleParameter("Trunk", "T", "Trunk radius", GH_ParamAccess.list);
            pManager.AddCircleParameter("Root", "R", "Root radius", GH_ParamAccess.list);
            pManager.AddCircleParameter("Crown", "C", "Crown radius", GH_ParamAccess.list);
            pManager.AddCircleParameter("Spacing", "S", "Spacing radius", GH_ParamAccess.list);
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
            // Create holder variables for input parameters
            List<PlantSpecies> plantSpecies = new List<PlantSpecies>();
            List<Point3d> plantLocations = new List<Point3d>();
            List<double> plantTimes = new List<double>();

            // Access and extract data from the input parameters individually
            DA.GetDataList(1, plantLocations);
            DA.GetDataList(2, plantTimes);

            // Need to unwrap the species from generic list to a plantSpecies list
            List<GH_ObjectWrapper> wrappedSpecies = new List<GH_ObjectWrapper>();
            if (!DA.GetDataList<GH_ObjectWrapper>(0, wrappedSpecies)) return;
            foreach (GH_ObjectWrapper unwrappedObject in wrappedSpecies)
            {
                plantSpecies.Add(unwrappedObject.Value as PlantSpecies);
            }

                        
            // We should now validate the data and warn the user if invalid data is supplied.
            if (plantLocations.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "tdtdtdtd: make this a useful check 1");
                return;
            }
            else if (plantLocations.Count > plantSpecies.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "There were more locations than plants, so locations have been truncated");
                plantLocations.RemoveRange(plantSpecies.Count, plantLocations.Count - plantSpecies.Count);
            }

            if (plantSpecies.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "tdtdtdtd: make this a useful check 3");
                return;
            }
            else if (plantSpecies.Count > plantLocations.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "There were more plants than locations, so plants have been truncated");
                plantSpecies.RemoveRange(plantLocations.Count, plantSpecies.Count - plantLocations.Count);
            }

            if (plantTimes.Count < plantSpecies.Count)
            {
                for (int i = 0; i <= (plantSpecies.Count - plantTimes.Count); i++)
                {
                    plantTimes.Add(50.0);
                }
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "There were more species than times, so have auto-added new times set to 50");
            }

                         
            // Create holder variables for output parameters
            List<Circle> allTrunks = new List<Circle>();
            List<Circle> allRoots = new List<Circle>();
            List<Circle> allCrowns = new List<Circle>();

            Rhino.RhinoApp.WriteLine("Total species {0}", plantSpecies.Count);
            for (int i = 0; i < plantSpecies.Count; i++)
            {
                Rhino.RhinoApp.WriteLine("_______");
                PlantSpecies plantInstance = plantSpecies[i];
                Rhino.RhinoApp.WriteLine("  starting on {0}", plantInstance.speciesName);

                allTrunks.Add(plantInstance.getTrunk(plantLocations[i], plantTimes[i]));
                allRoots.Add(plantInstance.getRoot(plantLocations[i], plantTimes[i]));
                allCrowns.Add(plantInstance.getCrown(plantLocations[i], plantTimes[i]));
            }
            
            // Assign variables to output parameters
            DA.SetDataList(0, allTrunks);
            DA.SetDataList(1, allRoots);
            DA.SetDataList(2, allCrowns);
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
                return badger.Properties.Resources.icon_flora;
            }
        }
        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{2d268bdc-ecaa-4cf7-815a-c8111d1798d6}"); }
        }
    }
}
