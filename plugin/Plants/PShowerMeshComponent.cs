﻿using System;
using System.Collections.Generic;
using System.Drawing;
using groundhog.Properties;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace groundhog
{
    public class GroundhogShowerMeshComponent : GroundHogComponent
    {
        public List<Color> allColours;
        public List<Mesh> canopyMeshes;
        public List<Mesh> rootMeshes;
        public List<GH_String> allLabels;

        public GroundhogShowerMeshComponent()
            : base("Plant Appearance (mesh)", "Shower (mesh)", "Simulate the appearance of a particular plant instance using a mesh between canopy and trunk", "Groundhog", "Flora")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.icon_pshower;

        public override Guid ComponentGuid => new Guid("{d5df9bcc-b4a9-48df-8c88-c0b7e4322668}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Species", "S", "The plant attributes to simulate", GH_ParamAccess.list);
            pManager.AddPointParameter("Locations", "L", "The locations to assign to each attribute", GH_ParamAccess.list);
            pManager.AddNumberParameter("Times", "T", "The time (in years) since initial planting to display", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddIntegerParameter("Sides", "S", "The number of polygon sides for each mesh. Higher numbers will create more complex geometry", GH_ParamAccess.item);
            pManager[3].Optional = true;
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
            var plantSides = 4;

            // Access and extract data from the input parameters individually
            DA.GetDataList(1, plantLocations);
            DA.GetData(2, ref plantTime);
            DA.GetData(3, ref plantSides);

            // Negative time values mean don't calculate/show plants (useful for successional schemes)
            if (plantTime < 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "The specified time was less than zero so no species have been allocated locations.");
                return;
            }
            // Negative time values mean don't calculate/show plants (useful for successional schemes)
            if (plantSides < 3)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "The specified plant sides were less than 3, so have been set to 3.");
                plantSides = 3;
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
                canopyMeshes.Add(plantInstance.GetCrownMesh(plantLocations[i], plantTime, plantSides));
                rootMeshes.Add(plantInstance.GetRootMesh(plantLocations[i], plantTime, plantSides));
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