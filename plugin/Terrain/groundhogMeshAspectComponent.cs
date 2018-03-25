using System;
using System.Collections.Generic;
using System.Drawing;
using groundhog.Properties;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace groundhog
{
    public class groundhogMeshAspectComponent : GH_Component
    {
        public groundhogMeshAspectComponent()
            : base("Mesh Aspect", "Aspect", "Analyses the slope of a Mesh, outputting separated faces for coloring and the slope/grade", "Groundhog", "Terrain")
        {
        }

        protected override Bitmap Icon => Resources.icon_mesh_aspect;

        public override Guid ComponentGuid => new Guid("{c3b67aca-0c15-2552-9d6c-96cce97fcb47}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "The terrain mesh", GH_ParamAccess.item);
            pManager[0].Optional = false;
            pManager.AddVectorParameter("Aspect", "A", "Vector representing the direction to measure aspect against", GH_ParamAccess.item, new Vector3d(0, 1, 0));
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh Faces", "F", "The sub mesh faces (for coloring)", GH_ParamAccess.list);
            pManager.AddPointParameter("Face Centers", "C", "The centers of each mesh face (for vector previews)", GH_ParamAccess.list);
            pManager.AddNumberParameter("Face Aspect Angles", "A", "The angle of the slope", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var MESH = default(Mesh);
            var ASPECT = default(Vector3d);

            // Access and extract data from the input parameters individually
            if (!DA.GetData(0, ref MESH)) return;
            if (!DA.GetData(1, ref ASPECT)) return;

            var subMeshes = Explode(MESH);
            var subAngles = GetAngles(MESH);
            var subCentres = GetCenters(MESH);
            var subDirections = GetDirections(subMeshes, subCentres);

            // This is the only step different to Slope; i.e. measure angle difference between slope and given vector
            var subAspects = new List<double>();
            // Need to measure with a specified plane so it doesn't return the smallest angle but rather the rotational/radial angle
            var leftPlane = new Plane(new Point3d(0, 0, 0), new Vector3d(0, 0, -1)); 
            foreach (var direction in subDirections)
            {
                if (direction.IsZero)
                {
                    subAspects.Add(0); // On perfectly flat surfaces measured angles will produce an infinite Angle
                }
                else
                {
                    var angle = Vector3d.VectorAngle(ASPECT, direction, leftPlane);
                    subAspects.Add(angle * (180 / Math.PI)); // Convert to radians
                }
            }

            // Assign variables to output parameters
            DA.SetDataList(0, subMeshes);
            DA.SetDataList(1, subCentres);
            DA.SetDataList(2, subAspects);
        }

        private List<double> GetAngles(Mesh mesh)
        {
            var subAngles = new List<double>();
            var normals = mesh.FaceNormals;
            foreach (var normal in normals)
            {
                var angle = (0.0 - (Math.Asin(Math.Abs(normal.Z)) - 0.5 * Math.PI)) * (180.0 / Math.PI);
                subAngles.Add(angle);
            }
            return subAngles;
        }

        private List<Point3d> GetCenters(Mesh mesh)
        {
            var centers = new List<Point3d>();
            for (var f = 0; f < mesh.Faces.Count; f++)
                centers.Add(mesh.Faces.GetFaceCenter(f));
            return centers;
        }

        private List<Vector3d> GetDirections(List<Mesh> meshes, List<Point3d> subCentres)
        {
            var directions = new List<Vector3d>();
            for (var m = 0; m < meshes.Count; m++)
            {
                var vertices = meshes[m].Vertices.ToPoint3dArray();

                Array.Sort(vertices, delegate(Point3d x, Point3d y) { return x.Z.CompareTo(y.Z); }); // Sort by Z values

                Point3d min;
                if (vertices[0].Z == vertices[1].Z && vertices[1].Z == vertices[2].Z)
                    min = subCentres[m];
                else if (vertices[0].Z == vertices[1].Z)
                    min = new Point3d(
                        0.5 * (vertices[0].X - vertices[1].X) + vertices[1].X,
                        0.5 * (vertices[0].Y - vertices[1].Y) + vertices[1].Y,
                        0.5 * (vertices[0].Z - vertices[1].Z) + vertices[1].Z
                    );
                else
                    min = vertices[0]; // Otherwise use lowerst

                // Get vector to lowest vertex
                var direction = new Vector3d(min.X - subCentres[m].X, min.Y - subCentres[m].Y, min.Z - subCentres[m].Z);
                
                directions.Add(direction);
            }

            return directions;
        }

        private List<Mesh> Explode(Mesh m)
        {
            var rtnlist = new List<Mesh>();

            for (var f = 0; f < m.Faces.Count; f++)
            {
                var newmesh = new Mesh();
                newmesh.Vertices.Add(m.Vertices[m.Faces[f].A]);
                newmesh.Vertices.Add(m.Vertices[m.Faces[f].B]);
                newmesh.Vertices.Add(m.Vertices[m.Faces[f].C]);
                if (m.Faces[f].IsQuad) newmesh.Vertices.Add(m.Vertices[m.Faces[f].D]);
                if (m.Faces[f].IsTriangle) newmesh.Faces.AddFace(0, 1, 2);
                if (m.Faces[f].IsQuad) newmesh.Faces.AddFace(0, 1, 2, 3);

                rtnlist.Add(newmesh);
            }

            return rtnlist;
        }
    }
}