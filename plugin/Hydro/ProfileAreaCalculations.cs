using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
            pManager.AddGeometryParameter("test", "T", "test", GH_ParamAccess.list);
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

            var test = new List<Curve>(); // DEBUG

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
            var upperParam = CHANNEL_CURVE.Domain.T1;
            var lowerParam = CHANNEL_CURVE.Domain.T0;
            var initialGuessParam = (upperParam - lowerParam) * 0.25;

            var outputProfiles = new List<Curve>();
            var outputAreas = new List<double>();

            // Use bisect method to refine to the approximate area
            double intervalBegin = lowerParam;
            double intervalEnd = upperParam * 0.5; // TODO replace with lowest Z? or a unitised halfway point? to test
            double precision = 0.01; // TODO: not hardcode
            double middle;

            while ((intervalEnd - intervalBegin) > precision)
            {
                // Get curve at test parameter
                middle = (intervalBegin + intervalEnd) / 2;

                // Find the actual channel geometries at that parameter
                var testChannels = GetWaterChannelsAtParameter(middle, CHANNEL_CURVE, TOLERANCE);
                if (testChannels == null)
                {
                    break; // No test curve when <2 intersections; i.e. overflown perimeter
                }

                test.AddRange(testChannels); // DEBUG

                // Calculate its area
                var calculatedAreas = GetAreasForWaterChannels(testChannels);
                var totalArea = calculatedAreas.Sum();

                // Refine the interval
                if (Math.Abs(AREA_TARGET - totalArea) <= AREA_PRECISION)
                {
                    // SUCCESS
                    outputProfiles.AddRange(testChannels);
                    outputAreas.AddRange(calculatedAreas);
                    break;
                }

                if (AREA_TARGET > totalArea)
                    intervalEnd = middle; // Reduce the upper bound as the sectional area is larger than desired
                else
                    intervalBegin = middle; // Reduce the lower bound as the sectional area is smaller than desired
            }

            if (!outputProfiles.Any())
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Specified area could not be contained in the profile; you probably need to reduce the area to get a result.");
            }

            // Assign variables to output parameters
            Console.WriteLine("done");
            DA.SetDataList(0, outputProfiles);
            DA.SetDataList(1, outputAreas);
            DA.SetDataList(2, test);
        }

        private List<double> GetAreasForWaterChannels(List<Curve> channels)
        {
            var areas = new List<double>();
            for (var i = 0; i < channels.Count; i = i + 1)
            {
                var area_calc = AreaMassProperties.Compute(channels[i]);
                if (area_calc != null)
                {
                    areas.Add(Math.Abs(area_calc.Area));
                }
            }
            return areas;
        }

        private List<Curve> GetWaterChannelsAtParameter(double bankParameter, Curve CHANNEL_CURVE, double TOLERANCE)
        {
            var test_point = CHANNEL_CURVE.PointAt(bankParameter); // Assume somewhat symmetrical so 0.25 is halfway up one side
            var test_plane = new Plane(new Point3d(0, 0, test_point.Z), new Vector3d(0, 0, 1)); // Create an XY plane positioned vertically at the test point
            var channelCurves = new List<Curve>();

            // Intersect Plane with the Curve to get water level(s)
            var intersections = Rhino.Geometry.Intersect.Intersection.CurvePlane(CHANNEL_CURVE, test_plane, TOLERANCE);
            if (intersections == null)
                return null;
            if (intersections.Count < 2)
                return null;

            for (var i = 0; i < Math.Floor(intersections.Count / 2.0); i = i + 2)
            {
                var ixA = intersections[i];
                var ixB = intersections[i + 1];

                // Make an array of top water line and the sub channel then join to close
                var wettedLine = CHANNEL_CURVE.Trim(ixA.ParameterA, ixB.ParameterA); // Get the sub-curve of the channel
                var waterLine = new Line(ixA.PointB, ixB.PointB).ToNurbsCurve();

                Curve[] channel = Curve.JoinCurves(new Curve[] { wettedLine, waterLine });                
                if (channel.Length > 0)
                    channelCurves.Add(channel[0]);
            }
            return channelCurves;
        }
    }
}