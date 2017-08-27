using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace groundhog
{
    public class groundhogMeshGradeComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public groundhogMeshGradeComponent()
            : base("Mesh Slope", "Mesh",
                "Analyses the slope of a Mesh, outputting sseparated faces for coloring and the slope/grade",
                "Groundhog", "Terrain")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "The terrain mesh", GH_ParamAccess.item);
            pManager[0].Optional = false;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh Faces", "F", "The sub mesh faces (for coloring)", GH_ParamAccess.list);
            pManager.AddPointParameter("Face Centers", "C", "The centers of each mesh face (for vector previews)", GH_ParamAccess.list);
            pManager.AddVectorParameter("Face Slope Vectors", "V", "The direction to the lowest points of each face", GH_ParamAccess.list);
            pManager.AddNumberParameter("Face Slope Angles", "A", "The angle of the slope", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh M = default(Mesh);
            // Access and extract data from the input parameters individually
            if (!DA.GetData(0, ref M)) return;

            List<Mesh> subMeshes = Explode(M);
            List<double> subAngles = getAngles(M);
            List<Point3d> subCentres = getCenters(M);

            List<Vector3d> subDirections = getDirections(subMeshes, subCentres);

            // Assign variables to output parameters
            DA.SetDataList(0, subMeshes);
            DA.SetDataList(1, subCentres);
            DA.SetDataList(2, subDirections);
            DA.SetDataList(3, subAngles);

        }

        private List<double> getAngles(Mesh mesh)
        {
            List<double> subAngles = new List<double>();
            var normals = mesh.FaceNormals;
            foreach (Vector3f normal in normals)
            {
                double angle = (0.0 - (Math.Asin(Math.Abs(normal.Z)) - 0.5 * Math.PI)) * (180.0 / Math.PI);
                subAngles.Add(angle);
            }
            return subAngles;
        }


        private List<Point3d> getCenters(Mesh mesh)
        {
            List<Point3d> centers = new List<Point3d>();
            for (int f = 0; f < mesh.Faces.Count; f++)
            {
                centers.Add(mesh.Faces.GetFaceCenter(f));
            }
            return centers;
        }


        private List<Vector3d> getDirections(List<Mesh> meshes, List<Point3d> subCentres)
        {
            List<Vector3d> directions = new List<Vector3d>();
            for (int m = 0; m < meshes.Count; m++)
            {
                Point3d[] vertices = meshes[m].Vertices.ToPoint3dArray();

                Array.Sort(vertices, delegate(Point3d x, Point3d y) { return x.Z.CompareTo(y.Z); }); // Sort by Z values

                Point3d min;
                if (vertices[0].Z == vertices[1].Z && vertices[1].Z == vertices[2].Z)
                { // If totally flat
                    // If equal use center
                    min = subCentres[m];
                }
                else if (vertices[0].Z == vertices[1].Z)
                {
                    // If equal use halfway point between lowest
                    min = new Point3d(
                      0.5 * (vertices[0].X - vertices[1].X) + vertices[1].X,
                      0.5 * (vertices[0].Y - vertices[1].Y) + vertices[1].Y,
                      0.5 * (vertices[0].Z - vertices[1].Z) + vertices[1].Z
                      );
                }
                else
                {
                    min = vertices[0]; // Otherwise use lowerst
                }

                // Get vector to lowest vertex
                Vector3d direction = new Vector3d(min.X - subCentres[m].X, min.Y - subCentres[m].Y, min.Z - subCentres[m].Z);

                direction.Unitize();
                directions.Add(direction);

            }

            return directions;
        }


        private List<Mesh> Explode(Mesh m)
        {
            var rtnlist = new List<Mesh>();

            for (int f = 0; f < m.Faces.Count; f++)
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

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return groundhog.Properties.Resources.icon_mesh_slope;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{c3b67aca-0e15-4279-9d6c-96cce97fcb47}"); }
        }
    }
}