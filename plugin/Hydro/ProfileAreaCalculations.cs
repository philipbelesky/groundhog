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
    public class GroundhogProfileAreaCalculations : GroundHogComponent
    {
        public GroundhogProfileAreaCalculations()
            : base("Flow Area", "FArea", "Determine the area of water within a channel given a volume", "Groundhog", "Hydro")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.icon_flows_srf;

        public override Guid ComponentGuid => new Guid("{ffba138d-a959-4dbc-988d-c1ef761b8e79}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Channel", "C", "The sectional curve profile of the channel; must be planar", GH_ParamAccess.item);
            pManager.AddNumberParameter("Area", "A", "The desired area of the flow body", GH_ParamAccess.item);
            pManager.AddNumberParameter("Precision", "T", "The number of units to be accurate to; if unspecified it will use 1% of the area", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Body", "B", "The perimeter(s) of the calculated water body", GH_ParamAccess.list);
            pManager.AddNumberParameter("Area", "A", "The area of the calculated perimeter(s)", GH_ParamAccess.list);
            pManager.AddPointParameter("test", "T", "test", GH_ParamAccess.list);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            // Create holder variables for input parameters
            var CHANNEL_CURVE = default(Curve);
            double AREA_TARGET = 0.0;

            // Access and extract data from the input parameters individually
            if (!DA.GetData(0, ref CHANNEL_CURVE)) return;
            if (!DA.GetData(1, ref AREA_TARGET)) return;
            double AREA_PRECISION = AREA_TARGET * 0.01; // Provide default (will be overwritten if set)
            DA.GetData(2, ref AREA_PRECISION);

            // Validation
            if (CHANNEL_CURVE == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "A null item has been provided as the Channel input; please correct this input.");
                return;
            }
            if (CHANNEL_CURVE.IsPlanar() == false)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "A non-planar curve has been provided as the channel section; please ensure it is planar.");
                return;
            }
            // TODO VALIDATE AREA
            
            var TOLERANCE = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;

            // Get the extremes of the curve
            // TODO: refine the search strategy here; should I be searching from the lowest-Z to the highest-Z at unit intervals rather than params?
            var upper_param = CHANNEL_CURVE.Domain.T1;
            var lower_param = CHANNEL_CURVE.Domain.T0;
            var initial_guess_param = (upper_param - lower_param) * 0.25;

            var output_profiles = new List<Curve>();
            var output_areas = new List<double>();

            // Use bisect method to refine to the approximate area
            double intervalBegin = lower_param;
            double intervalEnd = upper_param * 0.5; // TODO replace with lowest Z? or a unitised halfway point? to test
            double precision = 0.1; // TODO: not hardcode
            double middle;
            Curve test_curve;

            while ((intervalEnd - intervalBegin) > precision)
            {
                // Get curve at test parameter
                middle = (intervalBegin + intervalEnd) / 2;
                test_curve = getWaterAreaAtParameter(middle, CHANNEL_CURVE, TOLERANCE);

                // Calculate its area
                var area_calc = AreaMassProperties.Compute(test_curve);
                double? area = null;
                if (area_calc != null)
                {
                    if (area_calc.Area < 0.0)
                        area = area_calc.Area * -1; // Areas can be negative; same same
                    else
                        area = area_calc.Area;
                }
                else
                {                    
                    break; // TODO handle nulls from AreaMassProperties
                }
                if (!area.HasValue)
                {
                    break; // TODO handle nulls from AreaMassProperties.Area
                }

                // Refine the interval
                if (Math.Abs(AREA_TARGET - area.Value) <= AREA_PRECISION)
                {
                    // SUCCESS
                    output_profiles.Add(test_curve);
                    output_areas.Add(area.Value);
                    break;
                }

                if (AREA_TARGET > area)
                {
                    // Reduce the upper bound as the section is larger than desired
                    intervalEnd = middle;
                }
                else
                {
                    intervalBegin = middle;
                }   
            }
            
            // Assign variables to output parameters
            Console.WriteLine("done");
            DA.SetDataList(0, output_profiles);
            DA.SetDataList(1, output_areas);
            //DA.SetDataList(2, testPoints);
        }

        private Curve getWaterAreaAtParameter(double bankParameter, Curve CHANNEL_CURVE, double TOLERANCE)
        {
            var test_point = CHANNEL_CURVE.PointAt(bankParameter); // Assume somewhat symmetrical so 0.25 is halfway up one side
            var test_plane = new Plane(new Point3d(0, 0, test_point.Z), new Vector3d(0, 0, 1)); // Create an XY plane positioned vertically at the test point

            // Intersect Plane with the Curve to get water level(s)
            var intersections = Rhino.Geometry.Intersect.Intersection.CurvePlane(CHANNEL_CURVE, test_plane, TOLERANCE);

            // TODO: deal with multiple intersections and/or even numbers of intersections

            // Get intersection events on main curve
            var param_range = new Tuple<double, double>(intersections[0].ParameterA, intersections[1].ParameterA);
            var point_range = new Tuple<Point3d, Point3d>(intersections[0].PointA, intersections[1].PointA);
            var sub_curve = CHANNEL_CURVE.Trim(param_range.Item1, param_range.Item2); // Get the sub-curve

            // Make an array of top line and the channel to join then close
            var top_curve = new Line(point_range.Item1, point_range.Item2).ToNurbsCurve(); // Waterline
            Curve[] channel_curve = Curve.JoinCurves(new Curve[] { sub_curve, top_curve }); // Joined Curves

            return channel_curve[0];
        }
    }
}