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
    public class GroundhogChannelRegionComponent : GroundHogComponent
    {
        public GroundhogChannelRegionComponent()
            : base("Channel Region", "CRegion", "Determine the submerged region of a channel given a quantity of water", "Groundhog", "Hydro")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.icon_channel_region;

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
            pManager.AddCurveParameter("Channel", "C", "The perimeter(s) of the calculated water body", GH_ParamAccess.list);
            pManager.AddNumberParameter("Area", "A", "The area of the calculated perimeter(s)", GH_ParamAccess.list);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            // Create holder variables for input parameters
            var TOLERANCE = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            var CHANNEL_CURVE = default(Curve);
            double AREA_TARGET = 0.0;

            // Access and extract data from the input parameters individually
            if (!DA.GetData(0, ref CHANNEL_CURVE)) return;
            if (!DA.GetData(1, ref AREA_TARGET)) return;
            double AREA_PRECISION = AREA_TARGET * 0.01; // Default; overwritten if set
            DA.GetData(2, ref AREA_PRECISION);

            // Validation
            if (CHANNEL_CURVE == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                    "A null item has been provided as the Channel input; please correct this input.");
                return;
            }
            if (CHANNEL_CURVE.IsPlanar(TOLERANCE) == false)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                    "A non-planar curve has been provided as the channel section; please ensure it is planar.");
                return;
            }
            if (AREA_TARGET <= 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The area target must be greater than 0.");
                return;
            }
            if (AREA_PRECISION <= 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The area precision must be greater than 0.");
                return;
            }

            // Get the extremes of the curve
            var upperParam = CHANNEL_CURVE.Domain.T1;
            var lowerParam = CHANNEL_CURVE.Domain.T0;
            var initialGuessParam = (upperParam - lowerParam) * 0.25;

            // Placeholders for outputs
            var outputProfiles = new List<Curve>();
            var outputAreas = new List<double>();

            // Use bisect method to refine to the approximate area
            double intervalBegin = lowerParam;
            double intervalEnd = upperParam * 0.5; // TODO replace with lowest Z? or a unitised halfway point? to test
            double middle;
            double lastArea = 0.0;


            while ((intervalEnd - intervalBegin) > TOLERANCE)
            {
                // Get curve at test parameter
                middle = (intervalBegin + intervalEnd) / 2;

                // Find the actual channel geometries at that parameter
                var testChannels = GetWaterChannelsAtParameter(middle, CHANNEL_CURVE, TOLERANCE);
                if (testChannels == null)
                {
                    break; // No test curve when <2 intersections; i.e. overflown perimeter
                }

                // Calculate its area
                var calculatedAreas = GetAreasForWaterChannels(testChannels);
                var totalArea = calculatedAreas.Sum();
                lastArea = totalArea;

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
                string helpMessage;
                if (lastArea < AREA_TARGET)
                    helpMessage = "decrease";
                else
                    helpMessage = "increase";

                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                    $"Specified area {AREA_TARGET} could not be contained in the profile; the last attempt found an area of {Convert.ToInt32(lastArea)}. You probably need {helpMessage} the area to get a result.");
            }

            // Assign variables to output parameters
            DA.SetDataList(0, outputProfiles);
            DA.SetDataList(1, outputAreas);
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
            // Assume somewhat symmetrical so 0.25 is halfway up one side
            var test_point = CHANNEL_CURVE.PointAt(bankParameter);

            // Create an XY plane positioned vertically at the test point
            var test_plane = new Plane(new Point3d(0, 0, test_point.Z), new Vector3d(0, 0, 1));

            // Intersect Plane with the Curve to get water level(s)
            var intersections = Rhino.Geometry.Intersect.Intersection.CurvePlane(CHANNEL_CURVE, test_plane, TOLERANCE);
            if (intersections == null)
                return null;
            if (intersections.Count < 2)
                return null; // One or fewer intersections is not solvable

            var validIntersections = new List<Tuple<Point3d, double>>();
            // For each intersection event we check evaluate the curve slightly ahead and check whether the 
            // Z-position is down or up. A higher Z is not allowable as a starting point is a valid end
            for (var i = 0; i < intersections.Count; i = i + 1)
            {
                var currentCurvePoint = intersections[i].PointA;
                var currentCurveParameter = intersections[i].ParameterA;
                var pointFurtherAlongCurve = CHANNEL_CURVE.PointAt(currentCurveParameter + 0.01);
                if (pointFurtherAlongCurve.Z <= currentCurvePoint.Z)
                {
                    validIntersections.Add(Tuple.Create(currentCurvePoint, currentCurveParameter));
                    // Also add the next valid point along and skip processing it
                    i += 1;
                    if (i >= intersections.Count)
                    {
                        break; // Only add to the list when there is another point to pair with
                    }
                    validIntersections.Add(Tuple.Create(intersections[i].PointA, intersections[i].ParameterA));
                }
            }

            var channelCurves = new List<Curve>();
            // Loop over the valid interesections pairs and make them into bounded sub-curves (ala channels)
            for (var i = 0; i < validIntersections.Count; i = i + 2)
            {
                if (i + 1 >= validIntersections.Count)
                {
                    break; // Only add to the list when there is another point to pair with
                }
                var ixA = validIntersections[i];
                var ixB = validIntersections[i + 1];

                // Make an array of top water line and the sub channel then join to close
                var wettedLine = CHANNEL_CURVE.Trim(ixA.Item2, ixB.Item2); // Get the sub-curve of the channel
                var waterLine = new Line(ixA.Item1, ixB.Item1).ToNurbsCurve();

                Curve[] channel = Curve.JoinCurves(new Curve[] { wettedLine, waterLine });
                if (channel.Length > 0)
                    channelCurves.Add(channel[0]);
            }
            return channelCurves;
        }
    }
}