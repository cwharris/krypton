using System;
using Microsoft.Xna.Framework;

namespace Krypton.Hull
{
    internal class ShadowHullVertexApplicator
    {
        private float _cos;
        private float _sin;

        private Matrix _vertexMatrix = Matrix.Identity;
        private Matrix _normalMatrix = Matrix.Identity;

        private static ShadowHullVertex _shadowHullVertex;

        public void PrepareForDraw(
            Vector2 position,
            Vector2 scale,
            float angle)
        {
            // Create the matrices (3X speed boost versus prior version)
            _cos = (float) Math.Cos(angle);
            _sin = (float) Math.Sin(angle);

            // vertexMatrix = scale * rotation * translation;
            _vertexMatrix.M11 = scale.X*_cos;
            _vertexMatrix.M12 = scale.X*_sin;
            _vertexMatrix.M21 = scale.Y*-_sin;
            _vertexMatrix.M22 = scale.Y*_cos;
            _vertexMatrix.M41 = position.X;
            _vertexMatrix.M42 = position.Y;

            // normalMatrix = scaleInv * rotation;
            _normalMatrix.M11 = (1f/scale.X)*_cos;
            _normalMatrix.M12 = (1f/scale.X)*_sin;
            _normalMatrix.M21 = (1f/scale.Y)*-_sin;
            _normalMatrix.M22 = (1f/scale.Y)*_cos;
        }

        internal void Apply(
            ShadowHullVertex[] vertices,
            Color color,
            Action<ShadowHullVertex> addShadowHullVertex)
        {
            // Add the vertices to the buffer
            var hullVerticesLength = vertices.Length;

            for (var i = 0; i < hullVerticesLength; i++)
            {
                // Transform the vertices to world coordinates
                _shadowHullVertex = vertices[i];

                Vector2.Transform(
                    ref _shadowHullVertex.Position,
                    ref _vertexMatrix,
                    out _shadowHullVertex.Position);

                Vector2.TransformNormal(
                    ref _shadowHullVertex.Normal,
                    ref _normalMatrix,
                    out _shadowHullVertex.Normal);

                _shadowHullVertex.Color = color;

                addShadowHullVertex(_shadowHullVertex);
            }
        }
    }
}