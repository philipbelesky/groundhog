using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using groundhog.Properties;
using Rhino.Geometry;

namespace groundhog
{
    public class GroundhogMeshSlopeComponent : GroundHogComponent
    {
        public GroundhogMeshSlopeComponent()
            : base("Mesh Slope", "Slope",
                "Analyses the slope of a Mesh, outputting separated faces for coloring and the slope/grade",
                "Groundhog", "Terrain")
        {
        }

        protected override Bitmap Icon => Resources.icon_mesh_slope;

        public override Guid ComponentGuid => new Guid("{c3b67aca-0e15-4279-9d6c-96cce97fcb47}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "The terrain mesh", GH_ParamAccess.item);
            pManager[0].Optional = false;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh Faces", "F", "The sub mesh faces (for coloring)", GH_ParamAccess.list);
            pManager.AddPointParameter("Face Centers", "C", "The centers of each mesh face (for vector previews)",
                GH_ParamAccess.list);
            pManager.AddVectorParameter("Face Vectors", "V", "The direction to the lowest points of each face",
                GH_ParamAccess.list);
            pManager.AddNumberParameter("Face Slopes °", "A", "The slope of each mesh face, as the angle of inline",
                GH_ParamAccess.list);
            pManager.AddNumberParameter("Face Slopes %", "P", "The slope of each mesh face, as a percentage",
                GH_ParamAccess.list);
            pManager.AddNumberParameter("Face Slopes :", "P",
                "The slope of each mesh face, as the denominator of a ratio (i.e. 1:x)", GH_ParamAccess.list);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            var MESH = default(Mesh);
            // Access and extract data from the input parameters individually
            if (!DA.GetData(0, ref MESH)) return;

            var subMeshes = TerrainCalculations.Explode(MESH);
            var subCentres = TerrainCalculations.GetCenters(MESH);
            var subDirections = TerrainCalculations.GetDirections(subMeshes, subCentres);
            var subAngles = GetAngles(MESH);

            // Calculate ratios from angles
            var subPercentiles = new List<double>();
            foreach (var angle in subAngles)
            {
                var radians = Math.PI * angle / 180.0;
                subPercentiles.Add(Math.Tan(radians) * 100);
            }

            // Calculate ratio from percentiles
            var subRatioDenominators = new List<double>();
            foreach (var percentage in subPercentiles) subRatioDenominators.Add(100 / percentage);

            // Assign variables to output parameters
            DA.SetDataList(0, subMeshes);
            DA.SetDataList(1, subCentres);
            DA.SetDataList(2, subDirections);
            DA.SetDataList(3, subAngles);
            DA.SetDataList(4, subPercentiles);
            DA.SetDataList(5, subRatioDenominators);
        }

        private List<double> GetAngles(Mesh MESH)
        {
            var subAngles = new List<double>();
            var normals = MESH.FaceNormals;
            if (normals.Count == 0) // Quad Meshes and others don't have precomputed normals?
            {
                MESH.FaceNormals.ComputeFaceNormals();
                normals = MESH.FaceNormals;
            }

            foreach (var normal in normals)
                if (normal.X == 0 && normal.Y == 0)
                {
                    subAngles.Add(0); // On perfectly flat surfaces measured angles will produce an infinite number
                }
                else
                {
                    var angle = (0.0 - (Math.Asin(Math.Abs(normal.Z)) - 0.5 * Math.PI)) * (180.0 / Math.PI);
                    subAngles.Add(angle);
                }

            return subAngles;
        }
    }
}