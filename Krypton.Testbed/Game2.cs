using System;
using System.Collections.Generic;
using Krypton.Components;
using Krypton.Design;
using Krypton.Factories;
using Krypton.Fluent;
using Krypton.Lights;
using Krypton.Testbed.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Krypton.Testbed
{
    public class Game2 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly LightmapGeneratorComponent _lightmapGenerator;
        private readonly LightmapPresenterComponent _lightmapPresenter;
        private readonly BackBufferSlider _backBufferSlider;
        private PointLight _light;
        private ShadowHull _shadowHull;
        private GameTime _time;
        
        public Game2()
        {
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";

            Window.AllowUserResizing = true;

            _lightmapGenerator = new LightmapGeneratorComponent(this, Passes);
            _lightmapPresenter = new LightmapPresenterComponent(this, _lightmapGenerator);

            _backBufferSlider = new BackBufferSlider(this);

            Components.Add(_lightmapGenerator);
            Components.Add(_backBufferSlider);
            Components.Add(_lightmapPresenter);
        }

        protected IEnumerable<ILightmapPass> Passes
        {
            get
            {
                var w = GraphicsDevice.Viewport.Width;
                var h = GraphicsDevice.Viewport.Height;

                var aspect = (float)w/h;

                const float verticalUnits = 100f;

                yield return
                    new LightmapPass(
                        GraphicsDevice.Viewport,
                        Matrix.CreateOrthographic(
                            verticalUnits*aspect,
                            verticalUnits, 0, 1));
            }
        }

        protected override void LoadContent()
        {
            _shadowHull = HullFactory.CreateRectangle(10, 10);

            var lightTexture = TextureFactory.CreatePoint(GraphicsDevice, 256);

            _light = new PointLight(lightTexture)
                {
                    Radius = 100,
                    Intensity = 0.5f,
                    Position = new Vector2(0, 0),
                    ShadowType = ShadowType.Illuminated,
                };

            _light.Position = Vector2.UnitX * 30;

            GenerateTestHulls();

            _lightmapGenerator.Hulls.Add(_shadowHull);
            _lightmapGenerator.Lights.Add(_light);
        }

        private void GenerateTestHulls()
        {
            for (var x = 0; x < 20; x++)
            {
                for (var y = 0; y < 20; y++)
                {
                    _lightmapGenerator.Hulls.Add(
                        HullFactory.CreateRectangle(2, 2)
                            .Position((x*20) - 10, (y*20) - 10));
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            _shadowHull.Angle = (float) Math.Sin(gameTime.TotalGameTime.TotalSeconds)*MathHelper.PiOver4;

            _time = gameTime;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _lightmapPresenter.Visible = Keyboard.GetState().IsKeyUp(Keys.L);

            base.Draw(gameTime);
        }
    }
}
