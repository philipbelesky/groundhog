using System;
using System.Collections.Generic;
using System.Drawing;
using groundhog.Properties;
using Grasshopper.Kernel;
using Rhino.Geometry;


public static class TerrainCalculations
{

    public static List<Point3d> GetCenters(Mesh mesh)
    {
        var centers = new List<Point3d>();
        for (var f = 0; f < mesh.Faces.Count; f++)
            centers.Add(mesh.Faces.GetFaceCenter(f));
        return centers;
    }

    public static List<Vector3d> GetDirections(List<Mesh> meshes, List<Point3d> subCentres)
    {
        var directions = new List<Vector3d>();
        for (var m = 0; m < meshes.Count; m++)
        {
            var vertices = meshes[m].Vertices.ToPoint3dArray();

            Array.Sort(vertices, delegate (Point3d x, Point3d y) { return x.Z.CompareTo(y.Z); }); // Sort by Z values

            Point3d min;
            if (vertices[0].Z == vertices[1].Z && vertices[1].Z == vertices[2].Z)
                min = subCentres[m];
            else if (vertices[0].Z == vertices[1].Z)
                min = new Point3d(
                    0.5 * (vertices[0].X - vertices[1].X) + vertices[1].X,
                    0.5 * (vertices[0].Y - vertices[1].Y) + vertices[1].Y,
                    0.5 * (vertices[0].Z - vertices[1].Z) + vertices[1].Z
                );
            else
                min = vertices[0]; // Otherwise use lowerst

            // Get vector to lowest vertex
            var direction = new Vector3d(min.X - subCentres[m].X, min.Y - subCentres[m].Y, min.Z - subCentres[m].Z);

            directions.Add(direction);
        }

        return directions;
    }

    public static List<Mesh> Explode(Mesh m)
    {
        var rtnlist = new List<Mesh>();

        for (var f = 0; f < m.Faces.Count; f++)
        {
            var newmesh = new Mesh();
            newmesh.Vertices.Add(m.Vertices[m.Faces[f].A]);
            newmesh.Vertices.Add(m.Vertices[m.Faces[f].B]);
            newmesh.Vertices.Add(m.Vertices[m.Faces[f].C]);
            if (m.Faces[f].IsQuad) newmesh.Vertices.Add(m.Vertices[m.Faces[f].D]);
            if (m.Faces[f].IsTriangle) newmesh.Faces.AddFace(0, 1, 2);
            if (m.Faces[f].IsQuad) newmesh.Faces.AddFace(0, 1, 2, 3);

            rtnlist.Add(newmesh);
        }

        return rtnlist;
    }

}