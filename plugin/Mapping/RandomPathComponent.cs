using System;
using System.Collections.Generic;
using System.Drawing;
using groundhog.Properties;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using ShortestWalk.Gh;
using ShortestWalk.Geometry;

// Component source lightly modified from Giulio Piacentino's repo, see License/Attribution in /ShortestWalk/

namespace groundhog
{
    public class GroundhogRandomPathComponent : GroundHogComponent
    {
        public GroundhogRandomPathComponent()
            : base("Random Path", "RandPath", "Calculates a random path (in 2 dimensions)",
                   "Groundhog", "Mapping")
        { 
        }

        protected override Bitmap Icon => Resources.icon_random_path;

        public override Guid ComponentGuid => new Guid("{01610ad1-ef34-42d3-b2c2-d218aead143e}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Start", "S", "The initial starting point of the paths", GH_ParamAccess.list);
            pManager.AddNumberParameter("Directions", "D", "The possible angles in which to move (as a list of numbers in degrees)", GH_ParamAccess.list);
            pManager.AddNumberParameter("Step Size", "L", "The step size to move forward between each of the steps", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Step Count", "C", "The number of steps to take", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Path", "P", "The resulting random path", GH_ParamAccess.list);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            // Create holder variables for input parameters
            var PATH_ORIGINS = new List<Point3d>();
            var DIRECTIONS = new List<double>();
            int STEP_SIZE = 0;
            int STEP_COUNT = 0;
            
            // Access and extract data from the input parameters individually
            if (!DA.GetDataList(0, PATH_ORIGINS)) return;
            if (!DA.GetData(1, ref DIRECTIONS)) return;
            if (!DA.GetData(2, ref STEP_SIZE)) return;
            if (!DA.GetData(3, ref STEP_COUNT)) return;

            // Input validation
            if (DIRECTIONS.Count == 0)
            {
                for (int degree = 0; degree < 360; degree++)
                {
                    DIRECTIONS.Add(degree);
                }
            }
            if (STEP_SIZE == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The step size must be greater than 0");
                return;
            }
            if (STEP_COUNT == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The amount of steps must be greater than 0");
                return;
            }

            // Convert directions to radians
            for (int i = 0; i < DIRECTIONS.Count; i++)
            {
                DIRECTIONS[i] = Math.PI * DIRECTIONS[i] / 180.0;
            }


            Random rnd = new Random();

            // Do output
            // Assign variables to output parameters
            var outputPaths = new List<Curve>();
            DA.SetData(0, outputPaths);
        }

        private Polyline DispatchFlowPoints(Point3d startPoint, List<double> directions,
                                            double STEP_SIZE, int STEP_COUNT, Random rnd)
        {
            var stepPoints = new Point3d[STEP_COUNT + 1];
            stepPoints[0] = startPoint;

            for (int step = 0; step < STEP_COUNT; step++)
            {
                var direction = directions[rnd.Next(0, directions.Count - 1)];
                var vectorDirection = new Vector2d((double)Math.Cos(direction), (double)Math.Sin(direction));
                // multiply vector
                // move point; add to array
            }

            return new Polyline(stepPoints);
        }
    }
}
