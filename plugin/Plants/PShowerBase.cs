using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace groundhog
{
    public abstract class PShowerBase : GroundHogComponent
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;

        // Pass the constructor parameters up to the main GH_Component abstract class
        protected PShowerBase(string name, string nickname, string description)
            : base(name, nickname, description, "Groundhog", "Flora")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Species", "S", "The plant attributes to simulate", GH_ParamAccess.list);
            pManager.AddPointParameter("Locations", "L", "The locations to assign to each attribute", GH_ParamAccess.list);
            pManager.AddNumberParameter("Times", "T", "The time (in years) since initial planting to display", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddColourParameter("Color", "Co", "The species color of each plant", GH_ParamAccess.list);
            pManager.AddTextParameter("Label", "La", "The species label of each plant", GH_ParamAccess.list);
        }

        public int GetPlantSides(IGH_DataAccess DA) // For mesh components only
        {
            var plantSides = 8;
            DA.GetData(3, ref plantSides);
            // Negative time values mean don't calculate/show plants (useful for successional schemes)
            if (plantSides < 3)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "The specified plant sides were less than 3, so have been set to 3.");
                plantSides = 3;
            }

            return plantSides;
        }

        public double? GetSimulatedTime(IGH_DataAccess DA)
        {
            var plantTime = 10.0; // default value
            DA.GetData(2, ref plantTime);
            // Negative time values mean don't calculate/show plants (useful for successional schemes)
            if (plantTime < 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "The specified time was less than zero so no species have been allocated locations.");
                return null;
            }

            return plantTime;
        }

        public List<Point3d> GetSpeciesLocations(IGH_DataAccess DA)
        {
            var plantLocations = new List<Point3d>();
            DA.GetDataList(1, plantLocations);
            if (plantLocations.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "There were no locations provided for the specified species");
                return null;
            }

            return plantLocations;
        }

        public List<PlantSpecies> GetSpeciesInputs(IGH_DataAccess DA, List<Point3d> plantLocations)
        {
            var plantSpecies = new List<PlantSpecies>();

            // Need to unwrap the species from generic list to a plantSpecies list
            var wrappedSpecies = new List<GH_ObjectWrapper>();
            if (!DA.GetDataList(0, wrappedSpecies)) return null;
            foreach (var unwrappedObject in wrappedSpecies)
                plantSpecies.Add(unwrappedObject.Value as PlantSpecies);
            
            if (plantSpecies.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "There were no species provided for the specified locations");
                return null;
            }

            // We should now validate the data and warn the user if invalid data is supplied.
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
            return plantSpecies;
        }

        public PlantSpecies GetPlantInstance(List<PlantSpecies> plantSpecies, int index, Random rand, 
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
