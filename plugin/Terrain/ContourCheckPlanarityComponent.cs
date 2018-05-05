using System;
using System.Collections.Generic;
using System.Drawing;
using groundhog.Properties;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace groundhog
{
    public class GroundhogContourCheckPlanarityComponent : GroundHogComponent
    {

        public GroundhogContourCheckPlanarityComponent()
            : base("Contour Planarity Fix", "Contour Planarity", "Checks contours are planar and corrects them if not", "Groundhog", "Terrain")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.icon_contour_planarity;

        public override Guid ComponentGuid => new Guid("{2d234bdc-ecaa-4cf7-815a-c8136d1798d0}");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Contour Curves", "C", "The contours to check", GH_ParamAccess.list);
            pManager[0].Optional = false;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("All Contours", "AC", "All contours whether or not they were fixed", GH_ParamAccess.list);
            pManager.AddCurveParameter("Fixed Contours", "FC", "Just the non-planar contours that were fixed", GH_ParamAccess.list);
        }

        protected override void GroundHogSolveInstance(IGH_DataAccess DA)
        {
            // Create holder variables for input parameters
            var ALL_CONTOURS = new List<Curve>();

            // Access and extract data from the input parameters individually
            if (!DA.GetDataList(0, ALL_CONTOURS)) return;

            // Create holder variables for ouput parameters
            var fixedContours = new List<Curve>();
            var allContours = new List<Curve>();

            foreach (var contour in ALL_CONTOURS)
            {
                var degree = contour.Degree;
                if (contour.IsPolyline())
                {
                    Polyline contourPLine;
                    contour.TryGetPolyline(out contourPLine); // Convert to Polyline

                    var zValues = GetZValues(new List<Point3d>(contourPLine.ToArray()));

                    if (zValues[0] != zValues[zValues.Count - 1])
                    {
                        // All are not the same z-index
                        var medianZ = CalculateMean(zValues);

                        var newPoints = new List<Point3d>();
                        foreach (var point in contourPLine)
                        {
                            var newPoint = point;
                            newPoint.Z = medianZ;
                            newPoints.Add(newPoint);
                        }

                        var fixedPolyline = new PolylineCurve(newPoints);
                        fixedContours.Add(fixedPolyline);
                        allContours.Add(fixedPolyline);
                    }
                    else
                    {
                        allContours.Add(contour);
                    }
                }
                else
                {
                    // TODO: probably shouldn't just assume the curve is nurbs
                    var contourNurbsCurve = contour.ToNurbsCurve();

                    var pts = new List<Point3d>();
                    foreach (var ncp in contourNurbsCurve.Points)
                        pts.Add(ncp.Location);

                    var zValues = GetZValues(pts);

                    if (zValues[0] != zValues[zValues.Count - 1])
                    {
                        var medianZ = CalculateMean(zValues);

                        for (var index = 0; index < contourNurbsCurve.Points.Count; index++)
                        {
                            var tempPt = contourNurbsCurve.Points[index].Location;
                            var tempWeight = contourNurbsCurve.Points[index].Weight;
                            tempPt.Z = medianZ;
                            contourNurbsCurve.Points[index] = new ControlPoint(tempPt, tempWeight);
                        }
                        fixedContours.Add(contourNurbsCurve);
                        allContours.Add(contourNurbsCurve);
                    }
                    else
                    {
                        allContours.Add(contour);
                    }
                }
            }

            // Assign variables to output parameters
            DA.SetDataList(0, allContours);
            DA.SetDataList(1, fixedContours);
        }

        private double CalculateMean(List<double> values)
        {
            var count = values.Count;

            if (count == 0) throw new InvalidOperationException("Empty collection");
            return values[Convert.ToInt32(count / 2)];
        }

        private List<double> GetZValues(List<Point3d> controlPts)
        {
            var zValues = new List<double>();
            foreach (var point in controlPts)
                zValues.Add(point.Z);
            zValues.Sort();
            return zValues;
        }
    }
}