﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using Rhino.Geometry;

namespace Extensions
{
    public static class RenderExtensions
    {
        public static Mesh BitmapFromVertexColors(Mesh mesh, string file)
        {          
            if (!Directory.Exists(Path.GetDirectoryName(file))) throw new FileNotFoundException($" File \"{file}\" not found.", file);

            mesh.Unweld(0, false);
            mesh.TextureCoordinates.Clear();

            for (int i = 0; i < mesh.Vertices.Count; i++)
                mesh.TextureCoordinates.Add(0, 0);

            int size = (int)Math.Ceiling(Math.Sqrt(mesh.Faces.Count));
            float fSize = (float)size * 2;

            Bitmap bitmap = new Bitmap(size * 2, size * 2);

            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                MeshFace face = mesh.Faces[i];
                int x = (i % size) * 2;
                int y = (i / size) * 2;
                float fx = (float)x;
                float fy = (float)y;

                mesh.TextureCoordinates[face.A] = new Point2f((fx + 0.5) / fSize, (fy + 0.5) / fSize);
                mesh.TextureCoordinates[face.B] = new Point2f((fx + 1.5) / fSize, (fy + 0.5) / fSize);
                mesh.TextureCoordinates[face.C] = new Point2f((fx + 1.5) / fSize, (fy + 1.5) / fSize);
                mesh.TextureCoordinates[face.D] = new Point2f((fx + 0.5) / fSize, (fy + 1.5) / fSize);

                Color colorA = mesh.VertexColors[face.A];
                colorA = Color.FromArgb(255, colorA.R, colorA.G, colorA.B);
                Color colorB = mesh.VertexColors[face.B];
                colorB = Color.FromArgb(255, colorB.R, colorB.G, colorB.B);
                Color colorC = mesh.VertexColors[face.C];
                colorC = Color.FromArgb(255, colorC.R, colorC.G, colorC.B);
                Color colorD = mesh.VertexColors[face.D];
                colorD = Color.FromArgb(255, colorD.R, colorD.G, colorD.B);

                y = size * 2 - 2 - y;
                bitmap.SetPixel(x + 0, y + 1, colorA);
                bitmap.SetPixel(x + 1, y + 1, colorB);
                bitmap.SetPixel(x + 1, y + 0, colorC);
                bitmap.SetPixel(x + 0, y + 0, colorD);
            }

            bitmap.Save(file, System.Drawing.Imaging.ImageFormat.Png);
            return mesh;
        }

        public static IEnumerable<Mesh> BitmapFromSolidColoredMeshes(IEnumerable<Mesh> meshes, string file)
        {
            if (!Directory.Exists(Path.GetDirectoryName(file))) throw new FileNotFoundException($" File \"{file}\" not found.", file);
            int count = meshes.Count();
            int size = (int)Math.Ceiling(Math.Sqrt(count));
            float fSize = (float)size * 2;

            Bitmap bitmap = new Bitmap(size * 2, size * 2);

            int i = 0;
            foreach (var mesh in meshes)
            {
                int x = (i % size) * 2;
                int y = (i / size) * 2;
                float fx = (float)x;
                float fy = (float)y;

                Point2f coord = new Point2f((fx + 0.5) / fSize, (fy + 0.5) / fSize);

                mesh.TextureCoordinates.Clear();
                for (int j = 0; j < mesh.Vertices.Count; j++)
                    mesh.TextureCoordinates.Add(coord);

                Color color = mesh.VertexColors[0];
                color = Color.FromArgb(255, color.R, color.G, color.B);

                y = size * 2 - 2 - y;
                bitmap.SetPixel(x + 0, y + 1, color);
                bitmap.SetPixel(x + 1, y + 1, color);
                bitmap.SetPixel(x + 1, y + 0, color);
                bitmap.SetPixel(x + 0, y + 0, color);
                i++;
            }

            bitmap.Save(file, System.Drawing.Imaging.ImageFormat.Png);
            return meshes;
        }

        public static Mesh SetTextureCoords(Mesh mesh, IEnumerable<Point3d> coords)
        {
            var coords2f = coords.Select(p => new Point2f(p.X, p.Y)).ToArray();

            mesh.ClearTextureData();
            mesh.TextureCoordinates.AddRange(coords2f);

            return mesh;
        }
    }
}