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


public static class FlowCalculations
{
    public static double getSensibleFidelity(List<Point3d> flowPathPoints)
    {
        // Measure distances between each of the points
        double[] distances = new double[flowPathPoints.Count];
        for (var i = 1; i < flowPathPoints.Count; i++)
        {
            distances[i - 1] = flowPathPoints[0].DistanceTo(flowPathPoints[i]);
        }
        // Sort and find the average of the first three results; then divide to find a decent result
        Array.Sort(distances);
        var averageDistance = (distances[0] + distances[1] + distances[2]) / 3;
        var sensibleDistance = averageDistance / 10;
        return sensibleDistance;
    }

    public static Tuple<Grasshopper.DataTree<object>, List<Polyline>> MakeOutputs(List<Point3d>[] flowPathPoints)
    {
        var allFlowPathPointsTree = new Grasshopper.DataTree<object>();
        var allFlowPathCurvesList = new List<Polyline>();

        for (var i = 0; i < flowPathPoints.Length; i++)
        {
            var path = new GH_Path(i);
            // For each flow path make the polyline
            if (flowPathPoints[i].Count > 1)
            {
                var flowPath = new Polyline(flowPathPoints[i]);
                allFlowPathCurvesList.Add(flowPath);
            }

            // And make a branch for the list of points
            for (var j = 0; j < flowPathPoints[i].Count; j++)
                allFlowPathPointsTree.Add(flowPathPoints[i][j], path); // For each flow path point
        }

        return Tuple.Create(allFlowPathPointsTree, allFlowPathCurvesList);
    }

    public static Point3d MoveFlowPoint(Vector3d closestNormal, Point3d closestPoint, double MOVE_DISTANCE) 
    {
        // Get the vector to flow down
        var flowVector = Vector3d.CrossProduct(Vector3d.ZAxis, closestNormal);
        flowVector.Unitize();
        flowVector.Reverse();
        flowVector.Transform(Transform.Rotation(Math.PI / 2, closestNormal, closestPoint));

        // Flow to the new point
        var nextFlowPoint = Point3d.Add(closestPoint, flowVector * MOVE_DISTANCE);
        return nextFlowPoint;        
    }
}