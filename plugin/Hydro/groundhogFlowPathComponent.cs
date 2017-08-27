using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using groundhog.Properties;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino;
using Rhino.Geometry;

namespace groundhog
{
    public class groundhogFlowComponent : GH_Component
    {
        /// <summary>
        ///     Each implementation of GH_Component must provide a public
        ///     constructor without any arguments.
        ///     Category represents the Tab in which the component will appear,
        ///     Subcategory the panel. If you use non-existing tab or panel names,
        ///     new tabs/panels will automatically be created.
        /// </summary>
        public groundhogFlowComponent()
            : base("Flow Simulation", "Flows",
                "Construct flow paths along a surface",
                "Groundhog", "Hydro")
        {
        }


        /// <summary>
        ///     The Exposure property controls where in the panel a component icon
        ///     will appear. There are seven possible locations (primary to septenary),
        ///     each of which can be combined with the GH_Exposure.obscure flag, which
        ///     ensures the component will only be visible on panel dropdowns.
        /// </summary>
        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        ///     Provides an Icon for every component that will be visible in the User Interface.
        ///     Icons need to be 24x24 pixels.
        /// </summary>
        protected override Bitmap Icon => Resources.icon_flows;

        /// <summary>
        ///     Each component must have a unique Guid to identify it.
        ///     It is vital this Guid doesn't change otherwise old ghx files
        ///     that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{2d268bdc-ecaa-4cf7-815a-c8111d1798d1}");

        /// <summary>
        ///     Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface", "S", "Base landscape form (as surface) for the flows",
                GH_ParamAccess.item);
            pManager[0].Optional = true;
            pManager.AddMeshParameter("Mesh", "M", "Base landscape form (as mesh) for the flows", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddPointParameter("Points", "P", "Start points for the flow paths", GH_ParamAccess.list);
            pManager.AddNumberParameter("Fidelity", "F",
                "Amount to move for each flow iteration. Small numbers may take a long time to compute",
                GH_ParamAccess.item, 100.0);
            pManager.AddBooleanParameter("Thread", "T", "Whether to multithread the calculation", GH_ParamAccess.item,
                false);
            pManager[4].Optional = true;
        }

        /// <summary>
        ///     Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Flow Points", "F", "The points of each simulated point of movement",
                GH_ParamAccess.tree);
            pManager.AddCurveParameter("Flow Paths", "C", "A polyline linking each point", GH_ParamAccess.list);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            //pManager.HideParameter(0);
        }

        /// <summary>
        ///     This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">
        ///     The DA object can be used to retrieve data from input parameters and
        ///     to store data in output parameters.
        /// </param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Create holder variables for input parameters
            var FLOW_SURFACE = default(Surface);
            var FLOW_MESH = default(Mesh);
            var FLOW_ORIGINS = new List<Point3d>();
            var FLOW_FIDELITY = 1000.0;
            var THREAD = false;

            // Access and extract data from the input parameters individually
            DA.GetData(0, ref FLOW_SURFACE);
            DA.GetData(1, ref FLOW_MESH);
            if (!DA.GetDataList(2, FLOW_ORIGINS)) return;
            if (!DA.GetData(3, ref FLOW_FIDELITY)) return;
            if (!DA.GetData(4, ref THREAD)) return;

            var startPoints = FLOW_ORIGINS.ToArray(); // Array for multithreading
            var allFlowPathPoints = new List<Point3d>[startPoints.Length]; // Array of all the paths
            var flowPoints = new List<Point3d>();


            if (FLOW_SURFACE != default(Surface) && FLOW_MESH != default(Mesh))
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Chose to add either a surface or a mesh - not both!");

            var FLOW_BREP = default(Brep);
            if (FLOW_SURFACE != default(Surface)) FLOW_BREP = FLOW_SURFACE.ToBrep();

            if (THREAD)
                Parallel.For(0, startPoints.Length, i => // Shitty multithreading
                    {
                        allFlowPathPoints[i] = dispatchFlowPoints(FLOW_BREP, FLOW_MESH, startPoints[i], FLOW_FIDELITY);
                    }
                );
            else
                for (var i = 0; i < startPoints.Length; i = i + 1)
                    allFlowPathPoints[i] = dispatchFlowPoints(FLOW_BREP, FLOW_MESH, startPoints[i], FLOW_FIDELITY);

            var allFlowPathPointsTree = new DataTree<object>();
            var allFlowPathCurvesList = new List<Polyline>();


            for (var i = 0; i < allFlowPathPoints.Length; i++)
            {
                var path = new GH_Path(i);
                // For each flow path make the polyline
                if (allFlowPathPoints[i].Count > 1)
                {
                    var flowPath = new Polyline(allFlowPathPoints[i]);
                    allFlowPathCurvesList.Add(flowPath);
                }

                // And make a branch for the list of points
                for (var j = 0; j < allFlowPathPoints[i].Count; j++)
                    // For each flow path point
                    allFlowPathPointsTree.Add(allFlowPathPoints[i][j], path);
            }

            // Assign variables to output parameters
            DA.SetDataTree(0, allFlowPathPointsTree);
            DA.SetDataList(1, allFlowPathCurvesList);
        }


        private List<Point3d> dispatchFlowPoints(Brep FLOW_SURFACE, Mesh FLOW_MESH, Point3d initialStartPoint,
            double MOVE_DISTANCE)
        {
            var flowPoints = new List<Point3d>(); // Holds each step

            if (FLOW_MESH == default(Mesh) && FLOW_SURFACE == default(Brep))
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Both are null");
            bool usingMesh;
            if (FLOW_MESH == default(Mesh))
                usingMesh = false;
            else
                usingMesh = true;

            Point3d startPoint;
            if (usingMesh)
                startPoint = FLOW_MESH.ClosestPoint(initialStartPoint);
            else
                startPoint = FLOW_SURFACE.ClosestPoint(initialStartPoint);
            flowPoints.Add(startPoint);


            while (true)
            {
                Point3d nextPoint;
                if (usingMesh)
                    nextPoint = getNextFlowStepOnMesh(FLOW_MESH, startPoint, MOVE_DISTANCE);
                else
                    nextPoint = getNextFlowStepOnSurface(FLOW_SURFACE, startPoint, MOVE_DISTANCE);

                if (nextPoint.DistanceTo(startPoint) <= RhinoDoc.ActiveDoc.ModelAbsoluteTolerance)
                    break; // Test the point has actully moved
                if (nextPoint.Z >= startPoint.Z)
                    break; // Test this point is actually lower
                flowPoints.Add(nextPoint);
                startPoint = nextPoint; // Checks out; iterate on
            }

            return flowPoints;
        }

        private Point3d getNextFlowStepOnSurface(Brep FLOW_SURFACE, Point3d startPoint, double MOVE_DISTANCE)
        {
            double closestS, closestT;
            double maximumDistance = 0; // TD: setting this as +ve speeds up the search?
            Vector3d closestNormal;
            ComponentIndex closestCI;
            Point3d closestPoint;

            // Get closest point
            FLOW_SURFACE.ClosestPoint(startPoint, out closestPoint, out closestCI, out closestS, out closestT,
                maximumDistance, out closestNormal);

            // Get the vector to flow down
            var flowVector = Vector3d.CrossProduct(Vector3d.ZAxis, closestNormal);
            flowVector.Unitize();
            flowVector.Reverse();
            flowVector.Transform(Transform.Rotation(Math.PI / 2, closestNormal, closestPoint));

            // Flow to the new point
            var nextFlowPoint = Point3d.Add(closestPoint, flowVector * MOVE_DISTANCE);

            // Need to snap back to the surface (the vector may be pointing off the edge)
            return FLOW_SURFACE.ClosestPoint(nextFlowPoint);
        }

        private Point3d getNextFlowStepOnMesh(Mesh FLOW_MESH, Point3d startPoint, double MOVE_DISTANCE)
        {
            double maximumDistance = 0; // TD: setting this as +ve speeds up the search?
            Vector3d closestNormal;
            Point3d closestPoint;

            // Get closest point
            FLOW_MESH.ClosestPoint(startPoint, out closestPoint, out closestNormal, maximumDistance);

            // Get the vector to flow down
            var flowVector = Vector3d.CrossProduct(Vector3d.ZAxis, closestNormal);
            flowVector.Unitize();
            flowVector.Reverse();
            flowVector.Transform(Transform.Rotation(Math.PI / 2, closestNormal, closestPoint));

            // Flow to the new point
            //Point3d nextFlowPoint = Point3d.Add(closestPoint, V * MOVE_DISTANCE);
            var nextFlowPoint = Point3d.Add(closestPoint, flowVector * MOVE_DISTANCE);

            // Need to snap back to the surface (the vector may be pointing off the edge)
            return FLOW_MESH.ClosestPoint(nextFlowPoint);
        }
    }
}