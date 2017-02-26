using System;
using System.Collections.Generic;
using Krypton.Common;
using Microsoft.Xna.Framework;

namespace Krypton.Hull
{
    public class ShadowHullShape
    {
        public readonly ShadowHullVertex[] Vertices;

        private ShadowHullShape(params ShadowHullVertex[] vertices)
        {
            Vertices = vertices;
        }

        public static ShadowHullShape CreateConvex(IList<Vector2> points)
        {
            return CreateConvex(points, Vector2.Zero);
        }

        public static ShadowHullShape CreateConvex(IList<Vector2> points, Vector2 offset)
        {
            var numVertices = points.Count*2;
            var vertices = new ShadowHullVertex[numVertices];

            for (var i = 0; i < points.Count; i++)
            {
                var p1 = points[i];
                var p2 = points[(i + 1)%points.Count];

                var normal = (p2 - p1).Clockwise();

                normal.Normalize();

                vertices[i*2] =
                    new ShadowHullVertex(
                        position: offset + p1,
                        normal: normal,
                        color: new Color(0, 0, 0, 0.1f));

                vertices[i*2 + 1] =
                    new ShadowHullVertex(
                        position: offset + p2,
                        normal: normal,
                        color: new Color(0, 0, 0, 0.1f));
            }

            return new ShadowHullShape(vertices);
        }

        public static ShadowHullShape CreateRectangle(float width, float height)
        {
            return CreateRectangle(width, height, Vector2.Zero);
        }

        public static ShadowHullShape CreateRectangle(float width, float height, Vector2 offset)
        {
            return CreateConvex(
                new[]
                {
                    new Vector2(+width, +height),
                    new Vector2(-width, +height),
                    new Vector2(-width, -height),
                    new Vector2(+width, -height)
                },
                offset);
        }

        public static ShadowHullShape CreateCircle(
            float radius,
            int numSides)
        {
            return CreateCircle(radius, numSides, Vector2.Zero);
        }

        public static ShadowHullShape CreateCircle(
            float radius,
            int numSides,
            Vector2 offset)
        {
            var vertices = new Vector2[numSides];

            var angle = Math.PI * 2 / numSides;

            for (var i = 0; i < numSides; i++)
            {
                vertices[i] =
                    new Vector2(
                        (float) Math.Cos(i*angle)*radius,
                        (float) Math.Sin(i*angle)*radius);
            }

            return CreateConvex(vertices, offset);
        }
    }
}