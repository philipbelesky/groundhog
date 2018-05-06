using System;
using System.Collections.Generic;
using System.Drawing;
using groundhog.Properties;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace groundhog
{
    public class GroundhogShowerComponent : GroundHogComponent
    {
        public List<Color> allColours;
        public List<Circle> allCrowns;
        public List<GH_String> allLabels;

        public List<Circle> allRoots;
        public List<Circle> allSpacings;
        public List<Circle> allTrunks;

        public GroundhogShowerComponent()
            : base("Plant Appearance", "Shower", "Simulate the appearance of a particular plant instance", "Groundhog", "Flora")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.icon_pshower;

        public override Guid ComponentGuid => new Guid("{2d268bdc-ecaa-4cf7-815a-c8111d1798d6}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Plants", "P", "The plant attributes to simulate", GH_ParamAccess.list);
            pManager.AddPointParameter("Locations", "L", "The locations to assign to each attribute", GH_ParamAccess.list);
            pManager.AddNumberParameter("Times", "T", "The time (in years) since initial planting to display", GH_ParamAccess.item);
            pManager[2].Optional = true;
            //pManager.AddBooleanParameter("Visualisations", "V", "Whether to show a full L-system visualisation (true) or just the base geoemtries (false)", GH_ParamAccess.item, false);
            //pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCircleParameter("Trunk", "T", "Trunk radius", GH_ParamAccess.list);
            pManager.AddCircleParameter("Root", "R", "Root radius", GH_ParamAccess.list);
            pManager.AddCircleParameter("Crown", "C", "Crown radius", GH_ParamAccess.list);
            pManager.AddCircleParameter("Spacing", "S", "Spacing radius", GH_ParamAccess.list);
            pManager.AddColourParameter("Color", "C", "The species color of each plant", GH_ParamAccess.list);
            pManager.AddTextParameter("Label", "T", "The species label of each plant", GH_ParamAccess.list);
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
            allRoots = new List<Circle>();
            allCrowns = new List<Circle>();
            allSpacings = new List<Circle>();
            allTrunks = new List<Circle>();
            allColours = new List<Color>();
            allLabels = new List<GH_String>();

            for (var i = 0; i < plantSpecies.Count; i++)
            {
                var plantInstance = plantSpecies[i];
                allTrunks.Add(plantInstance.getTrunk(plantLocations[i], plantTime));
                allRoots.Add(plantInstance.getRoot(plantLocations[i], plantTime));
                allCrowns.Add(plantInstance.getCrown(plantLocations[i], plantTime));
                allSpacings.Add(plantInstance.getSpacing(plantLocations[i]));
                allColours.Add(plantInstance.getColour());
                allLabels.Add(plantInstance.getLabel());
            }

            // Assign variables to output parameters
            DA.SetDataList(0, allTrunks);
            DA.SetDataList(1, allRoots);
            DA.SetDataList(2, allCrowns);
            DA.SetDataList(3, allSpacings);
            DA.SetDataList(4, allColours);
            DA.SetDataList(5, allLabels);
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            //base.DrawViewportWires(args);
            int i;
            for (i = 0; i < allTrunks.Count; i = i + 1)
            {
                args.Display.DrawCircle(allTrunks[i], allColours[i], 4);
                args.Display.DrawCircle(allRoots[i], allColours[i], 2);
                args.Display.DrawCircle(allCrowns[i], allColours[i], 1);
                args.Display.DrawCircle(allSpacings[i], Color.FromArgb(110, 110, 110), 1);
                var line = new Line(new Point3d(0, 0, 0), new Point3d(1000, 1000, 0));
                args.Display.DrawDottedLine(line, Color.FromArgb(0, 0, 0));
            }
        }
    }
}