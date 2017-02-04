using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Krypton
{
    public class LightmapEffect
    {
        public LightmapEffect(Effect effect)
        {
            Effect = effect;
        }
        
        public Effect Effect { get; }

        public EffectTechnique SolidShadowTechnique => Effect.Techniques["PointLight_Shadow_Solid"];
        public EffectTechnique IlluminatedShadowTechnique => Effect.Techniques["PointLight_Shadow_Illuminated"];
        public EffectTechnique OccludedShadowTechnique => Effect.Techniques["PointLight_Shadow_Occluded"];
        public EffectTechnique LightTechnique => Effect.Techniques["PointLight_Light"];
        public EffectTechnique AlphaClearTechnique => Effect.Techniques["ClearTarget_Alpha"];

        public Texture2D LightTexture
        {
            get { return Effect.Parameters["Texture0"].GetValueTexture2D(); }
            set { Effect.Parameters["Texture0"].SetValue(value); }
        }

        public Vector2 LightPosition
        {
            get { return Effect.Parameters["LightPosition"].GetValueVector2(); }
            set { Effect.Parameters["LightPosition"].SetValue(value); }
        }

        public float LightInesityFactor
        {
            get { return Effect.Parameters["LightIntensityFactor"].GetValueSingle(); }
            set { Effect.Parameters["LightIntensityFactor"].SetValue(value); }
        }

        public Matrix Matrix
        {
            set { Effect.Parameters["Matrix"].SetValue(value); }
        }

        public EffectTechnique CurrentTechnique
        {
            get { return Effect.CurrentTechnique; }
            set { Effect.CurrentTechnique = value; }
        }
    }
}
