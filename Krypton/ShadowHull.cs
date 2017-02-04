using System;
using System.Collections.Generic;
using System.Linq;
using Krypton.Common;
using Krypton.Design;
using Microsoft.Xna.Framework;

namespace Krypton
{
    public class ShadowHull : IShadowHull
    {
        public static readonly Color ShadowBlack = new Color(0, 0, 0, 0);

        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public float Angle { get; set; }
        public float RadiusSquared { get; }

        private readonly HullVertex[] _vertices;
        private readonly int[] _indices;
        private Matrix _normalMatrix = Matrix.Identity;
        private Matrix _vertexMatrix = Matrix.Identity;
        private float _cos;
        private float _sin;
        private HullVertex _hullVertex;
        private HullVertex _point;

        public static ShadowHull Create(IList<Vector2> points)
        {
            return new ShadowHull();
        }

        private ShadowHull(
            HullVertex[] vertices,
            int[] indicies,
            float radiusSquared)
        {
            _vertices = vertices;
            _indices = indicies;
            RadiusSquared = radiusSquared;
        }

        public ShadowHull(params Vector2[] points) :
            this((IList<Vector2>) points)
        {
        }

        public ShadowHull(IList<Vector2> points)
        {
            var numVertices = points.Count * 2;
            var numTris = numVertices - 2;
            var numIndicies = numTris * 3;

            _vertices = new HullVertex[numVertices];
            _indices = new int[numIndicies];

            for (var i = 0; i < points.Count; i++)
            {
                var p1 = points[i];
                var p2 = points[(i + 1) % points.Count];

                var normal = (p2 - p1).Clockwise();

                normal.Normalize();

                _vertices[i * 2] =
                    new HullVertex(
                        position: p1,
                        normal: normal,
                        color: new Color(0, 0, 0, 0.1f));

                _vertices[i * 2 + 1] =
                    new HullVertex(
                        position: p2,
                        normal: normal,
                        color: new Color(0, 0, 0, 0.1f));
            }

            for (var i = 0; i < numTris; i++)
            {
                _indices[i * 3] = 0;
                _indices[i * 3 + 1] = i + 1;
                _indices[i * 3 + 2] = i + 2;
            }

            RadiusSquared = points.Max(x => x.LengthSquared());
        }

        /// <summary>
        /// Draws the shadowHull.
        /// </summary>
        /// <param name="drawContext">The Lightmap Draw Buffer</param>
        public void Draw(ILightmapDrawContext drawContext)
        {
            // Create the matrices (3X speed boost versus prior version)
            _cos = (float) Math.Cos(Angle);
            _sin = (float) Math.Sin(Angle);

            // vertexMatrix = scale * rotation * translation;
            _vertexMatrix.M11 = Scale.X * _cos;
            _vertexMatrix.M12 = Scale.X * _sin;
            _vertexMatrix.M21 = Scale.Y * -_sin;
            _vertexMatrix.M22 = Scale.Y * _cos;
            _vertexMatrix.M41 = Position.X;
            _vertexMatrix.M42 = Position.Y;

            // normalMatrix = scaleInv * rotation;
            _normalMatrix.M11 = (1f / Scale.X) * _cos;
            _normalMatrix.M12 = (1f / Scale.X) * _sin;
            _normalMatrix.M21 = (1f / Scale.Y) * -_sin;
            _normalMatrix.M22 = (1f / Scale.Y) * _cos;

            drawContext.SetStartVertex();

            // Add the vertices to the buffer
            var hullVerticesLength = _vertices.Length;

            for (var i = 0; i < hullVerticesLength; i++)
            {
                // Transform the vertices to world coordinates
                _point = _vertices[i];

                Vector2.Transform(
                    ref _point.Position,
                    ref _vertexMatrix,
                    out _hullVertex.Position);

                Vector2.TransformNormal(
                    ref _point.Normal,
                    ref _normalMatrix,
                    out _hullVertex.Normal);

                _hullVertex.Color = ShadowBlack;

                drawContext.AddVertex(_hullVertex);
            }

            var hullIndicesLength = _indices.Length;

            for (var i = 0; i < hullIndicesLength; i++)
            {
                drawContext.AddIndex(_indices[i]);
            }
        }
    }
}
