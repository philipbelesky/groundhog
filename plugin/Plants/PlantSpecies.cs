using System;
using System.Drawing;
using GH_IO;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using groundhog;
using Rhino.Geometry;

namespace groundhog
{
    public interface Plant_IGH_Goo : IGH_Goo
    {

    }
    
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

        // Random seeds for variances (these are set during showers; not during init else they aren't unique 
        // As data is usually repeated in Grasshopper so object values are duplicated
        private double crownVarianceMultiplier,
            heightVarianceMultiplier,
            rootVarianceMultiplier,
            trunkVarianceMultiplier;
        
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

        public void SetVarianceValues(int randomSeed) // Rand can't be generated here as its time dependent = same values
        {
            // Seeds are index of plant in list order so that they are deterministic (when the time value changes)
            var rand = new Random(randomSeed); 
            crownVarianceMultiplier = GetVarianceMultiplier(rand, varianceCrownRadius);
            heightVarianceMultiplier = GetVarianceMultiplier(rand, varianceHeight);
            rootVarianceMultiplier = GetVarianceMultiplier(rand, varianceRootRadius);
            trunkVarianceMultiplier = GetVarianceMultiplier(rand, varianceTrunkRadius);
        }

        private double GetVarianceMultiplier(Random rand, double varianceValue)
        {
            var multiplier = rand.NextDouble() * varianceValue; // E.g. 0.5 * 20% = 10
            if (rand.Next(2) > 0) return 1 + multiplier / 100; // E.g. 1 + (10 / 100) = 1.1
            return 1 - multiplier / 100; // E.g. 1 - (10 / 100) = 0.9
        }

        // Get current state
        private double GetGrowth(double initialState, double eventualState, double time, double varianceMultiplier)
        {
            var variedEndState = varianceMultiplier * eventualState;
            var annualRate = (variedEndState - initialState) / timetoMaturity;
            var grownTime = Math.Min(time, timetoMaturity);
            var grownState = grownTime * annualRate + initialState;
            return grownState;
        }

        // Get geometry
        public Circle GetCrownDisc(Point3d location, double time)
        {
            var height = GetGrowth(initialHeight, matureHeight, time, heightVarianceMultiplier);
            var radius = GetGrowth(initialCrownRadius, matureCrownRadius, time, crownVarianceMultiplier);
            var canopyLocation = new Point3d(location.X, location.Y, location.Z + height);
            return new Circle(canopyLocation, radius);
        }

        public Circle GetRootDisc(Point3d location, double time)
        {
            var radius = GetGrowth(initialRootRadius, matureRootRadius, time, rootVarianceMultiplier);
            return new Circle(location, radius);
        }

        public Circle GetTrunkDisc(Point3d location, double time)
        {
            var radius = GetGrowth(initialTrunkRadius, matureTrunkRadius, time, trunkVarianceMultiplier);
            return new Circle(location, radius);
        }

        public Circle GetSpacingDisc(Point3d location)
        {
            return new Circle(location, requiredSpacingRadius);
        }

        public Mesh GetCrownMesh(Point3d location, double time, int plantSides)
        {
            return makeMeshForAttribute(GetCrownDisc(location, time), GetTrunkDisc(location, time), plantSides);
        }

        public Mesh GetRootMesh(Point3d location, double time, int plantSides)
        {
            var rootBallBottomDisc = GetTrunkDisc(location, time);
            var rootDepth =
                GetGrowth(initialRootRadius, matureRootRadius, time,
                    rootVarianceMultiplier); // Assume approx spherical 
            rootBallBottomDisc.Translate(new Vector3d(0, 0, rootDepth * -1));
            return makeMeshForAttribute(GetRootDisc(location, time), rootBallBottomDisc, plantSides);
        }

        private Mesh makeMeshForAttribute(Circle topCircumference, Circle bottomCircumference, int plantSides)
        {
            var mesh = new Mesh();
            var topPolygon = Polyline.CreateCircumscribedPolygon(topCircumference, plantSides * 2);
            var bottomPolygon = Polyline.CreateCircumscribedPolygon(bottomCircumference, plantSides);

            mesh.Vertices.AddVertices(bottomPolygon);
            mesh.Vertices.AddVertices(topPolygon);
            mesh.Vertices.Add(topPolygon.CenterPoint());

            //// Build the edges of the canopy mesh 
            mesh.Faces.AddFace(new MeshFace(0, plantSides + 1,
                mesh.Vertices.Count - 3)); // Counter clockwise; pointing in
            mesh.Faces.AddFace(new MeshFace(0, plantSides + 2, plantSides + 1)); // Clockwise; pointing in
            mesh.Faces.AddFace(new MeshFace(0, 1, plantSides + 2)); // Clockwise; pointing out
            for (var i = 1; i < plantSides; i++)
            {
                var baseNodeIndex = i * 2 + plantSides + 1;
                mesh.Faces.AddFace(new MeshFace(i, baseNodeIndex, baseNodeIndex - 1)); // Counter clockwise; pointing in
                mesh.Faces.AddFace(new MeshFace(i, baseNodeIndex + 1, baseNodeIndex)); // Clockwise; pointing in
                mesh.Faces.AddFace(new MeshFace(i, i + 1, baseNodeIndex + 1)); // Clockwise; pointing out
            }

            // Build the cap of the canopy mesh
            for (var i = plantSides + 1; i < mesh.Vertices.Count - 2; i++)
                mesh.Faces.AddFace(new MeshFace(mesh.Vertices.Count - 1, i, i + 1));

            mesh.Normals.ComputeNormals();
            mesh.Compact();
            return mesh;
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