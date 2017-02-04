using System;
using Microsoft.Xna.Framework;

namespace Krypton
{
    public class ShadowHull
    {
        public Vector2 Position;

        public float Angle;

        public float MaxRadius;

        public ShadowHullPoint[] Points;

        public int NumPoints;

        public int[] Indicies;

        public int NumIndicies;

        public bool Visible = true;

        public Vector2 Scale = Vector2.One;

        public float Opacity = 1f;

        private ShadowHull()
        {
        }

        public static ShadowHull CreateRectangle(Vector2 size)
        {
            var hull = new ShadowHull();

            size *= 0.5f;

            hull.MaxRadius = (float) Math.Sqrt(size.X*size.X + size.Y*size.Y);
            hull.NumPoints = 4*2;

            var numTris = hull.NumPoints - 2;
            hull.NumIndicies = numTris*3;

            hull.Points = new ShadowHullPoint[hull.NumPoints];
            hull.Indicies = new int[hull.NumIndicies];

            // Vertex position
            var posTr = new Vector2(+size.X, +size.Y);
            var posBr = new Vector2(+size.X, -size.Y);
            var posBl = new Vector2(-size.X, -size.Y);
            var posTl = new Vector2(-size.X, +size.Y);

            // Right
            hull.Points[0] = new ShadowHullPoint(posTr, Vector2.UnitX);
            hull.Points[1] = new ShadowHullPoint(posBr, Vector2.UnitX);

            // Bottom                                         
            hull.Points[2] = new ShadowHullPoint(posBr, -Vector2.UnitY);
            hull.Points[3] = new ShadowHullPoint(posBl, -Vector2.UnitY);

            // Left                                           
            hull.Points[4] = new ShadowHullPoint(posBl, -Vector2.UnitX);
            hull.Points[5] = new ShadowHullPoint(posTl, -Vector2.UnitX);

            // Top
            hull.Points[6] = new ShadowHullPoint(posTl, Vector2.UnitY);
            hull.Points[7] = new ShadowHullPoint(posTr, Vector2.UnitY);

            // Create tris
            for (var i = 0; i < numTris; i++)
            {
                hull.Indicies[i*3 + 0] = 0;
                hull.Indicies[i*3 + 1] = i + 1;
                hull.Indicies[i*3 + 2] = i + 2;
            }

            return hull;
        }

        /// <summary>
        /// Creates a circular shadow hull
        /// </summary>
        /// <param name="radius">radius of the circle</param>
        /// <param name="sides">number of sides the circle will be comprised of</param>
        /// <returns>A circular shadow hull</returns>
        public static ShadowHull CreateCircle(float radius, int sides)
        {
            if (sides < 3)
            {
                throw new ArgumentOutOfRangeException(nameof(sides), "Shadow hull must have at least 3 sides.");
            }

            var hull = new ShadowHull();

            hull.MaxRadius = radius;

            // Calculate number of sides
            hull.NumPoints = sides*2;
            var numTris = hull.NumPoints - 2;
            hull.NumIndicies = numTris*3;

            hull.Points = new ShadowHullPoint[hull.NumPoints];
            hull.Indicies = new int[hull.NumIndicies];

            var angle = (float) (-Math.PI*2)/sides; // XNA Renders Clockwise
            var angleOffset = angle/2;

            for (var i = 0; i < sides; i++)
            {
                // Create vertices
                var v1 = new ShadowHullPoint();
                var v2 = new ShadowHullPoint();

                // Vertex Position
                v1.Position.X = (float) Math.Cos(angle*i)*radius;
                v1.Position.Y = (float) Math.Sin(angle*i)*radius;

                v2.Position.X = (float) Math.Cos(angle*(i + 1))*radius;
                v2.Position.Y = (float) Math.Sin(angle*(i + 1))*radius;

                // Vertex Normal
                v1.Normal.X = (float) Math.Cos(angle*i + angleOffset);
                v1.Normal.Y = (float) Math.Sin(angle*i + angleOffset);

                v2.Normal.X = (float) Math.Cos(angle*i + angleOffset);
                v2.Normal.Y = (float) Math.Sin(angle*i + angleOffset);

                // Copy vertices
                hull.Points[i*2 + 0] = v1;
                hull.Points[i*2 + 1] = v2;
            }

            for (var i = 0; i < numTris; i++)
            {
                hull.Indicies[i*3 + 0] = 0;
                hull.Indicies[i*3 + 1] = i + 1;
                hull.Indicies[i*3 + 2] = i + 2;
            }

            return hull;
        }

        /// <summary>
        /// Creates a custom shadow hull based on a series of vertices
        /// </summary>
        /// <param name="points">The points which the shadow hull will be comprised of</param>
        /// <returns>A custom shadow hulll</returns>
        public static ShadowHull CreateConvex(ref Vector2[] points)
        {
            // Validate input
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points));
            }
            if (points.Length < 3)
            {
                throw new ArgumentOutOfRangeException(nameof(points), "Need at least 3 points to create shadow hull.");
            }

            var numPoints = points.Length;

            var hull = new ShadowHull();

            hull.NumPoints = numPoints*2;
            var numTris = hull.NumPoints - 2;
            hull.NumIndicies = numTris*3;

            hull.Points = new ShadowHullPoint[hull.NumPoints];
            hull.Indicies = new int[hull.NumIndicies];

            for (var i = 0; i < numPoints; i++)
            {
                var p1 = points[(i + 0)%numPoints];
                var p2 = points[(i + 1)%numPoints];

                hull.MaxRadius = Math.Max(hull.MaxRadius, p1.Length());

                var line = p2 - p1;

                var normal = new Vector2(-line.Y, +line.X);

                normal.Normalize();

                hull.Points[i*2 + 0] = new ShadowHullPoint(p1, normal);
                hull.Points[i*2 + 1] = new ShadowHullPoint(p2, normal);
            }

            for (var i = 0; i < numTris; i++)
            {
                hull.Indicies[i*3 + 0] = 0;
                hull.Indicies[i*3 + 1] = i + 1;
                hull.Indicies[i*3 + 2] = i + 2;
            }

            return hull;
        }
    }
}