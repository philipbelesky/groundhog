using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace groundhog
{
    public class groundhogFieldComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public groundhogFieldComponent()
            : base("Field Mapper", "Field",
                "Create ",
                "Groundhog", "Mapping")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Bounds", "B", "Boundary box for the resulting field", GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddPointParameter("Points", "P", "Sample points for the resulting field", GH_ParamAccess.list);
            pManager.AddCurveParameter("Areas", "A", "Closed curves representing particular values", GH_ParamAccess.tree);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // Generic is its a GH_ObjectWrapper wrapper for our custom class
            pManager.AddSurfaceParameter("Field", "F", "Resulting field", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Create holder variables for input parameters
            Curve gridBounds = null;
            int gridDivisions = 24;
            List<Curve> areas = new List<Curve>();

            // Access and extract data from the input parameters individually
            if (!DA.GetData(0, ref gridBounds)) return;
            if (!DA.GetData(1, ref gridDivisions)) return;
            if (!DA.GetDataList(2, areas)) return;

            // Create holder variables for output parameters

            // Assign variables to output parameters
            //DA.SetData(0, field);

        }

 

           /// <summary>
        /// The Exposure property controls where in the panel a component icon 
        /// will appear. There are seven possible locations (primary to septenary), 
        /// each of which can be combined with the GH_Exposure.obscure flag, which 
        /// ensures the component will only be visible on panel dropdowns.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return groundhog.Properties.Resources.icon_field;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{2d268bdc-ecaa-4cf7-811a-c8111d1798d4}"); }
        }
    }
}
