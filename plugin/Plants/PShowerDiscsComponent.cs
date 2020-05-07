using System;
using System.Collections.Generic;
using System.Drawing;
using groundhog.Properties;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace groundhog
{
    public class GroundhogShowerDiscsComponent : GroundHogComponent
    {
        public List<Color> allColours;
        public List<Mesh> canopyMeshes;
        public List<Mesh> rootMeshes;
        public List<GH_String> allLabels;

        public GroundhogShowerDiscsComponent()
            : base("Plant Appearance (discs)", "Shower (discs)", "Simulate the appearance of a particular plant instance using circles", "Groundhog", "Flora")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.icon_pshower;

        public override Guid ComponentGuid => new Guid("{2d268bdc-ecaa-4cf7-815a-c8111d1798d6}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Species", "S", "The plant attributes to simulate", GH_ParamAccess.list);
            pManager.AddPointParameter("Locations", "L", "The locations to assign to each attribute", GH_ParamAccess.list);
            pManager.AddNumberParameter("Times", "T", "The time (in years) since initial planting to display", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Canopies", nickname: "C", description: "The mesh of each plant's canopy spread'", GH_ParamAccess.list);
            pManager.AddMeshParameter("Roots", nickname: "R", description: "The mesh of each plant's root spread'", GH_ParamAccess.list);
            pManager.AddColourParameter("Color", "Co", "The species color of each plant", GH_ParamAccess.list);
            pManager.AddTextParameter("Label", "La", "The species label of each plant", GH_ParamAccess.list);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            // Create holder variables for input parameters
            var plantSpecies = new List<PlantSpecies>();
            var plantLocations = new List<Point3d>();
            var plantTime = 10.0; // default value

            // Access and extract data from the input parameters individually
            DA.GetDataList(1, plantLocations);
            DA.GetData(2, ref plantTime);

            // Negative time values mean don't calculate/show plants (useful for successional schemes)
            if (plantTime < 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "The specified time was less than zero so no species have been allocated locations.");
                return;
            }

            // Need to unwrap the species from generic list to a plantSpecies list
            var wrappedSpecies = new List<GH_ObjectWrapper>();
            if (!DA.GetDataList(0, wrappedSpecies)) return;
            foreach (var unwrappedObject in wrappedSpecies)
                plantSpecies.Add(unwrappedObject.Value as PlantSpecies);
            
            // We should now validate the data and warn the user if invalid data is supplied.
            if (plantLocations.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "There were no locations provided for the specified species");
                return;
            }
            if (plantLocations.Count > plantSpecies.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark,
                    "There were more locations provided than species, so some locations have not been allocated plants");
                plantLocations.RemoveRange(plantSpecies.Count, plantLocations.Count - plantSpecies.Count);
            }

            if (plantSpecies.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "There were no species provided for the specified locations");
                return;
            }
            if (plantSpecies.Count > plantLocations.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark,
                    "There were more species provided than locations, so some species have not been allocated locations");
                plantSpecies.RemoveRange(plantLocations.Count, plantSpecies.Count - plantLocations.Count);
            }

            // Create holder variables for output parameters
            canopyMeshes = new List<Mesh>();
            rootMeshes = new List<Mesh>();
            allColours = new List<Color>();
            allLabels = new List<GH_String>();

            for (var i = 0; i < plantSpecies.Count; i++)
            {
                var plantInstance = plantSpecies[i];
                canopyMeshes.Add(plantInstance.GetCrownMesh(plantLocations[i], plantTime));
                rootMeshes.Add(plantInstance.GetRootMesh(plantLocations[i], plantTime));
                allColours.Add(plantInstance.GetColor());
                allLabels.Add(plantInstance.GetLabel());
            }

            // Assign variables to output parameters
            DA.SetDataList(0, canopyMeshes);
            DA.SetDataList(1, rootMeshes);
            DA.SetDataList(2, allColours);
            DA.SetDataList(3, allLabels);
        }
    }
}