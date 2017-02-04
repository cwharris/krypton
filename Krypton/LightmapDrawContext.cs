using System;
using System.Collections.Generic;
using Krypton.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Krypton
{
    public class LightmapDrawContext : ILightmapDrawContext
    {
        private static readonly VertexPositionTexture[] UnitQuad =
        {
            new VertexPositionTexture(new Vector3(-1, +1, 0), new Vector2(0, 0)),
            new VertexPositionTexture(new Vector3(+1, +1, 0), new Vector2(1, 0)),
            new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)),
            new VertexPositionTexture(new Vector3(+1, -1, 0), new Vector2(1, 1)),
        };

        private readonly GraphicsDevice _device;
        private readonly List<HullVertex> _vertices = new List<HullVertex>();
        private readonly int[] _indices = new int[1024 * 1024];
        private int _numIndicies;
        private int _startVertex;
        private VertexPositionColorTexture[] _clippedFovVertices;

        public LightmapDrawContext(GraphicsDevice device)
        {
            _device = device;
        }
        
        public void ClearShadowHulls()
        {
            _vertices.Clear();
            _numIndicies = 0;
        }

        public void PrepareToDrawNextShadowHull()
        {
            _startVertex = _vertices.Count;
        }

        public void AddShadowHullVertex(HullVertex hullVertex)
        {
            _vertices.Add(hullVertex);
        }

        public void AddShadowHullIndex(int index)
        {
            _indices[_numIndicies++] = index + _startVertex;
        }

        public void DrawShadowHulls()
        {
            if (_numIndicies < 3)
            {
                return;
            }

            _device.DrawUserIndexedPrimitives(
                primitiveType: PrimitiveType.TriangleList,
                vertexData: _vertices.ToArray(),
                vertexOffset: 0,
                numVertices: _vertices.Count,
                indexData: _indices,
                indexOffset: 0,
                primitiveCount: _numIndicies / 3);
        }

        public void DrawUnitQuad()
        {
            _device.DrawUserPrimitives(
                primitiveType: PrimitiveType.TriangleStrip,
                vertexData: UnitQuad,
                vertexOffset: 0,
                primitiveCount: 2);
        }
        
        public void DrawClippedFov(
            Vector2 position,
            float rotation,
            float size,
            Color color,
            float fov)
        {
            if (fov <= 0)
            {
                return;
            }

            if (fov >= MathHelper.TwoPi)
            {
                DrawSquareQuad(position, rotation, size, color);
                return;
            }

            fov = MathHelper.Clamp(fov, 0, MathHelper.TwoPi);

            var ccw = ClampToUnitSquare(fov / 2);
            var cw = ClampToUnitSquare(-fov / 2);

            var texCcw = new Vector2(ccw.X + 1, -ccw.Y + 1) / 2;
            var texCw = new Vector2(cw.X + 1, -cw.Y + 1) / 2;

            _clippedFovVertices = new[]
            {
                new VertexPositionColorTexture(Vector3.Zero, color, new Vector2(0.5f, 0.5f)),
                new VertexPositionColorTexture(new Vector3(ccw, 0), color, texCcw),
                new VertexPositionColorTexture(new Vector3(-1, +1, 0), color, new Vector2(0, 0)),
                new VertexPositionColorTexture(new Vector3(+1, +1, 0), color, new Vector2(1, 0)),
                new VertexPositionColorTexture(new Vector3(+1, -1, 0), color, new Vector2(1, 1)),
                new VertexPositionColorTexture(new Vector3(-1, -1, 0), color, new Vector2(0, 1)),
                new VertexPositionColorTexture(new Vector3(cw, 0), color, texCw),
            };

            var matrix =
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateScale(size / 2) *
                Matrix.CreateTranslation(new Vector3(position, 0));

            for (var i = 0; i < _clippedFovVertices.Length; i++)
            {
                var vertex = _clippedFovVertices[i];

                Vector3.Transform(
                    ref vertex.Position,
                    ref matrix,
                    out vertex.Position);

                _clippedFovVertices[i] = vertex;
            }

            var clippedFovIndices = GetClippedFovIndicies(fov);

            _device.DrawUserIndexedPrimitives(
                primitiveType: PrimitiveType.TriangleList,
                vertexData: _clippedFovVertices,
                vertexOffset: 0,
                numVertices: _clippedFovVertices.Length,
                indexData: clippedFovIndices,
                indexOffset: 0,
                primitiveCount: clippedFovIndices.Length / 3);
        }
        
        private static int[] GetClippedFovIndicies(float fov)
        {
            if (fov <= MathHelper.Pi / 2)
            {
                return new[] {0, 1, 6};
            }

            if (fov <= 3 * MathHelper.Pi / 2)
            {
                return new[] {0, 1, 3, 0, 3, 4, 0, 4, 6,};
            }

            return new[] {0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5, 0, 5, 6};
        }
        
        private static Vector2 ClampToUnitSquare(float angle)
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);

            var absMax = Math.Max(Math.Abs(cos), Math.Abs(sin));

            return new Vector2(
                (float) (cos/absMax),
                (float) (sin/absMax));
        }
        
        private void DrawSquareQuad(
            Vector2 position,
            float rotation,
            float size,
            Color color)
        {
            size = (size/2)*(float) Math.Sqrt(2); // sqrt(x^2 + y^2); where (x = size / 2) and (y = size/2)

            rotation += (float)Math.PI / 4;

            var cos = (float)Math.Cos(rotation) * size;
            var sin = (float)Math.Sin(rotation) * size;

            var v1 = new Vector3(+cos + position.X, +sin + position.Y, 0);
            var v2 = new Vector3(-sin + position.X, +cos + position.Y, 0);
            var v3 = new Vector3(-cos + position.X, -sin + position.Y, 0);
            var v4 = new Vector3(+sin + position.X, -cos + position.Y, 0);

            var quad = new[]
            {
                new VertexPositionColorTexture(v2, color, new Vector2(0, 0)),
                new VertexPositionColorTexture(v1, color, new Vector2(1, 0)),
                new VertexPositionColorTexture(v3, color, new Vector2(0, 1)),
                new VertexPositionColorTexture(v4, color, new Vector2(1, 1)),
            };

            _device.DrawUserPrimitives(
                primitiveType: PrimitiveType.TriangleStrip,
                vertexData: quad,
                vertexOffset: 0,
                primitiveCount: 2);
        }
    }
}
