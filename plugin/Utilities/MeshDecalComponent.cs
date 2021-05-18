namespace Groundhog.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Grasshopper.Kernel;
    using Groundhog.Properties;
    using Rhino.Geometry;

    // Why is this not a native component :/
    public class MeshDecalComponent : GroundHogComponent
    {
        public MeshDecalComponent()
            : base("Mesh Color by Face", "Mesh Paint", "Colors each mesh face with a solid color, in order. Like mesh spray, but without any blending.", "Groundhog", "Utilities")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.icon_mesh_fill;
        public override Guid ComponentGuid => new Guid("{e17330a1-3eea-443f-aaad-3ad0520d3583}");

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            Mesh meshToColor = null;
            var colorsPerFace = new List<Color>();

            if (!DA.GetData(0, ref meshToColor)) return;
            if (!DA.GetDataList(1, colorsPerFace)) return;

            meshToColor.VertexColors.CreateMonotoneMesh(System.Drawing.Color.White);
            meshToColor.Unweld(0, false);

            for (var i = 0; i < meshToColor.Faces.Count; i++)
            {
                var face = meshToColor.Faces[i];
                var color = colorsPerFace[i];
                meshToColor.VertexColors.SetColor(face, color);
            }

            DA.SetData(0, meshToColor);
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "TODO", GH_ParamAccess.item);
            pManager[0].Optional = false;
            pManager.AddColourParameter("Colors", "C", "TODO", GH_ParamAccess.list);
            pManager[1].Optional = false;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("All Contours", "AC", "All contours whether or not they were fixed",
                GH_ParamAccess.item);
        }
    }
}
