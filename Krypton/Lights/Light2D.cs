using System;
using System.Collections.Generic;
using Krypton.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Krypton.Lights
{
    public class Light2D : ILight2D
    {
        private Vector2 _position = Vector2.Zero;
        private float _fov = MathHelper.TwoPi;
        private float _intensity = 1;

        public bool IsOn { get; set; } = true;

        public ShadowType ShadowType { get; set; } = ShadowType.Solid;

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        
        public float X
        {
            get { return _position.X; }
            set { _position.X = value; }
        }
        
        public float Y
        {
            get { return _position.Y; }
            set { _position.Y = value; }
        }
        
        public float Angle { get; set; } = 0;

        public Texture2D Texture { get; set; } = null;

        public Color Color { get; set; } = Color.White;

        public float Range { get; set; } = 1;

        public float Fov
        {
            get { return _fov; }
            set { _fov = MathHelper.Clamp(value, 0, MathHelper.TwoPi); }
        }
        
        public float Intensity
        {
            get { return _intensity; }
            set { _intensity = MathHelper.Clamp(value, 0.01f, 3f); }
        }

        public void Draw(
            KryptonRenderHelper helper,
            IList<ShadowHull> shadowHulls)
        {
            if (!IsOn)
            {
                return;
            }

            // Make sure we only render the following shadowHulls
            helper.ShadowHullVertices.Clear();
            helper.ShadowHullIndicies.Clear();

            // Loop through each hull
            foreach (var hull in shadowHulls)
            {
                if (!hull.Visible)
                {
                    continue;
                }

                if (!IsHullWithinRange(hull))
                {
                    continue;
                }
                
                helper.BufferAddShadowHull(hull);
            }

            // Set the effect and parameters
            helper.Effect.Parameters["LightPosition"]
                .SetValue(_position);

            helper.Effect.Parameters["Texture0"]
                .SetValue(Texture);

            helper.Effect.Parameters["LightIntensityFactor"]
                .SetValue(1 / (_intensity * _intensity));
            
            switch (ShadowType)
            {
                case ShadowType.Solid:
                    helper.Effect.CurrentTechnique = helper.Effect.Techniques["PointLight_Shadow_Solid"];
                    break;

                case ShadowType.Illuminated:
                    helper.Effect.CurrentTechnique = helper.Effect.Techniques["PointLight_Shadow_Illuminated"];
                    break;

                case ShadowType.Occluded:
                    helper.Effect.CurrentTechnique = helper.Effect.Techniques["PointLight_Shadow_Occluded"];
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(ShadowType),
                        $"Shadow Type does not exist: {ShadowType}");
            }

            foreach (var pass in helper.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                helper.BufferDraw();
            }

            helper.Effect.CurrentTechnique = helper.Effect.Techniques["PointLight_Light"];

            foreach (var pass in helper.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                helper.DrawClippedFov(
                    _position,
                    Angle,
                    Range * 2,
                    Color,
                    _fov);
            }

            helper.Effect.CurrentTechnique = helper.Effect.Techniques["ClearTarget_Alpha"];

            foreach (var pass in helper.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                helper.GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleStrip,
                    KryptonRenderHelper.UnitQuad,
                    vertexOffset: 0,
                    primitiveCount: 2);
            }
        }

        private bool IsHullWithinRange(ShadowHull hull)
        {
            var offset = hull.Position - Position;
            var distance = hull.MaxRadius*Math.Max(hull.Scale.X, hull.Scale.Y) + Range;
            return offset.X*offset.X + offset.Y*offset.Y < distance*distance;
        }
        
        public BoundingRect Bounds
        {
            get
            {
                BoundingRect rect;

                rect.Min.X = _position.X - Range;
                rect.Min.Y = _position.Y - Range;
                rect.Max.X = _position.X + Range;
                rect.Max.Y = _position.Y + Range;

                return rect;
            }
        }
    }
}
