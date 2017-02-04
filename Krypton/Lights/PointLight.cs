using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Krypton.Common;
using Krypton.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Krypton.Lights
{
    [DebuggerDisplay("Solid: {Position} {Radius} {Color} {Intensity}")]
    public class PointLight : ILight
    {
        private float _radius;
        
        public bool On { get; set; }

        public IEnumerable<Vector2> Outline => null;
        public Color Color { get; set; }
        public float RadiusSquared { get; private set; }
        public Texture2D Texture { get; }
        public Vector2 Position { get; set; }
        public float Intensity { get; set; }
        public float IntensityFactor => 1 / (Intensity * Intensity);
        public ShadowType ShadowType { get; set; }

        public float Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                RadiusSquared = value*value;
            }
        }

        public PointLight(Texture2D texture)
        {
            On = true;
            Texture = texture;
            Intensity = 1;
            Radius = 1;
            Color = Color.White;
            ShadowType = ShadowType.Solid;
        }
        
        public void Draw(
            LightmapEffect lightmapEffect,
            ILightmapPass lightmapPass,
            ILightmapDrawContext helper,
            IEnumerable<IShadowHull> hulls)
        {
            lightmapEffect.Effect.GraphicsDevice.ScissorRectangle = lightmapPass.GetScissor(this);

            // lightmapEffect.Effect.GraphicsDevice.ScissorRectangle = new Rectangle(0, 0, 200, 200);

            // 1) Clear shadowHull buffers
            helper.Clear();

            // 2) Prepare to draw hulls
            var hullsToDraw = hulls.Where(x => Vector2.DistanceSquared(Position, x.Position) < RadiusSquared + x.RadiusSquared);

            foreach (var hull in hullsToDraw)
            {
                hull.Draw(helper);
            }

            // 3) Set lightmapEffect stuff
            lightmapEffect.LightPosition = Position;
            lightmapEffect.LightTexture = Texture;
            lightmapEffect.LightInesityFactor = IntensityFactor;

            switch (ShadowType)
            {
                case ShadowType.Illuminated:
                    lightmapEffect.CurrentTechnique = lightmapEffect.IlluminatedShadowTechnique;
                    break;
                case ShadowType.Occluded:
                    lightmapEffect.CurrentTechnique = lightmapEffect.OccludedShadowTechnique;
                    break;
                case ShadowType.Solid:
                    lightmapEffect.CurrentTechnique = lightmapEffect.SolidShadowTechnique;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // 4) Draw hulls for each shadow pass
            foreach (var pass in lightmapEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                helper.Draw();
            }

            // 5) Draw light for each light pass
            lightmapEffect.CurrentTechnique = lightmapEffect.LightTechnique;

            foreach (var pass in lightmapEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                helper.DrawClippedFov(
                    position: Position,
                    rotation: 0f,
                    size: Radius * 2,
                    color: Color,
                    fov: MathHelper.TwoPi);
            }

            // 6) Clear the target's alpha chanel
            lightmapEffect.CurrentTechnique = lightmapEffect.AlphaClearTechnique;

            //lightmapEffect.Effect.GraphicsDevice.ScissorRectangle = lightmapEffect.Effect.GraphicsDevice.Viewport.Bounds;

            foreach (var pass in lightmapEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                helper.DrawUnitQuad();
            }
        }
    }
}
