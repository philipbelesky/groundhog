﻿namespace Groundhog
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Grasshopper.Kernel;
    using Groundhog.Properties;
    using Rhino.Geometry;

    public class PShowerDiscsComponent : PShowerBase
    {
        public PShowerDiscsComponent()
            : base("Plant Appearance (discs)", "Shower (discs)",
            "Simulate the appearance of a particular plant instance using circles")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.icon_pshower;

        public override Guid ComponentGuid => new Guid("{2d268bdc-ecaa-4cf7-815a-c8111d1798d6}");

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCircleParameter("Trunk", "Tr", "Trunk radius", GH_ParamAccess.list);
            pManager.AddCircleParameter("Root", "Rr", "Root radius", GH_ParamAccess.list);
            pManager.AddCircleParameter("Crown", "Cr", "Crown radius", GH_ParamAccess.list);
            pManager.AddCircleParameter("Spacing", "Sr", "Spacing radius", GH_ParamAccess.list);
            base.RegisterOutputParams(pManager);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            if (!this.SetupSharedVariables(DA))
                return;

            // Create holder variables for output parameters
            var allRoots = new List<Circle>();
            var allCrowns = new List<Circle>();
            var allSpacings = new List<Circle>();
            var allTrunks = new List<Circle>();

            for (var i = 0; i < this.PLANT_SPECIES.Count; i++)
            {
                var plantInstance = GetPlantInstance(this.PLANT_SPECIES, i, this.allLabels, this.allColours);
                allTrunks.Add(plantInstance.GetTrunkDisc(this.PLANT_LOCATIONS[i], this.PLANT_TIME));
                allRoots.Add(plantInstance.GetRootDisc(this.PLANT_LOCATIONS[i], this.PLANT_TIME));
                allCrowns.Add(plantInstance.GetCrownDisc(this.PLANT_LOCATIONS[i], this.PLANT_TIME));
                allSpacings.Add(plantInstance.GetSpacingDisc(this.PLANT_LOCATIONS[i]));
                this.allLabels.Add(plantInstance.GetLabel());
            }

            // Assign variables to output parameters
            DA.SetDataList(0, allTrunks);
            DA.SetDataList(1, allRoots);
            DA.SetDataList(2, allCrowns);
            DA.SetDataList(3, allSpacings);
            DA.SetDataList(4, this.allColours);
            DA.SetDataList(5, this.allLabels);
        }
    }
}