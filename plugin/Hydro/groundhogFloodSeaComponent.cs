using System;
using System.Drawing;
using groundhog.Properties;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace groundhog
{
    public class groundhogSeaFloodComponent : GH_Component
    {
        /// <summary>
        ///     Each implementation of GH_Component must provide a public
        ///     constructor without any arguments.
        ///     Category represents the Tab in which the component will appear,
        ///     Subcategory the panel. If you use non-existing tab or panel names,
        ///     new tabs/panels will automatically be created.
        /// </summary>
        public groundhogSeaFloodComponent()
            : base("Sea Flood Simulator", "Sea Floods",
                "Examine flooding levels along a surface from a tidal source",
                "Groundhog", "Hydro")
        {
        }

        /// <summary>
        ///     The Exposure property controls where in the panel a component icon
        ///     will appear. There are seven possible locations (primary to septenary),
        ///     each of which can be combined with the GH_Exposure.obscure flag, which
        ///     ensures the component will only be visible on panel dropdowns.
        /// </summary>
        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        ///     Provides an Icon for every component that will be visible in the User Interface.
        ///     Icons need to be 24x24 pixels.
        /// </summary>
        protected override Bitmap Icon => Resources.icon_floods_sea;

        /// <summary>
        ///     Each component must have a unique Guid to identify it.
        ///     It is vital this Guid doesn't change otherwise old ghx files
        ///     that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{2d234bdc-ecaa-4cf7-815a-c8111d1798d0}");

        /// <summary>
        ///     Registers all the input parameters for this component.
        /// </summary>
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

        /// <summary>
        ///     Registers all the output parameters for this component.
        /// </summary>
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

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            //pManager.HideParameter(0);
        }

        /// <summary>
        ///     This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">
        ///     The DA object can be used to retrieve data from input parameters and
        ///     to store data in output parameters.
        /// </param>
        protected override void SolveInstance(IGH_DataAccess DA)
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
            var seaLevel = CreateLevel(CalculateValue(YEAR, DATUM, RISE, 0));
            var stormSurgeLevel = CreateLevel(CalculateValue(YEAR, DATUM, RISE, SURGE));
            var highNeapLevel = CreateLevel(CalculateValue(YEAR, DATUM, RISE, HIGH_NEAP));
            var lowNeapLevel = CreateLevel(CalculateValue(YEAR, DATUM, RISE, LOW_NEAP));
            var highSpringLevel = CreateLevel(CalculateValue(YEAR, DATUM, RISE, HIGH_SPRING));
            var lowSpringLevel = CreateLevel(CalculateValue(YEAR, DATUM, RISE, LOW_SPRING));

            // Assign variables to output parameters
            DA.SetData(0, seaLevel);
            DA.SetData(1, stormSurgeLevel);
            DA.SetData(2, highNeapLevel);
            DA.SetData(3, lowNeapLevel);
            DA.SetData(4, highSpringLevel);
            DA.SetData(5, lowSpringLevel);
        }

        private double CalculateValue(double SIMULATED_YEAR, double DATUM, double RISE, double EVENT_HEIGHT)
        {
            // TODO: test with negative years; fractional years...
            var yearsElapased = SIMULATED_YEAR - DateTime.Now.Year;
            var simulatedLevel = DATUM + EVENT_HEIGHT + Math.Pow(RISE, yearsElapased);
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
                new Interval(0, origin.DistanceTo(yExtent))
            );

            return planedSurface;
        }
    }
}