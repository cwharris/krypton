using System.Linq;
using Krypton.Components;
using Krypton.Design;
using Krypton.Lights;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Krypton.Testbed
{
    public class LightmapDebuggerComponent : DrawableGameComponent
    {
        private readonly LightmapGeneratorComponent _lightmapGenerator;

        private SpriteBatch _batch;

        private Texture2D _texture;

        public LightmapDebuggerComponent(Game game, LightmapGeneratorComponent lightmapGenerator)
            : base(game)
        {
            _lightmapGenerator = lightmapGenerator;
        }

        public override void Draw(GameTime gameTime)
        {
            _batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            foreach (var pass in _lightmapGenerator.Passes)
            {
                DrawPass(pass);
            }

            _batch.End();
        }

        protected override void LoadContent()
        {
            _batch = new SpriteBatch(GraphicsDevice);
            _texture = Game.Content.Load<Texture2D>("Krypton/Debug/tex");
        }

        private void DrawPass(ILightmapPass pass)
        {
            foreach (var light in _lightmapGenerator.Lights)
            {
                var pointLight = light as PointLight;

                if (pointLight == null)
                {
                    continue;
                }

                DrawLight(pass, pointLight);
            }

            foreach (var hull in _lightmapGenerator.Hulls)
            {
                DrawHull(pass, hull);
            }
        }

        private void DrawHull(
            ILightmapPass pass,
            IShadowHull shadowHull)
        {
        }

        private void DrawLight(
            ILightmapPass pass,
            PointLight light)
        {
            var v = Vector2.Transform(new Vector2(light.Position.X, light.Position.Y), pass.Matrix);
            v = ScreenToPixel(pass, v);

            Vector2 v1, v2, v3, v4;

            GraphicsDevice.ScissorRectangle= GetViewport(pass, light, out v2, out v1, out v4, out v3);

            _batch.Draw(_texture, v, null, Color.White, 0, Vector2.One * 8, Vector2.One, SpriteEffects.None, 0);
            _batch.Draw(_texture, v1, null, Color.White, 0, Vector2.One * 8, Vector2.One / 2, SpriteEffects.None, 0);
            _batch.Draw(_texture, v2, null, Color.White, 0, Vector2.One * 8, Vector2.One / 2, SpriteEffects.None, 0);
            _batch.Draw(_texture, v3, null, Color.White, 0, Vector2.One * 8, Vector2.One / 2, SpriteEffects.None, 0);
            _batch.Draw(_texture, v4, null, Color.White, 0, Vector2.One * 8, Vector2.One / 2, SpriteEffects.None, 0);
        }

        private static Rectangle GetViewport(
            ILightmapPass pass,
            PointLight light,
            out Vector2 v2,
            out Vector2 v1,
            out Vector2 v4,
            out Vector2 v3)
        {
            v1 = Vector2.Transform(new Vector2(light.Position.X + light.Radius, light.Position.Y + light.Radius), pass.Matrix);
            v2 = Vector2.Transform(new Vector2(light.Position.X - light.Radius, light.Position.Y + light.Radius), pass.Matrix);
            v3 = Vector2.Transform(new Vector2(light.Position.X - light.Radius, light.Position.Y - light.Radius), pass.Matrix);
            v4 = Vector2.Transform(new Vector2(light.Position.X + light.Radius, light.Position.Y - light.Radius), pass.Matrix);

            v1 = ScreenToPixel(pass, v1);
            v2 = ScreenToPixel(pass, v2);
            v3 = ScreenToPixel(pass, v3);
            v4 = ScreenToPixel(pass, v4);

            var min = new[] { v1, v2, v3, v4 }.Aggregate(Vector2.Min);
            var max = new[] { v1, v2, v3, v4 }.Aggregate(Vector2.Max);

            var rect = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));

            return Rectangle.Intersect(rect, pass.Viewport.Bounds);
        }

        private static Vector2 ScreenToPixel(
            ILightmapPass pass,
            Vector2 position)
        {
            position.X = ((1 + position.X) / 2) * pass.Viewport.Width + pass.Viewport.X;
            position.Y = ((1 - position.Y) / 2) * pass.Viewport.Height + pass.Viewport.Y;

            return position;
        }
    }
}
