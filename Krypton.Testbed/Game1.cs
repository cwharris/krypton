using System;
using System.Collections.Generic;
using Krypton.Common;
using Krypton.Components;
using Krypton.Design;
using Krypton.Factories;
using Krypton.Fluent;
using Krypton.Lights;
using Krypton.Testbed.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Krypton.Testbed
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly LightmapGeneratorComponent _lightmapGenerator;
        private readonly LightmapPresenterComponent _lightmapPresenter;
        private readonly BackBufferSlider _backBufferSlider;
        
        private PointLight _light;
        private ShadowHull _shadowHull;
        private GameTime _time;
        
        public Game1()
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

                var aspect = (float)w / h;

                const float verticalUnits = 100f;

                var state = Keyboard.GetState();

                if (state.IsKeyUp(Keys.D1))
                {
                    yield return
                        new LightmapPass(
                            viewport: GraphicsDevice.Viewport,
                            matrix: Matrix.CreateRotationZ((float)_time.TotalGameTime.TotalSeconds/10) * Matrix.CreateOrthographic(verticalUnits * aspect, verticalUnits, 0, 1));
                }
                else
                {
                    var rowSize = h / 2;
                    var colSize = w / 2;

                    yield return
                        new LightmapPass(
                            new Viewport(0, 0, colSize, rowSize),
                            Matrix.CreateOrthographic(verticalUnits * aspect, verticalUnits, 0, 1));

                    yield return
                        new LightmapPass(
                            new Viewport(colSize, 0, colSize, rowSize),
                            Matrix.CreateOrthographic(verticalUnits * aspect, verticalUnits, 0, 1));

                    yield return
                        new LightmapPass(
                            new Viewport(0, rowSize, colSize, rowSize),
                            Matrix.CreateOrthographic(verticalUnits * aspect, verticalUnits, 0, 1));

                    yield return
                        new LightmapPass(
                            new Viewport(colSize, rowSize, colSize, rowSize),
                            Matrix.CreateOrthographic(verticalUnits * aspect, verticalUnits, 0, 1));
                }
            }
        }
        
        protected override void LoadContent()
        {
            _shadowHull = HullFactory.CreateRectangle(10, 10);

            var lightTexture = TextureFactory.CreatePoint(GraphicsDevice, 256);

            _light = new PointLight(lightTexture)
                {
                    Radius = 50,
                    Intensity = 0.5f,
                    Position = new Vector2(0, 0),
                };

            _light.Position = Vector2.UnitX * 30;

            var r = new Random();

            GenerateRandomHullsAndLights(lightTexture, r);

            GenerateTestHullsAndLights(lightTexture);

            _lightmapGenerator.Hulls.Add(_shadowHull);

            // this.lightmapGenerator.Lights.Add(this.light);
        }
        
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            _shadowHull.Angle = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds) * MathHelper.PiOver4;

            _time = gameTime;

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            _lightmapPresenter.Visible = Keyboard.GetState().IsKeyUp(Keys.L);

            base.Draw(gameTime);
        }
        
        private void GenerateTestHullsAndLights(Texture2D lightTexture)
        {
            AddTestLight(lightTexture, new Vector2(74, 42));
            AddTestLight(lightTexture, new Vector2(74, -42));
            AddTestLight(lightTexture, new Vector2(-74, 42));
            AddTestLight(lightTexture, new Vector2(-74, -42));

            AddTestLight(lightTexture, new Vector2(15, 0));
            AddTestLight(lightTexture, new Vector2(-15, 0));
            AddTestLight(lightTexture, new Vector2(0, 15));
            AddTestLight(lightTexture, new Vector2(0, -15));
        }
        
        private void AddTestLight(Texture2D lightTexture, Vector2 position)
        {
            _lightmapGenerator.Lights.Add(
                new PointLight(lightTexture)
                    {
                        Radius = 15,
                        Color = Color.White,
                        Position = position,
                        Intensity = 0.65f,
                    });
        }
        
        private void GenerateRandomHullsAndLights(Texture2D lightTexture, Random random)
        {
            // Generate some random lights
            for (var i = 0; i < 10; i++)
            {
                AddTestLight(lightTexture, random.NextVector(-50, 50));
            }

            // Generate some random hulls
            for (var i = 0; i < 100; i++)
            {
                _lightmapGenerator.Hulls.Add(
                    HullFactory.CreateRectangle(2, 1)
                        .Position(random.NextVector(-50, 50))
                        .Angle(random.NextAngle()));
            }
        }
    }
}
