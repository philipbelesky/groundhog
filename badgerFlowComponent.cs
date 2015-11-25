using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace badger
{
    public class badgerFlowComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public badgerFlowComponent()
            : base("Flow Simulation", "Flows",
                "Construct flow paths along a surface",
                "Badger", "Hydro")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "Base surface for the flows", GH_ParamAccess.item);
            pManager.AddPointParameter("Points", "P", "Start points for the flow paths", GH_ParamAccess.list);
            pManager.AddNumberParameter("Fidelity", "F", "Amount to move for each flow iteration. Small numbers may take a long time to compute", GH_ParamAccess.item, 100.0);
            pManager.AddNumberParameter("Jump", "J", "A vertical jump component to override small basin effects", GH_ParamAccess.item, 100.0);
            pManager[3].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Flow Paths", "F", "The paths of each simulated point", GH_ParamAccess.list);
            pManager.AddCurveParameter("Flow Catchments", "C", "The catchment zones based on adjacent end points", GH_ParamAccess.list);
            pManager.AddPointParameter("Flow End Points", "P", "When each simulated point ends up", GH_ParamAccess.list);

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
            // Create holder variables for input parameters
            Surface FLOW_SURFACE = default(Surface);
            List<Point3d> FLOW_ORIGINS = new List<Point3d>();
            double FLOW_FIDELITY = 0.0;

            // Access and extract data from the input parameters individually
            if (!DA.GetData(0, ref FLOW_SURFACE)) return;
            if (!DA.GetDataList(1, FLOW_ORIGINS)) return;
            if (!DA.GetData(2, ref FLOW_FIDELITY)) return;

            // We should now validate the data and warn the user if invalid data is supplied.
            if (FLOW_SURFACE == default(Surface))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "tdtdtdtd: make this a useful check 1");
                return;
            }
            if (FLOW_ORIGINS.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "tdtdtdtd: make this a useful check 2");
                return;
            }

            // Create holder variables for output parameters
            List<Point3d> allFlowSteps = new List<Point3d>();
            List<Curve> allFlowPaths = new List<Curve>();

            var calculateTotal = System.Diagnostics.Stopwatch.StartNew();
            
            foreach (Point3d flowOrigin in FLOW_ORIGINS)
            {
                int iterations = 0;
                List<Point3d> flowSteps = new List<Point3d>();
                flowSteps.Add(flowOrigin);
                Point3d flowStart = flowOrigin;

                while (true)
                {
                    iterations++;

                    Vector3d flowDirection = getFlowDirection(FLOW_SURFACE, flowStart);
                    Point3d flowEnd = getFlowEnd(FLOW_SURFACE, flowStart, flowDirection, FLOW_FIDELITY);

                    if (iterations > 1000)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Breaking due to iterations limit");
                        break;
                    }
                    else if (flowEnd == default(Point3d))
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Breaking due to point being off the surface");
                        break;
                    }
                    else if (flowEnd.DistanceTo(flowStart) < Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance)
                    {
                        // If the input and output points are the same its a local maxima / basin
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Breaking due to local maxima");
                        break;
                    }
                    else if (Math.Abs(flowEnd.Z - flowStart.Z) <= Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance)
                    {
                         // If the Z hasn't changed significantly its also probably trapped
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Breaking due to flat plane maxima");
                        break;
                    }
                    else
                    {
                        flowSteps.Add(flowEnd);
                        // Setup the next iteration
                        flowStart = flowEnd;
                    }

                }

                allFlowSteps.AddRange(flowSteps);

                if (flowSteps.Count > 1)
                {
                    var flowPath = Curve.CreateInterpolatedCurve(flowSteps, 3);       
                    allFlowPaths.Add(flowPath);
                }

            }

            calculateTotal.Stop();

            // Assign variables to output parameters
            DA.SetDataList(0, allFlowPaths);
            DA.SetDataList(2, allFlowSteps);
        }

        private Tuple<double, double> getClosestUVOnSurface(Surface FLOW_SURFACE, Point3d point)
        {
            double closestU, closestV;
            FLOW_SURFACE.ClosestPoint(point, out closestU, out closestV);
            return Tuple.Create(closestU, closestV);
        }

        private Point3d getClosestPointOnSurface(Surface FLOW_SURFACE, Point3d point)
        {
            var uv = getClosestUVOnSurface(FLOW_SURFACE, point);
            Point3d closestPoint = FLOW_SURFACE.PointAt(uv.Item1, uv.Item2);
            return closestPoint;
        }

        private Vector3d getFlowDirection(Surface FLOW_SURFACE, Point3d flowOrigin)
        {
            double rotationAngle = Rhino.RhinoMath.ToRadians(90);
            var closestUV = getClosestUVOnSurface(FLOW_SURFACE, flowOrigin);

            Vector3d flowNormal = FLOW_SURFACE.NormalAt(closestUV.Item1, closestUV.Item2);
            Vector3d flowCrossProduct = Rhino.Geometry.Vector3d.CrossProduct(flowNormal, Vector3d.ZAxis);
            flowCrossProduct.Rotate(rotationAngle, flowNormal);
            flowCrossProduct.Unitize();
            return flowCrossProduct;
        }

        private Point3d getFlowEnd(Surface FLOW_SURFACE, Point3d startPoint, Vector3d flowVector, double flowDistance)
        {
            // Apply the vector to the flow origin to get the flow end point
            Vector3d flowVectorMagnitude = Rhino.Geometry.Vector3d.Multiply(flowVector, flowDistance);
            var moveTranslation = Transform.Translation(flowVectorMagnitude);

            Point3d endPoint = startPoint; // It's passing by value right?
            endPoint.Transform(moveTranslation);

            Point3d testPoint = getClosestPointOnSurface(FLOW_SURFACE, endPoint);

            if (endPoint.DistanceTo(testPoint) > (flowDistance / 5))
            {
                // Return nothing if line has overshot the surface
                return default(Point3d);
            }
            else
            {
                return endPoint;
            }
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
                return badger.Properties.Resources.icon_flows;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{2d268bdc-ecaa-4cf7-815a-c8111d1798d1}"); }
        }
    }
}
