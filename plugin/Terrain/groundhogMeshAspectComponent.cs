﻿using System;
using System.Collections.Generic;
using System.Drawing;
using groundhog.Properties;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace groundhog
{
    public class groundhogMeshAspectComponent : GroundHog_Component
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
            pManager.AddNumberParameter("Face Aspects", "A", "The aspect of the slope (measured in degrees)", GH_ParamAccess.list);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            var MESH = default(Mesh);
            var ASPECT = default(Vector3d);

            // Access and extract data from the input parameters individually
            if (!DA.GetData(0, ref MESH)) return;
            if (!DA.GetData(1, ref ASPECT)) return;

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
            {
                if ((direction.X == 0 && direction.Y == 0) || direction.IsZero)
                {
                    subAspects.Add(0); // On perfectly flat surfaces measured angles will produce an infinite Angle
                }
                else
                {
                    var angle = Vector3d.VectorAngle(ASPECT, direction, leftPlane);
                    subAspects.Add(angle * (180 / Math.PI)); // Convert to radians
                }
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