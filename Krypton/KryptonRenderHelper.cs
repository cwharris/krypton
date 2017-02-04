using System;
using System.Collections.Generic;
using Krypton.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Krypton
{
    public class KryptonRenderHelper
    {
        public static readonly VertexPositionTexture[] UnitQuad =
        {
            new VertexPositionTexture
            {
                Position = new Vector3(-1, 1, 0),
                TextureCoordinate = new Vector2(0, 0),
            },
            new VertexPositionTexture
            {
                Position = new Vector3(1, 1, 0),
                TextureCoordinate = new Vector2(1, 0),
            },
            new VertexPositionTexture
            {
                Position = new Vector3(-1, -1, 0),
                TextureCoordinate = new Vector2(0, 1),
            },
            new VertexPositionTexture
            {
                Position = new Vector3(1, -1, 0),
                TextureCoordinate = new Vector2(1, 1),
            },
        };

        public GraphicsDevice GraphicsDevice { get; }

        public Effect Effect { get; }

        public List<ShadowHullVertex> ShadowHullVertices { get; } = new List<ShadowHullVertex>();

        public List<int> ShadowHullIndicies { get; } = new List<int>();

        public KryptonRenderHelper(
            GraphicsDevice graphicsDevice,
            Effect effect)
        {
            GraphicsDevice = graphicsDevice;
            Effect = effect;
        }

        public void BufferAddShadowHull(ShadowHull hull)
        {
            // Why do we need all of these again? (hint: we don't)

            var vertexMatrix = Matrix.Identity;
            var normalMatrix = Matrix.Identity;

            // Create the matrices (3X speed boost versus prior version)
            var cos = (float) Math.Cos(hull.Angle);
            var sin = (float) Math.Sin(hull.Angle);

            // vertexMatrix = scale * rotation * translation;
            vertexMatrix.M11 = hull.Scale.X*cos;
            vertexMatrix.M12 = hull.Scale.X*sin;
            vertexMatrix.M21 = hull.Scale.Y*-sin;
            vertexMatrix.M22 = hull.Scale.Y*cos;
            vertexMatrix.M41 = hull.Position.X;
            vertexMatrix.M42 = hull.Position.Y;

            // normalMatrix = scaleInv * rotation;
            normalMatrix.M11 = 1f/hull.Scale.X*cos;
            normalMatrix.M12 = 1f/hull.Scale.X*sin;
            normalMatrix.M21 = 1f/hull.Scale.Y*-sin;
            normalMatrix.M22 = 1f/hull.Scale.Y*cos;

            // Where are we in the buffer?
            var vertexCount = ShadowHullVertices.Count;

            // Add the vertices to the buffer
            for (var i = 0; i < hull.NumPoints; i++)
            {
                // Transform the vertices to screen coordinates
                var point = hull.Points[i];

                ShadowHullVertex hullVertex;

                Vector2.Transform(
                    ref point.Position,
                    ref vertexMatrix,
                    out hullVertex.Position);

                Vector2.TransformNormal(
                    ref point.Normal,
                    ref normalMatrix,
                    out hullVertex.Normal);

                hullVertex.Color = new Color(
                    r: 0,
                    g: 0,
                    b: 0,
                    a: 1 - hull.Opacity);

                ShadowHullVertices.Add(hullVertex);
            }

            foreach (var index in hull.Indicies)
            {
                ShadowHullIndicies.Add(vertexCount + index);
            }
        }

        public void DrawSquareQuad(
            Vector2 position,
            float rotation,
            float size,
            Color color)
        {
            size /= 2;

            size = (float) Math.Sqrt(Math.Pow(size, 2) + Math.Pow(size, 2));

            rotation += (float) Math.PI/4;

            var cos = (float) Math.Cos(rotation)*size;
            var sin = (float) Math.Sin(rotation)*size;

            var v1 = new Vector3(+cos, +sin, 0) + new Vector3(position, 0);
            var v2 = new Vector3(-sin, +cos, 0) + new Vector3(position, 0);
            var v3 = new Vector3(-cos, -sin, 0) + new Vector3(position, 0);
            var v4 = new Vector3(+sin, -cos, 0) + new Vector3(position, 0);

            var quad = new VertexPositionColorTexture[]
            {
                new VertexPositionColorTexture()
                {
                    Position = v2,
                    Color = color,
                    TextureCoordinate = new Vector2(0, 0),
                },
                new VertexPositionColorTexture()
                {
                    Position = v1,
                    Color = color,
                    TextureCoordinate = new Vector2(1, 0),
                },
                new VertexPositionColorTexture()
                {
                    Position = v3,
                    Color = color,
                    TextureCoordinate = new Vector2(0, 1),
                },
                new VertexPositionColorTexture()
                {
                    Position = v4,
                    Color = color,
                    TextureCoordinate = new Vector2(1, 1),
                },
            };

            GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleStrip, quad, 0, 2);
        }

        public void DrawClippedFov(Vector2 position, float rotation, float size, Color color, float fov)
        {
            fov = MathHelper.Clamp(fov, 0, MathHelper.TwoPi);

            if (fov == 0)
            {
                return;
            }

            if (fov == MathHelper.TwoPi)
            {
                DrawSquareQuad(position, rotation, size, color);
                return;
            }

            var ccw = ClampToBox(fov/2);
            var cw = ClampToBox(-fov/2);

            var ccwTex = new Vector2(ccw.X + 1, -ccw.Y + 1)/2f;
            var cwTex = new Vector2(cw.X + 1, -cw.Y + 1)/2f;

            var vertices = new[]
            {
                new VertexPositionColorTexture
                {
                    Position = Vector3.Zero,
                    Color = color,
                    TextureCoordinate = new Vector2(0.5f, 0.5f),
                },
                new VertexPositionColorTexture
                {
                    Position = new Vector3(ccw, 0),
                    Color = color,
                    TextureCoordinate = ccwTex
                },
                new VertexPositionColorTexture
                {
                    Position = new Vector3(-1, 1, 0),
                    Color = color,
                    TextureCoordinate = new Vector2(0, 0),
                },
                new VertexPositionColorTexture
                {
                    Position = new Vector3(1, 1, 0),
                    Color = color,
                    TextureCoordinate = new Vector2(1, 0),
                },
                new VertexPositionColorTexture
                {
                    Position = new Vector3(1, -1, 0),
                    Color = color,
                    TextureCoordinate = new Vector2(1, 1),
                },
                new VertexPositionColorTexture
                {
                    Position = new Vector3(-1, -1, 0),
                    Color = color,
                    TextureCoordinate = new Vector2(0, 1),
                },
                new VertexPositionColorTexture
                {
                    Position = new Vector3(cw, 0),
                    Color = color,
                    TextureCoordinate = cwTex,
                },
            };

            var r = Matrix.CreateRotationZ(rotation)*
                    Matrix.CreateScale(size/2)*
                    Matrix.CreateTranslation(new Vector3(position, 0));

            for (var i = 0; i < vertices.Length; i++)
            {
                var vertex = vertices[i];

                Vector3.Transform(
                    ref vertex.Position,
                    ref r,
                    out vertex.Position);

                vertices[i] = vertex;
            }

            var indicies = GetIndicies(fov);

            GraphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.TriangleList,
                vertices,
                0,
                vertices.Length,
                indicies,
                0,
                indicies.Length/3);
        }

        private static int[] GetIndicies(float fov)
        {
            if (fov <= MathHelper.Pi/2)
            {
                return new[]
                {
                    0, 1, 6,
                };
            }

            if (fov <= 3*MathHelper.Pi/2)
            {
                return new[]
                {
                    0, 1, 3,
                    0, 3, 4,
                    0, 4, 6,
                };
            }

            return new[]
            {
                0, 1, 2,
                0, 2, 3,
                0, 3, 4,
                0, 4, 5,
                0, 5, 6,
            };
        }

        public static Vector2 ClampToBox(float angle)
        {
            var x = Math.Cos(angle);
            var y = Math.Sin(angle);
            var absMax = Math.Max(Math.Abs(x), Math.Abs(y));

            return new Vector2(
                (float) (x/absMax),
                (float) (y/absMax));
        }

        public void BufferDraw()
        {
            if (ShadowHullIndicies.Count >= 3)
            {
                GraphicsDevice.DrawUserIndexedPrimitives(
                    primitiveType: PrimitiveType.TriangleList,
                    vertexData: ShadowHullVertices.ToArray(),
                    vertexOffset: 0,
                    numVertices: ShadowHullVertices.Count,
                    indexData: ShadowHullIndicies.ToArray(),
                    indexOffset: 0,
                    primitiveCount: ShadowHullIndicies.Count/3);
            }
        }

        public void DrawFullscreenQuad()
        {
            // Obtain the original rendering states
            //var originalRastkerizerState = GraphicsDevice.RasterizerState;

            // Draw the quad
            Effect.CurrentTechnique = Effect.Techniques["ScreenCopy"];
            //_graphicsDevice.RasterizerState = RasterizerState.CullNone;

            var texelBias = new Vector2(
                0.5f/GraphicsDevice.Viewport.Width,
                0.5f/GraphicsDevice.Viewport.Height);

            Effect.Parameters["TexelBias"]
                .SetValue(texelBias);

            foreach (var effectPass in Effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();

                GraphicsDevice.DrawUserPrimitives(
                    primitiveType: PrimitiveType.TriangleStrip,
                    vertexData: UnitQuad,
                    vertexOffset: 0,
                    primitiveCount: 2);
            }

            // Reset to the original rendering states
            //_graphicsDevice.RasterizerState = originalRasterizerState;
        }

        public void BlurTextureToTarget(Texture2D texture, LightMapSize mapSize, BlurTechnique blurTechnique,
            float bluriness)
        {
            // Get the pass to use
            var passName = "";

            switch (blurTechnique)
            {
                case BlurTechnique.Horizontal:
                    Effect.Parameters["BlurFactorU"]
                        .SetValue(1f/GraphicsDevice.PresentationParameters.BackBufferWidth);

                    passName = "HorizontalBlur";

                    break;

                case BlurTechnique.Vertical:
                    Effect.Parameters["BlurFactorV"]
                        .SetValue(1f/GraphicsDevice.PresentationParameters.BackBufferHeight);

                    passName = "VerticalBlur";

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(blurTechnique), blurTechnique, null);
            }

            var biasFactor = BiasFactorFromLightMapSize(mapSize);

            var texelBias = new Vector2
            {
                X = biasFactor/GraphicsDevice.Viewport.Width,
                Y = biasFactor/GraphicsDevice.Viewport.Height,
            };


            Effect.Parameters["Texture0"].SetValue(texture);
            Effect.Parameters["TexelBias"].SetValue(texelBias);
            Effect.Parameters["Bluriness"].SetValue(bluriness);
            Effect.CurrentTechnique = Effect.Techniques["Blur"];

            Effect.CurrentTechnique.Passes[passName].Apply();

            GraphicsDevice.DrawUserPrimitives(
                primitiveType: PrimitiveType.TriangleStrip,
                vertexData: UnitQuad,
                vertexOffset: 0,
                primitiveCount: 2);
        }

        public void DrawTextureToTarget(
            Texture2D texture,
            LightMapSize mapSize,
            BlendTechnique blend)
        {
            var techniqueName = "";

            switch (blend)
            {
                case BlendTechnique.Add:
                    techniqueName = "TextureToTarget_Add";
                    break;

                case BlendTechnique.Multiply:
                    techniqueName = "TextureToTarget_Multiply";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(blend), blend, null);
            }

            var biasFactor = BiasFactorFromLightMapSize(mapSize);

            var texelBias = new Vector2
            {
                X = biasFactor/GraphicsDevice.ScissorRectangle.Width,
                Y = biasFactor/GraphicsDevice.ScissorRectangle.Height,
            };

            Effect.Parameters["Texture0"].SetValue(texture);
            Effect.Parameters["TexelBias"].SetValue(texelBias);
            Effect.CurrentTechnique = Effect.Techniques[techniqueName];

            foreach (var effectPass in Effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                GraphicsDevice.DrawUserPrimitives(
                    primitiveType: PrimitiveType.TriangleStrip,
                    vertexData: UnitQuad,
                    vertexOffset: 0,
                    primitiveCount: 2);
            }
        }

        private static float BiasFactorFromLightMapSize(LightMapSize mapSize)
        {
            switch (mapSize)
            {
                case LightMapSize.Full:
                    return 0.5f;

                case LightMapSize.Fourth:
                    return 0.6f;

                case LightMapSize.Eighth:
                    return 0.7f;

                default:
                    return 0.0f;
            }
        }

        public void BufferAddBoundOutline(BoundingRect boundingRect)
        {
            var vertexCount = ShadowHullVertices.Count;

            ShadowHullVertices.Add(
                new ShadowHullVertex
                {
                    Color = Color.Black,
                    Normal = Vector2.Zero,
                    Position = new Vector2(
                        boundingRect.Left,
                        boundingRect.Top)
                });

            ShadowHullVertices.Add(
                new ShadowHullVertex
                {
                    Color = Color.Black,
                    Normal = Vector2.Zero,
                    Position = new Vector2(
                        boundingRect.Right,
                        boundingRect.Top)
                });

            ShadowHullVertices.Add(
                new ShadowHullVertex
                {
                    Color = Color.Black,
                    Normal = Vector2.Zero,
                    Position = new Vector2(
                        boundingRect.Right,
                        boundingRect.Bottom)
                });

            ShadowHullVertices.Add(
                new ShadowHullVertex
                {
                    Color = Color.Black,
                    Normal = Vector2.Zero,
                    Position = new Vector2(
                        boundingRect.Left,
                        boundingRect.Bottom)
                });

            ShadowHullIndicies.Add(vertexCount + 0);
            ShadowHullIndicies.Add(vertexCount + 1);
            ShadowHullIndicies.Add(vertexCount + 2);

            ShadowHullIndicies.Add(vertexCount + 0);
            ShadowHullIndicies.Add(vertexCount + 2);
            ShadowHullIndicies.Add(vertexCount + 3);
        }
    }
}
