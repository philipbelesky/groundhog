using System;
using System.Collections.Generic;
using System.Drawing;
using groundhog.Properties;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino;
using Rhino.Geometry;

namespace groundhog
{
    public class GroundhogRandomPathComponent : GroundHogComponent
    {
        public GroundhogRandomPathComponent()
            : base("Random Path", "RandPath", "Calculates a random path (in 2 dimensions)",
                   "Groundhog", "Mapping")
        { 
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.icon_random_path;

        public override Guid ComponentGuid => new Guid("{01610ad1-ef34-42d3-b2c2-d218aead143e}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Start", "S", "The initial starting point or points for the path(s)", GH_ParamAccess.list);
            pManager.AddNumberParameter("Step Size", "L", "The distance to move forward for each step. If provided as a list a random option will be selected for each step.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Step Count", "C", "The number of steps to take.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Directions", "D", "The possible angles in which to move (as a list of numbers in degrees). If not set a random direction in a 360 degree range will be used.", GH_ParamAccess.list);
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Path", "P", "The resulting random path", GH_ParamAccess.list);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            // Create holder variables for input parameters
            var PATH_ORIGINS = new List<Point3d>();
            var STEP_SIZES = new List<double>();
            int STEP_COUNT = 0;
            var DIRECTIONS = new List<double>();
            
            // Access and extract data from the input parameters individually
            if (!DA.GetDataList(0, PATH_ORIGINS)) return;
            if (!DA.GetDataList(1, STEP_SIZES)) return;
            DA.GetData(2, ref STEP_COUNT);
            DA.GetDataList(3, DIRECTIONS);

            // Input validation
            if (DIRECTIONS.Count == 0)
            {
                for (int degree = 0; degree < 360; degree++)
                {
                    DIRECTIONS.Add(degree);
                }
            }
            if (STEP_SIZES.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "At least one step size must be provided.");
                return;
            }
            int negativeIndex = STEP_SIZES.FindIndex(isZero);
            if (negativeIndex != -1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
                                  string.Format("Distances cannot be zero. At least one zero-value found at index {0}.", negativeIndex));
                return;
            }
            if (STEP_COUNT == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The amount of steps must be greater than 0.");
                return;
            }

            // Convert directions to radians
            for (int i = 0; i < DIRECTIONS.Count; i++)
            {
                DIRECTIONS[i] = Math.PI * DIRECTIONS[i] / 180.0;
            }

            // Create random generate just once
            Random rnd = new Random();

            // Calculate walks
            var outputPaths = new List<Curve>();
            for (int i = 0; i < PATH_ORIGINS.Count; i++)
            {
                outputPaths.Add(DispatchRandomPaths(PATH_ORIGINS[i], DIRECTIONS, STEP_SIZES, STEP_COUNT, rnd));
            }
            
            // Assign variables to output parameters
            DA.SetDataList(0, outputPaths);
        }

        private PolylineCurve DispatchRandomPaths(Point3d startPoint, List<double> DIRECTIONS,
                                          List<double> STEP_SIZES, int STEP_COUNT, Random rnd)
        {
            var stepPoints = new Point3d[STEP_COUNT];
            stepPoints[0] = startPoint;
            // Loop over the steps and make the movement
            for (int step = 1; step < STEP_COUNT; step++)
            {
                var testA = rnd.Next(0, DIRECTIONS.Count - 1);
                var testD = rnd.Next(0, STEP_SIZES.Count - 1);
                var angle = DIRECTIONS[rnd.Next(0, DIRECTIONS.Count - 1)];
                var distance = STEP_SIZES[rnd.Next(0, STEP_SIZES.Count - 1)];
                // Create vector from angle
                var vectorDirection = new Vector3d((double)Math.Cos(angle), (double)Math.Sin(angle), 0);
                vectorDirection.Unitize();
                // Create new point by moving the old one
                stepPoints[step] = Point3d.Add(stepPoints[step - 1], vectorDirection * distance); 
            }
            var path = new PolylineCurve(stepPoints);
            return path;
        }

        static Predicate<double> _isZero = isZero;

        private static bool isZero(double number)
        {
            return number == 0;
        }
    }
}
