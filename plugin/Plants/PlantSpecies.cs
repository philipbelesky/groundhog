using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace groundhog
{
    public class PlantSpecies : GH_Param<IGH_Goo>
    {
        // Aesthetics
        private readonly int displayR, displayG, displayB;

        // Form
        private readonly double initialCrownRadius, matureCrownRadius, varianceCrownRadius;

        private readonly double initialHeight, matureHeight, varianceHeight;
        private readonly double initialRootRadius, matureRootRadius, varianceRootRadius;

        private readonly double initialTrunkRadius, matureTrunkRadius, varianceTrunkRadius;

        // Placement
        private readonly double requiredSpacingRadius;

        // Naming
        public readonly string speciesName, commonName, indigenousName;

        // Lifespan
        private readonly double timetoMaturity;

        // Init
        public PlantSpecies(
            string speciesName, string commonName, string indigenousName,
            double timetoMaturity,
            double requiredSpacingRadius,
            double initialCrownRadius, double matureCrownRadius, double varianceCrownRadius,
            double initialRootRadius, double matureRootRadius, double varianceRootRadius,
            double initialHeight, double matureHeight, double varianceHeight,
            double initialTrunkRadius, double matureTrunkRadius, double varianceTrunkRadius,
            int displayR, int displayG, int displayB
        )
            : base(new GH_InstanceDescription("Plant param", "P", "TODO:", "Params"))
        {
            // Naming
            this.speciesName = speciesName;
            this.commonName = commonName;
            this.indigenousName = indigenousName;
            // Lifespan
            this.timetoMaturity = timetoMaturity;
            // Placement
            this.requiredSpacingRadius = requiredSpacingRadius;
            // Form
            this.initialCrownRadius = initialCrownRadius;
            this.matureCrownRadius = matureCrownRadius;
            this.varianceCrownRadius = varianceCrownRadius;
            this.initialRootRadius = initialRootRadius;
            this.matureRootRadius = matureRootRadius;
            this.varianceRootRadius = varianceRootRadius;
            this.initialHeight = initialHeight;
            this.matureHeight = matureHeight;
            this.varianceHeight = varianceHeight;
            this.initialTrunkRadius = initialTrunkRadius;
            this.matureTrunkRadius = matureTrunkRadius;
            this.varianceTrunkRadius = varianceTrunkRadius;
            // Aesthetics
            this.displayR = displayR;
            this.displayG = displayG;
            this.displayB = displayB;
        }

        // Get current state
        private double GetGrowth(double initial, double eventual, double time)
        {
            var annualRate = (eventual - initial) / timetoMaturity;
            var grownTime = Math.Min(time, timetoMaturity);
            var grownState = grownTime * annualRate + initial;
            return grownState;
        }

        // Get geometry
        public Circle GetCrownDisc(Point3d location, double time)
        {
            var height = GetGrowth(initialHeight, matureHeight, time);
            var radius = GetGrowth(initialCrownRadius, matureCrownRadius, time);
            var canopyLocation = new Point3d(location.X, location.Y, location.Z + height);
            return new Circle(canopyLocation, radius);
        }

        public Circle GetRootDisc(Point3d location, double time)
        {
            var radius = GetGrowth(initialRootRadius, matureRootRadius, time);
            return new Circle(location, radius);
        }

        public Circle GetTrunkDisc(Point3d location, double time)
        {
            var radius = GetGrowth(initialTrunkRadius, matureTrunkRadius, time);
            return new Circle(location, radius);
        }

        public Circle GetSpacingDisc(Point3d location)
        {
            return new Circle(location, requiredSpacingRadius);
        }

        public Mesh GetCrownMesh(Point3d location, double time, int plantSides)
        {
            var mesh = new Mesh();
            var trunkCircle = GetTrunkDisc(location, time);
            var canopyCircle = GetCrownDisc(location, time);
            var trunkPolygon = Polyline.CreateCircumscribedPolygon(trunkCircle, plantSides);
            var canopyPolygon = Polyline.CreateCircumscribedPolygon(canopyCircle, plantSides * 2);

            mesh.Vertices.AddVertices(trunkPolygon);
            mesh.Vertices.AddVertices(canopyPolygon);
            mesh.Faces.AddFace(new MeshFace(0, plantSides + 1, mesh.Vertices.Count - 2)); // Counter clockwise; pointing in
            mesh.Faces.AddFace(new MeshFace(0, plantSides + 2, plantSides + 1)); // Clockwise; pointing in
            mesh.Faces.AddFace(new MeshFace(0, 1, plantSides + 2)); // Clockwise; pointing out
            for (var i = 1; i < plantSides; i++)
            {
                int baseNodeIndex = (i * 2) + plantSides + 1;
                mesh.Faces.AddFace(new MeshFace(i, baseNodeIndex, baseNodeIndex - 1)); // Counter clockwise; pointing in
                mesh.Faces.AddFace(new MeshFace(i, baseNodeIndex + 1, baseNodeIndex)); // Clockwise; pointing in
                mesh.Faces.AddFace(new MeshFace(i, i + 1, baseNodeIndex + 1)); // Clockwise; pointing out
            }

            mesh.Normals.ComputeNormals();
            mesh.Compact();
            return mesh;
        }

        public Mesh GetRootMesh(Point3d location, double time, int plantSides)
        {
            return null;
        }

        public Color GetColor()
        {
            return Color.FromArgb(displayR, displayG, displayB);
        }

        public GH_String GetLabel()
        {
            return new GH_String(speciesName);
        }

        #region casting

        protected override IGH_Goo InstantiateT()
        {
            return new GH_ObjectWrapper();
        }

        #endregion


        #region properties

        public override GH_Exposure Exposure => GH_Exposure.primary;

        public override Guid ComponentGuid => new Guid("2d268bdc-ecaa-4cf7-815a-c8111d1798d7");

        public override string ToString()
        {
            return "groundhog Plant Species (" + speciesName + ")";
        }

        #endregion
    }
}