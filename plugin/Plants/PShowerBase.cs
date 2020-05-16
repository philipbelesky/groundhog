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
        protected List<Color> allColours;
        protected List<GH_String> allLabels;
        protected List<Point3d> PLANT_LOCATIONS;
        protected int PLANT_SIDES = 8;
        protected List<PlantSpecies> PLANT_SPECIES;
        protected double PLANT_TIME = 10.0;

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
            allColours = new List<Color>();
            allLabels = new List<GH_String>();

            PLANT_SPECIES = new List<PlantSpecies>();
            // Need to unwrap the species from generic list to a plantSpecies list
            var wrappedSpecies = new List<GH_ObjectWrapper>();
            if (!DA.GetDataList(0, wrappedSpecies)) return false;
            foreach (var unwrappedObject in wrappedSpecies)
                PLANT_SPECIES.Add(unwrappedObject.Value as PlantSpecies);
            if (PLANT_SPECIES.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                    "There were no species provided for the specified locations");
                return false;
            }

            PLANT_LOCATIONS = new List<Point3d>();
            DA.GetDataList(1, PLANT_LOCATIONS);
            if (PLANT_LOCATIONS.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark,
                    "There were no locations provided for the specified species");
                return false;
            }

            DA.GetData(2, ref PLANT_TIME);
            // Negative time values mean don't calculate/show plants (useful for successional schemes)
            if (PLANT_TIME < 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark,
                    "The specified time was less than zero so no species have been allocated locations.");
                return false;
            }

            if (PLANT_LOCATIONS.Count > PLANT_SPECIES.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark,
                    "There were more locations provided than species, so some locations have not been allocated plants");
                PLANT_LOCATIONS.RemoveRange(PLANT_SPECIES.Count, PLANT_LOCATIONS.Count - PLANT_SPECIES.Count);
            }

            if (PLANT_SPECIES.Count > PLANT_LOCATIONS.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark,
                    "There were more species provided than locations, so some species have not been allocated locations");
                PLANT_SPECIES.RemoveRange(PLANT_LOCATIONS.Count, PLANT_SPECIES.Count - PLANT_LOCATIONS.Count);
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