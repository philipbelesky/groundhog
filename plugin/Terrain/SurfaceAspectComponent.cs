using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using groundhog.Properties;
using Rhino.Geometry;

namespace groundhog
{
    public class GroundhogSurfaceAspectComponent : GroundHogComponent
    {
        public GroundhogSurfaceAspectComponent()
            : base("Surface Aspect", "Aspect",
                "Analyses the aspect of a Surface, outputting separated faces for coloring and the aspect", "Groundhog",
                "Terrain")
        {
        }

        protected override Bitmap Icon => Resources.icon_surface_aspect;

        public override Guid ComponentGuid => new Guid("{56be2741-8d92-4607-8177-a4108d9d72fd}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "The terrain surface", GH_ParamAccess.item);
            pManager[0].Optional = false;
            pManager.AddVectorParameter("Aspect", "A", "Vector representing the direction to measure aspect against",
                GH_ParamAccess.item, new Vector3d(0, 1, 0));
            pManager[1].Optional = true;
            // TODO: add Mesh construction settings/parameters (Param_MeshParameters)
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh Faces", "F", "The sub mesh faces (for coloring)", GH_ParamAccess.list);
            pManager.AddPointParameter("Face Centers", "C", "The centers of each mesh face (for vector previews)",
                GH_ParamAccess.list);
            pManager.AddNumberParameter("Face Aspects °", "A", "The aspect of each mesh face (measured in degrees)",
                GH_ParamAccess.list);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            var SURFACE = default(Surface);
            var ASPECT = default(Vector3d);

            // Access and extract data from the input parameters individually
            if (!DA.GetData(0, ref SURFACE)) return;
            if (!DA.GetData(1, ref ASPECT)) return;

            // Convert Surface to Mesh; TODO: expose mesh parameters; handle multiple outputs
            var PREMESH = Mesh.CreateFromBrep(SURFACE.ToBrep(), MeshingParameters.Default);
            var MESH = PREMESH[0];

            var subMeshes = TerrainCalculations.Explode(MESH);
            var subCentres = TerrainCalculations.GetCenters(MESH);
            var subDirections = TerrainCalculations.GetDirections(subMeshes, subCentres);
            // This is the only step different to Slope; i.e. measure angle difference between slope and given vector
            var subAspects = GetAspects(subDirections, ASPECT);

            // Assign variables to output parameters
            DA.SetDataList(0, subMeshes);
            DA.SetDataList(1, subCentres);
            DA.SetDataList(2, subAspects);
        }

        private List<double> GetAspects(List<Vector3d> subDirections, Vector3d ASPECT)
        {
            var subAspects = new List<double>();
            // Need to measure with a specified plane so it doesn't return the smallest angle but rather the rotational/radial angle
            var leftPlane = new Plane(new Point3d(0, 0, 0), new Vector3d(0, 0, -1));
            foreach (var direction in subDirections)
                if (direction.X == 0 && direction.Y == 0 || direction.IsZero)
                {
                    subAspects.Add(0); // On perfectly flat surfaces measured angles will produce an infinite Angle
                }
                else
                {
                    var angle = Vector3d.VectorAngle(ASPECT, direction, leftPlane);
                    subAspects.Add(angle * (180 / Math.PI)); // Convert to radians
                }

            return subAspects;
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
            {
                var angle = (0.0 - (Math.Asin(Math.Abs(normal.Z)) - 0.5 * Math.PI)) * (180.0 / Math.PI);
                subAngles.Add(angle);
            }

            return subAngles;
        }
    }
}