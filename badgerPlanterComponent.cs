using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace badger
{
    public class badgerPlanterComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public badgerPlanterComponent()
            : base("Plant Placement Solver", "Placer",
                "Place particular plant objects at a spatial point given particular inputs and constraints",
                "Badger", "Flora")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Plants", "P", "The plant objects to simulate", GH_ParamAccess.list);
            pManager.AddSurfaceParameter("Area", "A", "The area in which to place the plants", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Plants", "P", "The resulting plant objects", GH_ParamAccess.list);
            pManager.AddPointParameter("Locations", "L", "The locations of the resulting plant objects", GH_ParamAccess.list);
            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            //pManager.HideParameter(0);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {            
            // Create holder variables for output parameters
            //double DATUM, YEAR, RISE, SURGE;
            //DATUM = YEAR = RISE = SURGE = 0;
            
            // Access and extract data from the input parameters individually
            //if (!DA.GetData(0, ref DATUM)) return;

            // TODO: determine if I need to do input validation here
                                               
            // Create holder variables for output parameters
            //PlaneSurface seaLevel = createLevel(calculateValue(YEAR, DATUM, RISE, 0));

            // Assign variables to output parameters
            //DA.SetData(0, seaLevel);
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
                return badger.Properties.Resources.icon_flora;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{2d268bdc-ecaa-4cf7-815a-c8111d1798d5}"); }
        }
    }
}
