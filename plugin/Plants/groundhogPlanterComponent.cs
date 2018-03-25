using System;
using System.Drawing;
using groundhog.Properties;
using Grasshopper.Kernel;

namespace groundhog
{
    // To fix/restore
    //public class groundhogPlanterComponent : GroundHog_Component
    //{

    //    public groundhogPlanterComponent()
    //        : base("Plant Placement Solver", "Placer", "Place particular plant objects at a spatial point given particular inputs and constraints", "Groundhog", "Flora")
    //    {
    //    }

    //    public override GH_Exposure Exposure => GH_Exposure.primary;

    //    protected override Bitmap Icon => Resources.icon_pplacer;

    //    public override Guid ComponentGuid => new Guid("{2d268bdc-ecaa-4cf7-815a-c8111d1798d5}");

    //    protected override void RegisterInputParams(GH_InputParamManager pManager)
    //    {
    //        pManager.AddGenericParameter("Plants", "P", "The planting pallete from which to select from", GH_ParamAccess.list);
    //        pManager.AddSurfaceParameter("Area", "A", "The area in which to place the plants", GH_ParamAccess.item);
    //    }

    //    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    //    {
    //        pManager.AddGenericParameter("Plants", "P", "The resulting plant objects", GH_ParamAccess.list);
    //        pManager.AddPointParameter("Locations", "L", "The locations of the resulting plant objects", GH_ParamAccess.list);
    //    }

    //    protected override void SolveInstance(IGH_DataAccess DA)
    //    {

    //    }
    //}
}