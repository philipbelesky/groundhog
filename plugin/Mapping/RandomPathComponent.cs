﻿namespace Groundhog
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Grasshopper.Kernel;
    using Groundhog.Properties;
    using Rhino.Geometry;

    public class RandomPathComponent : GroundHogComponent
    {
        private static Predicate<double> isZero = IsZero;

        public RandomPathComponent()
            : base("Random Path", "RandPath", "Calculates a random path (in 2 dimensions)", "Groundhog", "Mapping")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{01610ad1-ef34-42d3-b2c2-d218aead143e}");

        protected override Bitmap Icon => Resources.icon_path_random;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter(
                "Start", "P", "The initial starting point or points for the path(s)", GH_ParamAccess.list);
            pManager.AddNumberParameter(
                "Step Size", "L", "The distance to move forward for each step. If provided as a list a random option will be selected for each step.", GH_ParamAccess.list);
            pManager.AddIntegerParameter(
                "Step Count", "C", "The number of steps to take.", GH_ParamAccess.item);
            pManager.AddIntegerParameter(
                "Random Seed", "S", "The random seed to be used for each of the path distance and angle choices. If set the same random results will be produced; if not set they will be different for each run.", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddNumberParameter(
                "Directions", "D", "The possible angles in which to move (as a list of numbers in degrees). If not set a random direction in a 360 degree range will be used.", GH_ParamAccess.list);
            pManager[4].Optional = true;
            pManager.AddCurveParameter(
                "Boundary", "B",  "A boundary (must be a closed planar curve) that the steps will not be allowed to cross", GH_ParamAccess.item);
            pManager[5].Optional = true;
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
            var STEP_COUNT = 0;
            int? SEED = null;
            var DIRECTIONS = new List<double>();
            Curve BOUNDARY = null;

            // Access and extract data from the input parameters individually
            if (!DA.GetDataList(0, PATH_ORIGINS)) return;
            if (!DA.GetDataList(1, STEP_SIZES)) return;
            DA.GetData(2, ref STEP_COUNT);
            DA.GetData(3, ref SEED);
            DA.GetDataList(4, DIRECTIONS);
            DA.GetData(5, ref BOUNDARY);

            // Input validation
            if (DIRECTIONS.Count == 0)
                for (var degree = 0; degree < 360; degree++)
                    DIRECTIONS.Add(degree);
            if (STEP_SIZES.Count == 0)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "At least one step size must be provided.");
                return;
            }

            var negativeIndex = STEP_SIZES.FindIndex(IsZero);
            if (negativeIndex != -1)
            {
                this.AddRuntimeMessage(
                    GH_RuntimeMessageLevel.Error, string.Format("Distances cannot be zero. At least one zero-value found at index {0}.", negativeIndex));
                return;
            }

            if (STEP_COUNT == 0)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The amount of steps must be greater than 0.");
                return;
            }

            if (BOUNDARY != null)
            {
                if (BOUNDARY.IsClosed == false)
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The boundary curve must be closed.");
                    return;
                }

                if (BOUNDARY.IsPlanar() == false)
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The boundary curve must be planar.");
                    return;
                }

                for (var i = 0; i < PATH_ORIGINS.Count; i++)
                    if (BOUNDARY.Contains(PATH_ORIGINS[i], Plane.WorldXY, this.docUnitTolerance) == PointContainment.Outside)
                    {
                        this.AddRuntimeMessage(
                            GH_RuntimeMessageLevel.Warning, "If using a boundary curve all points must start within in.");
                        return;
                    }
            }

            // Convert directions to radians
            for (var i = 0; i < DIRECTIONS.Count; i++) DIRECTIONS[i] = Math.PI * DIRECTIONS[i] / 180.0;

            // Create random generate just once
            var rnd = new Random();
            if (SEED.HasValue)
                rnd = new Random(SEED.Value); // Use SEED if it has been provided

            // Calculate walks
            var outputPaths = new List<Curve>();
            for (var i = 0; i < PATH_ORIGINS.Count; i++)
                outputPaths.Add(this.DispatchRandomPaths(PATH_ORIGINS[i], DIRECTIONS, STEP_SIZES,
                    STEP_COUNT, BOUNDARY, rnd));

            // Assign variables to output parameters
            DA.SetDataList(0, outputPaths);
        }

        private static bool IsZero(double number)
        {
            return number == 0;
        }

        private PolylineCurve DispatchRandomPaths(Point3d startPoint, List<double> directions,
            List<double> stepSizes, int stepCount, Curve boundary, Random rnd)
        {
            var stepPoints = new List<Point3d> { startPoint };

            // Loop over the steps and make the movement
            var attempts = 0; // Safety check for when its impractical to find a solution that can not avoid boundary
            for (var step = 1; step < stepCount; step++)
            {
                if (attempts >= 1000)
                {
                    this.AddRuntimeMessage(
                        GH_RuntimeMessageLevel.Remark, "One path was unable to find a movement that could not avoid the boundary so it ended before taking all the specified steps.");
                    break;
                }

                while (attempts < 1000)
                {
                    var stepPoint = this.GetNextStepPoint(stepPoints[step - 1], directions, stepSizes, rnd);
                    attempts++;
                    if (boundary == null || boundary.Contains(stepPoint, Plane.WorldXY, this.docUnitTolerance) !=
                        PointContainment.Outside)
                    {
                        stepPoints.Add(stepPoint);
                        break;
                    }
                }
            }

            var path = new PolylineCurve(stepPoints);
            return path;
        }

        private Point3d GetNextStepPoint(
            Point3d startPoint, List<double> directions, List<double> stepSizes, Random rnd)
        {
            var angle = directions[rnd.Next(0, directions.Count)];
            var distance = stepSizes[rnd.Next(0, stepSizes.Count)];
            // Create vector from angle
            var vectorDirection = new Vector3d(Math.Cos(angle), Math.Sin(angle), 0);
            vectorDirection.Unitize();
            // Create new point by moving the old one
            return Point3d.Add(startPoint, vectorDirection * distance);
        }
    }
}