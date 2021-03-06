﻿namespace Groundhog
{
    using System;
    using System.Drawing;
    using Grasshopper.Kernel;
    using Groundhog.Properties;
    using Rhino.Geometry;

    public class FloodRiverComponent : GroundHogComponent
    {
        public FloodRiverComponent()
            : base("River Flood Levels", "River Floods", "Examine flooding levels along a surface from a river source",
                "Groundhog", "Hydro")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{2d268bdc-ecaa-4cf7-815a-c8111d1798d0}");

        protected override Bitmap Icon => Resources.icon_floods_river;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Datum", "d", "The local mean sea level", GH_ParamAccess.item, 0d);
            pManager[0].Optional = true;
            pManager.AddNumberParameter("Year", "Y", "The year in which to simulate the flooding. Defaults to current.",
                GH_ParamAccess.item, DateTime.Now.Year);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Rise", "R",
                "The assumed local mean sea level rise per year to evalute in future scenarios. Defaults to 3",
                GH_ParamAccess.item, 3d);
            pManager[2].Optional = true;
            pManager.AddNumberParameter("Surge", "S", "The local mean height of a typical storm surge. Defaults to 500",
                GH_ParamAccess.item, 500d);
            pManager[3].Optional = true;
            pManager.AddNumberParameter("High Neap", "HN", "The local mean high water neap tide", GH_ParamAccess.item,
                1400d);
            pManager[4].Optional = true;
            pManager.AddNumberParameter("Low Neap", "LN", "The local mean low water neap tide", GH_ParamAccess.item,
                700d);
            pManager[5].Optional = true;
            pManager.AddNumberParameter("High Spring", "HS", "The local mean high water spring tide",
                GH_ParamAccess.item, 1000d);
            pManager[6].Optional = true;
            pManager.AddNumberParameter("Low Spring", "LS", "The local mean low water neap tide", GH_ParamAccess.item,
                400d);
            pManager[7].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Sea Level", "SL", "The simulated mean sea level", GH_ParamAccess.item);
            pManager.AddSurfaceParameter("Storm Surge Level", "SS", "The simulated mean storm surge level",
                GH_ParamAccess.item);
            pManager.AddSurfaceParameter("High Neap", "HN", "The simulated mean high water neap level",
                GH_ParamAccess.item);
            pManager.AddSurfaceParameter("Low Neap", "LN", "The simulated mean low water neap level",
                GH_ParamAccess.item);
            pManager.AddSurfaceParameter("High Spring", "HS", "The simulated mean high water spring level",
                GH_ParamAccess.item);
            pManager.AddSurfaceParameter("Low Spring", "LS", "The simulated mean low water spring level",
                GH_ParamAccess.item);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            // Create holder variables for output parameters
            double DATUM, YEAR, RISE, SURGE;
            DATUM = YEAR = RISE = SURGE = 0;
            double HIGH_NEAP, LOW_NEAP, HIGH_SPRING, LOW_SPRING;
            HIGH_NEAP = LOW_NEAP = HIGH_SPRING = LOW_SPRING = 0;

            // Access and extract data from the input parameters individually
            if (!DA.GetData(0, ref DATUM)) return;
            if (!DA.GetData(1, ref YEAR)) return;
            if (!DA.GetData(2, ref RISE)) return;
            if (!DA.GetData(3, ref SURGE)) return;
            if (!DA.GetData(4, ref HIGH_NEAP)) return;
            if (!DA.GetData(5, ref LOW_NEAP)) return;
            if (!DA.GetData(6, ref HIGH_SPRING)) return;
            if (!DA.GetData(7, ref LOW_SPRING)) return;

            // TODO: determine if I need to do input validation here

            // Create holder variables for output parameters
            var seaLevel = this.CreateLevel(CalculateValue(YEAR, DATUM, RISE, 0));
            var stormSurgeLevel = this.CreateLevel(CalculateValue(YEAR, DATUM, RISE, SURGE));
            var highNeapLevel = this.CreateLevel(CalculateValue(YEAR, DATUM, RISE, HIGH_NEAP));
            var lowNeapLevel = this.CreateLevel(CalculateValue(YEAR, DATUM, RISE, LOW_NEAP));
            var highSpringLevel = this.CreateLevel(CalculateValue(YEAR, DATUM, RISE, HIGH_SPRING));
            var lowSpringLevel = this.CreateLevel(CalculateValue(YEAR, DATUM, RISE, LOW_SPRING));

            // Assign variables to output parameters
            DA.SetData(0, seaLevel);
            DA.SetData(1, stormSurgeLevel);
            DA.SetData(2, highNeapLevel);
            DA.SetData(3, lowNeapLevel);
            DA.SetData(4, highSpringLevel);
            DA.SetData(5, lowSpringLevel);
        }

        private double CalculateValue(double simulatedYear, double datum, double rise, double eventHeight)
        {
            // TODO: test with negative years; fractional years...
            var yearsElapased = simulatedYear - DateTime.Now.Year;
            var simulatedLevel = datum + eventHeight + (rise * yearsElapased);
            return simulatedLevel;
        }

        private PlaneSurface CreateLevel(double eventHeight)
        {
            // TODO: fix this
            var origin = new Point3d(0, 0, eventHeight);
            var xExtent = new Point3d(100, 0, eventHeight);
            var yExtent = new Point3d(0, 100, eventHeight);
            var plane = new Plane(origin, xExtent, yExtent);

            var planedSurface = new PlaneSurface(plane,
                new Interval(0, origin.DistanceTo(xExtent)),
                new Interval(0, origin.DistanceTo(yExtent)));

            return planedSurface;
        }
    }
}