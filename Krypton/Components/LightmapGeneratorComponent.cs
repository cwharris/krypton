using System;
using System.Collections.Generic;
using System.Linq;
using Krypton.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Krypton.Components
{
    public class LightmapGeneratorComponent : DrawableGameComponent, ILightmapProvider
    {
        private LightmapDrawContext _lightmapDrawContext;

        public LightmapGeneratorComponent(
            Game game,
            IEnumerable<ILightmapPass> passes) :
            base(game)
        {
            if (passes == null)
            {
                throw new ArgumentNullException(nameof(passes));
            }

            Passes = passes;
        }

        public event Action<ILightmapPass> PassStart;
        public event Action<ILightmapPass> PassComplete;
        public event Action<ILightmapPass> PassRunning;
        
        public ILightmapGenerator Generator { get; private set; }
        public IList<ILight> Lights { get; } = new List<ILight>();
        public IList<IShadowHull> Hulls { get; } = new List<IShadowHull>();
        public Color AmbientColor { get; set; } = Color.Black;
        Texture2D ILightmapProvider.Lightmap => Lightmap;
        public IEnumerable<ILightmapPass> Passes { get; set; }
        protected LightmapEffect Effect { get; private set; }
        protected RenderTarget2D Lightmap { get; private set; }
        
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(Lightmap);

            GraphicsDevice.Clear(
                options: ClearOptions.Target | ClearOptions.Stencil,
                color: AmbientColor,
                depth: 0,
                stencil: 1);

            DrawLightmap(gameTime);

            GraphicsDevice.SetRenderTarget(null);
        }
        
        protected void OnPassStart(ILightmapPass pass)
        {
            PassStart?.Invoke(pass);
        }
        
        protected void OnPassRunning(ILightmapPass pass)
        {
            PassRunning?.Invoke(pass);
        }
        
        protected void OnPassComplete(ILightmapPass pass)
        {
            PassComplete?.Invoke(pass);
        }
        
        protected void DrawLightmap(GameTime gameTime)
        {
            foreach (var pass in Passes)
            {
                // Start Pass
                OnPassStart(pass);
                GraphicsDevice.Viewport = pass.Viewport;
                Effect.Matrix = pass.Matrix;

                // Execute Pass
                OnPassRunning(pass);
                Generator.DrawLightmap(pass, Lights, Hulls);

                // Complete Pass
                OnPassComplete(pass);
            }
        }
        
        protected override void LoadContent()
        {
            LoadEffect();

            // Can add injection here, like above.
            _lightmapDrawContext = new LightmapDrawContext(GraphicsDevice);

            CreateLightmapTarget();

            GraphicsDevice.DeviceReset += GraphicsDeviceReset;
    
            Generator = new LightmapGenerator(GraphicsDevice, Effect, _lightmapDrawContext);
        }
        
        private void LoadEffect()
        {
            // Load Effect
            var effectProvider = Game.Components.OfType<ILightmapEffectProvider>().FirstOrDefault();

            if (effectProvider != null)
            {
                Effect = effectProvider.Effect;
            }
            else
            {
                var unwrappedEffect = Game.Content.Load<Effect>("Krypton/KryptonEffect");

                Effect = new LightmapEffect(unwrappedEffect);
            }

            if (Effect == null)
            {
                throw new ContentLoadException("Unable to obtain / load the Krypton Effect");
            }
        }
        
        private void CreateLightmapTarget()
        {
            Lightmap?.Dispose();

            // Generate Lightmap Target
            Lightmap = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8,
                0,
                RenderTargetUsage.PlatformContents);
        }
        
        private void GraphicsDeviceReset(
            object sender,
            EventArgs args)
        {
            CreateLightmapTarget();
        }
    }
}
