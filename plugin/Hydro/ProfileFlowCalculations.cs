using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using groundhog.Properties;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino;
using Rhino.Geometry;

namespace groundhog
{
    public class GroundhogProfileFlowCalculations : GroundHogComponent
    {
        public GroundhogProfileFlowCalculations()
            : base("Flow Profile", "FProfile", "Calculate information about water flow in a given sectional area", "Groundhog", "Hydro")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.icon_flows_srf;

        public override Guid ComponentGuid => new Guid("{008255a6-edff-44d9-b96f-23eb050b4a1a}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Body", "B", "A closed planar curve representing a section of the water body; assumes a level top", GH_ParamAccess.item);
            pManager.AddNumberParameter("Gauckler–Manning", "GM", "The Gauckler–Manning coefficient", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("slope", "S", "Slope of the hydraulic grade line", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Max Depth", "X", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Mean Depth", "M", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Wetted Perimeter", "W", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Hydralic Radius", "H", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Velocity", "V", "", GH_ParamAccess.item);
            pManager.AddNumberParameter("Discharge", "D", "The rate of discharge in cubic document units per second", GH_ParamAccess.item);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            // Create holder variables for input parameters
            var WATER_BODY = default(Curve);
            double VELOCITY = 0.0;

            // Access and extract data from the input parameters individually
            DA.GetData(0, ref WATER_BODY);
            DA.GetData(2, ref VELOCITY);

            // Validation
            if (VELOCITY == 0.0)
            {
                // TODO: dont calculate this
            }

            // Calculate area
            var area_calc = AreaMassProperties.Compute(WATER_BODY);
            double? area = null;
            if (area_calc != null)
            {
                if (area_calc.Area < 0.0)
                    area = area_calc.Area * -1; // Areas can be negative; same same
                else
                    area = area_calc.Area;
            }

            // Mean Depth
            // TODO cross-sectional area divided by the surface width

            // Max Depth
            // TODO: Lowest point on the curve; explode the curve to get vertices then sort by lowest/highest Z 
            
            // Wetted Perimeter
            double wettedPerimeter;
            // TODO explode the curve; find the top most section then measure the length of the lower areas
            // TODO: check how this works on polylines; maybe need to not presume the curve has segments; or to search them
            
            // Hydraulic Radius
            // TODO this is the area divided by the wetted perimeter

            // Velocity
            // TODO this is derived from the manning formula
            
        }
    }
}