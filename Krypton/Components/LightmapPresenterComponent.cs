using Krypton.Common;
using Krypton.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Krypton.Components
{
    public class LightmapPresenterComponent : DrawableGameComponent
    {
        private readonly ILightmapProvider _lightmapProvider;
        private SpriteBatch _spriteBatch;
        private BlendState _blendState;

        public LightmapPresenterComponent(
            Game game,
            ILightmapProvider lightmapProvider) :
            base(game)
        {
            _lightmapProvider = lightmapProvider;
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.DrawToTarget(
                _blendState,
                _lightmapProvider.Lightmap);
        }
        
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _blendState = new BlendState
            {
                ColorBlendFunction = BlendFunction.Add,
                ColorDestinationBlend = Blend.SourceColor,
                ColorSourceBlend = Blend.Zero,
                AlphaBlendFunction = BlendFunction.Add,
                AlphaDestinationBlend = Blend.SourceAlpha,
                AlphaSourceBlend = Blend.Zero,
            };
        }
    }
}
