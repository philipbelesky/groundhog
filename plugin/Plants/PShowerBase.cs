using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace groundhog
{
    public abstract class PShowerBase : GroundHogComponent
    {
        protected double plantTime = 10.0;
        protected int plantSides = 8;
        protected List<Point3d> plantLocations = new List<Point3d>();
        protected List<PlantSpecies> plantSpecies = new List<PlantSpecies>();
        protected List<Color> allColours = new List<Color>();
        protected List<GH_String> allLabels = new List<GH_String>();

        // Pass the constructor parameters up to the main GH_Component abstract class
        protected PShowerBase(string name, string nickname, string description)
            : base(name, nickname, description, "Groundhog", "Flora")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Species", "S", "The plant attributes to simulate", GH_ParamAccess.list);
            pManager.AddPointParameter("Locations", "L", "The locations to assign to each attribute",
                GH_ParamAccess.list);
            pManager.AddNumberParameter("Times", "T", "The time (in years) since initial planting to display",
                GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddColourParameter("Color", "Co", "The species color of each plant", GH_ParamAccess.list);
            pManager.AddTextParameter("Label", "La", "The species label of each plant", GH_ParamAccess.list);
        }

        protected bool SetupSharedVariables(IGH_DataAccess DA)
        {
            // Inits shared variables; a false return triggers inheriting classes to return

            // Need to unwrap the species from generic list to a plantSpecies list
            var wrappedSpecies = new List<GH_ObjectWrapper>();
            if (!DA.GetDataList(0, wrappedSpecies)) return false;
            foreach (var unwrappedObject in wrappedSpecies)
                plantSpecies.Add(unwrappedObject.Value as PlantSpecies);
            if (plantSpecies.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                    "There were no species provided for the specified locations");
                return false;
            }

            DA.GetDataList(1, plantLocations);
            if (plantLocations.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark,
                    "There were no locations provided for the specified species");
                return false;
            }

            DA.GetData(2, ref plantTime);
            // Negative time values mean don't calculate/show plants (useful for successional schemes)
            if (plantTime < 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark,
                    "The specified time was less than zero so no species have been allocated locations.");
                return false;
            }
            
            if (plantLocations.Count > plantSpecies.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark,
                    "There were more locations provided than species, so some locations have not been allocated plants");
                plantLocations.RemoveRange(plantSpecies.Count, plantLocations.Count - plantSpecies.Count);
            }
            if (plantSpecies.Count > plantLocations.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark,
                    "There were more species provided than locations, so some species have not been allocated locations");
                plantSpecies.RemoveRange(plantLocations.Count, plantSpecies.Count - plantLocations.Count);
            }
   
            return true;
        }
        
        protected PlantSpecies GetPlantInstance(List<PlantSpecies> plantSpecies, int index, Random rand,
            List<GH_String> allLabels, List<Color> allColours)
        {
            var plantInstance = plantSpecies[index];
            plantInstance.SetVarianceValues(rand);
            allLabels.Add(plantInstance.GetLabel());
            allColours.Add(plantInstance.GetColor());
            return plantInstance;
        }
    }
}