using System.Collections.Generic;
using System.Linq;
using Krypton.Common;
using Krypton.Design;
using Microsoft.Xna.Framework;

namespace Krypton.Hull
{
    public class ShadowHull : IShadowHull
    {
        public static readonly Color ShadowBlack = new Color(0, 0, 0, 0);

        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public float Angle { get; set; }
        public float RadiusSquared { get; }

        private readonly ShadowHullVertex[] _vertices;
        private readonly int[] _indices;

        private static readonly ShadowHullVertexApplicator ShadowHullVertexApplicator =
            new ShadowHullVertexApplicator();

        private ShadowHull(
            ShadowHullVertex[] vertices,
            int[] indicies)
        {
            _vertices = vertices;
            _indices = indicies;
            RadiusSquared = vertices.Max(x => x.Position.LengthSquared());
        }

        private ShadowHull(IList<Vector2> points)
        {
            var numVertices = points.Count*2;
            var numTris = numVertices - 2;
            var numIndicies = numTris*3;

            _vertices = new ShadowHullVertex[numVertices];
            _indices = new int[numIndicies];

            for (var i = 0; i < points.Count; i++)
            {
                var p1 = points[i];
                var p2 = points[(i + 1)%points.Count];

                var normal = (p2 - p1).Clockwise();

                normal.Normalize();

                _vertices[i*2] =
                    new ShadowHullVertex(
                        position: p1,
                        normal: normal,
                        color: new Color(0, 0, 0, 0.1f));

                _vertices[i*2 + 1] =
                    new ShadowHullVertex(
                        position: p2,
                        normal: normal,
                        color: new Color(0, 0, 0, 0.1f));
            }

            for (var i = 0; i < numTris; i++)
            {
                _indices[i*3] = 0;
                _indices[i*3 + 1] = i + 1;
                _indices[i*3 + 2] = i + 2;
            }

            RadiusSquared = points.Max(x => x.LengthSquared());
        }

        /// <summary>
        /// Draws the shadowHull.
        /// </summary>
        /// <param name="drawContext">The Lightmap DrawShadowHulls Buffer</param>
        public void Draw(IShadowHullDrawContext drawContext)
        {
            ShadowHullVertexApplicator.PrepareForDraw(
                position: Position,
                scale: Scale,
                angle: Angle);

            ShadowHullVertexApplicator.Apply(
                vertices: _vertices,
                color: ShadowBlack,
                addShadowHullVertex: drawContext.AddShadowHullVertex);

            var hullIndicesLength = _indices.Length;

            for (var i = 0; i < hullIndicesLength; i++)
            {
                drawContext.AddShadowHullIndex(_indices[i]);
            }
        }

        public static ShadowHull Create(params Vector2[] points)
        {
            return new ShadowHull(points);
        }

        public static ShadowHull Create(ShadowHullShape shape)
        {
            var vertices = shape.Vertices;
            var indices = GetIndiciesForShape(shape).ToArray();

            return new ShadowHull(
                vertices,
                indices);
        }

        public static ShadowHull Create(IList<ShadowHullShape> shapes)
        {
            var vertices = shapes
                .SelectMany(shape => shape.Vertices)
                .ToArray();

            var indices = GetIndiciesForShapes(shapes).ToArray();

            return new ShadowHull(
                vertices,
                indices);
        }

        private static IEnumerable<int> GetIndiciesForShape(ShadowHullShape shape)
        {
            var numTris = shape.Vertices.Length - 2;

            for (var i = 0; i < numTris; i++)
            {
                yield return 0;
                yield return i + 1;
                yield return i + 2;
            }
        }

        private static IEnumerable<int> GetIndiciesForShapes(IEnumerable<ShadowHullShape> shapes)
        {
            var start = 0;

            foreach (var shape in shapes)
            {
                var numTris = shape.Vertices.Length - 2;

                for (var i = 0; i < numTris; i++)
                {
                    yield return start;
                    yield return start + i + 1;
                    yield return start + i + 2;
                }

                start = shape.Vertices.Length;
            }
        }
    }
}
