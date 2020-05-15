﻿using System;
using System.Collections.Generic;
using System.Drawing;
using groundhog.Properties;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;

namespace groundhog
{
    public class GroundhogFieldVisualisationGridComponent : GroundHogComponent
    {
        public GroundhogFieldVisualisationGridComponent()
          : base("Field Visualisation (Grid)", "Field Grid", "Translate a field into a grid based visualisation. Outputs a number constrained by a range which can be used to then make a color or shape.",               "Groundhog", "Mapping")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Field", "F", "Field generated by a Field Component", GH_ParamAccess.item);
            pManager.AddCurveParameter("Bounds", "B", "Curve boundary to constraint the visualisation to a certain area", GH_ParamAccess.item);
            pManager[1].Optional = true;

            pManager.AddNumberParameter("Domain Start", "DS", "Starting value representing the 'bottom' value", GH_ParamAccess.item, 0.0);
            pManager[2].Optional = true;

            pManager.AddNumberParameter("Domain End", "DE", "Ending value representing the 'top' value", GH_ParamAccess.item, 1.0);
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "P", "Grid Points", GH_ParamAccess.list);
            pManager.AddNumberParameter("Parameters", "P", "Grid Parameters", GH_ParamAccess.list);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            // Create holder variables for input parameters
            Surface gridRawField = null;
            Curve gridBoundary = null;
            double pStart = 0.0;
            double pEnd = 1.0;

            if (!DA.GetData(0, ref gridRawField)) return;
            if (!DA.GetData(1, ref gridBoundary)) return;
            if (!DA.GetData(2, ref pStart)) return;
            if (!DA.GetData(3, ref pEnd)) return;

            if (pStart == pEnd)
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Parameter start and end numbers must be different");
            // Validate boundary curve is planar
            if (gridBoundary != null && !gridBoundary.IsPlanar())
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Boundary curve is not planar");
            if (gridBoundary != null && !gridBoundary.IsClosed)
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Boundary curve is not closed");

            NurbsSurface gridField = gridRawField.ToNurbsSurface();
            if (gridRawField == null)
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Grid field is null or not able to be converted to a NURBS surface");

            var inBoundsPoints = new List<Point3d>();
            var surfaceCP = gridField.Points;
            double? lowestZ = null; // Track lowest point for range normalisation purposes
            double? highestZ = null; // Track lowest point for range normalisation purposes
            int uCount = surfaceCP.CountU;
            int vCount = surfaceCP.CountV;

            // Get Euclidean point coordinates and remove points out of bounds
            for (var u = 0; u < uCount; u = u + 1)
            {
                for (var v = 0; v < vCount; v = v + 1)
                {
                    var point = surfaceCP.GetControlPoint(u, v).Location;
                    if (!lowestZ.HasValue || point.Z < lowestZ)
                    {
                        lowestZ = point.Z;
                    }
                    if (!highestZ.HasValue || point.Z > highestZ)
                    {
                        highestZ = point.Z;
                    }

                    if (gridBoundary != null)
                    {
                        var pointContainment = gridBoundary.Contains(point, Plane.WorldXY, docUnitTolerance);
                        if (pointContainment.ToString() == "Inside")
                        {
                            inBoundsPoints.Add(point);
                        }
                    }
                    else
                    {
                        inBoundsPoints.Add(point);
                    }
                }
            }

            // Check before force-unwrapping with .Value() below
            if (highestZ == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Upper bound null; this shouldn't happen");
            }
            if (lowestZ == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Upper bound null; this shouldn't happen");
            }

            var inBoundsParameters = new List<double>();
            for (var i = 0; i < inBoundsPoints.Count; i = i + 1)
            {
                var z = inBoundsPoints[i].Z;
                double remappedZ = Remap(z, lowestZ.Value, highestZ.Value, pStart, pEnd);
                inBoundsParameters.Add(remappedZ);
            }

            // Assign variables to output parameters
            DA.SetDataList(0, inBoundsPoints);
            DA.SetDataList(1, inBoundsParameters);
        }

        protected override Bitmap Icon => Resources.icon_field_grid_vis;

        public override Guid ComponentGuid
        {
            get { return new Guid("34ab1ff1-fee6-4a7c-a8cb-1d074b7ff4c3"); }
        }

        private double Remap(double value, double fromLow, double fromHigh, double toLow, double toHigh)
        {
            // Should really make this a shared method; I use it all over the show
            return (value - fromLow) / (fromHigh - fromLow) * (toHigh - toLow) + toLow;
        }
    }
}