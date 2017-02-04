using System;
using System.Collections.Generic;
using Krypton.Common;
using Krypton.Lights;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Krypton
{
    public class KryptonEngine : DrawableGameComponent
    {
        // The Krypton Effect
        private readonly string _effectAssetName;
        private Effect _effect;

        // The goods

        // World View Projection matrix, and it's min and max view bounds
        private Matrix _wvp = Matrix.Identity;
        private BoundingRect _bounds = BoundingRect.MinMax;

        // Blur
        private float _bluriness;
        private RenderTarget2D _mapBlur;

        // Light maps
        private RenderTarget2D _map;
        private LightMapSize _lightMapSize = LightMapSize.Full;

        /// <summary>
        /// Krypton's render helper. It helps render. It also needs to be re-written.
        /// </summary>
        public KryptonRenderHelper RenderHelper { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating how Krypton should cull geometry. The default value is CullMode.CounterClockwise
        /// </summary>
        public CullMode CullMode { get; set; } = CullMode.CullCounterClockwiseFace;

        /// <summary>
        /// The collection of lights krypton uses to render shadows
        /// </summary>
        public List<ILight2D> Lights { get; } = new List<ILight2D>();

        /// <summary>
        /// The collection of hulls krypton uses to render shadows
        /// </summary>
        public List<ShadowHull> Hulls { get; } = new List<ShadowHull>();

        /// <summary>
        /// Gets or sets the matrix used to draw the light map. This should match your scene's matrix.
        /// </summary>
        public Matrix Matrix
        {
            get { return _wvp; }
            set
            {
                if (_wvp == value)
                {
                    return;
                }

                _wvp = value;

                // This is totally ghetto, but it works for now. :)
                // Compute the world-space bounds of the given matrix
                var inverse = Matrix.Invert(value);

                var v1 = Vector2.Transform(new Vector2(1, 1), inverse);
                var v2 = Vector2.Transform(new Vector2(1, -1), inverse);
                var v3 = Vector2.Transform(new Vector2(-1, -1), inverse);
                var v4 = Vector2.Transform(new Vector2(-1, 1), inverse);

                _bounds.Min = v1;
                _bounds.Min = Vector2.Min(_bounds.Min, v2);
                _bounds.Min = Vector2.Min(_bounds.Min, v3);
                _bounds.Min = Vector2.Min(_bounds.Min, v4);

                _bounds.Max = v1;
                _bounds.Max = Vector2.Max(_bounds.Max, v2);
                _bounds.Max = Vector2.Max(_bounds.Max, v3);
                _bounds.Max = Vector2.Max(_bounds.Max, v4);

                _bounds = BoundingRect.MinMax;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating weither or not to use SpriteBatch's matrix when drawing lightmaps
        /// </summary>
        public bool SpriteBatchCompatablityEnabled { get; set; }

        /// <summary>
        /// Ambient color of the light map. Lights + AmbientColor = Final 
        /// </summary>
        public Color AmbientColor { get; set; } = new Color(0, 0, 0);

        /// <summary>
        /// Gets or sets the value used to determine light map size
        /// </summary>
        public LightMapSize LightMapSize
        {
            get { return _lightMapSize; }
            set
            {
                if (_lightMapSize == value)
                {
                    return;
                }

                _lightMapSize = value;
                DisposeRenderTargets();
                CreateRenderTargets();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating how much to blur the final light map. If the value is zero, the lightmap will not be blurred
        /// </summary>
        public float Bluriness
        {
            get { return _bluriness; }
            set { _bluriness = Math.Max(0, value); }
        }

        /// <summary>
        /// Constructs a new instance of krypton
        /// </summary>
        /// <param name="game">Your game object</param>
        /// <param name="effectAssetName">The asset name of Krypton's effect file, which must be included in your content project</param>
        public KryptonEngine(Game game, string effectAssetName)
            : base(game)
        {
            _effectAssetName = effectAssetName;
        }

        /// <summary>
        /// Initializes Krpyton, and hooks itself to the graphics device
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            GraphicsDevice.DeviceReset += new EventHandler<EventArgs>(GraphicsDevice_DeviceReset);
        }

        /// <summary>
        /// Resets kryptons graphics device resources
        /// </summary>
        private void GraphicsDevice_DeviceReset(
            object sender,
            EventArgs e)
        {
            DisposeRenderTargets();
            CreateRenderTargets();
        }

        /// <summary>
        /// Load's the graphics related content required to draw light maps
        /// </summary>
        protected override void LoadContent()
        {
            // This needs to better handle content loading...
            // if the window is resized, Krypton needs to notice.
            _effect = Game.Content.Load<Effect>(_effectAssetName);
            RenderHelper = new KryptonRenderHelper(GraphicsDevice, _effect);

            CreateRenderTargets();
        }

        /// <summary>
        /// Unload's the graphics content required to draw light maps
        /// </summary>
        protected override void UnloadContent()
        {
            DisposeRenderTargets();
        }

        /// <summary>
        /// Creates render targets
        /// </summary>
        private void CreateRenderTargets()
        {
            var targetWidth = GraphicsDevice.Viewport.Width/(int) _lightMapSize;
            var targetHeight = GraphicsDevice.Viewport.Height/(int) _lightMapSize;

            _map = new RenderTarget2D(
                graphicsDevice: GraphicsDevice,
                width: targetWidth,
                height: targetHeight,
                mipMap: false,
                preferredFormat: SurfaceFormat.Color,
                preferredDepthFormat: DepthFormat.Depth24Stencil8,
                preferredMultiSampleCount: 0,
                usage: RenderTargetUsage.PlatformContents);

            _mapBlur = new RenderTarget2D(
                graphicsDevice: GraphicsDevice,
                width: targetWidth,
                height: targetHeight,
                mipMap: false,
                preferredFormat: SurfaceFormat.Color,
                preferredDepthFormat: DepthFormat.Depth24Stencil8,
                preferredMultiSampleCount: 0,
                usage: RenderTargetUsage.PlatformContents);
        }

        /// <summary>
        /// Disposes of render targets
        /// </summary>
        private void DisposeRenderTargets()
        {
            TryDispose(_map);
            TryDispose(_mapBlur);
        }

        /// <summary>
        /// Attempts to dispose of disposable objects, and assigns them a null value afterward
        /// </summary>
        /// <param name="obj"></param>
        private static void TryDispose(IDisposable obj)
        {
            obj?.Dispose();
        }

        /// <summary>
        /// Draws the light map to the current render target
        /// </summary>
        /// <param name="gameTime">N/A - Required</param>
        public override void Draw(GameTime gameTime)
        {
            LightMapPresent();
        }

        /// <summary>
        /// Prepares the light map to be drawn (pre-render)
        /// </summary>
        public void LightMapPrepare()
        {
            // Prepare the matrix with optional settings and assign it to an effect parameter
            Matrix lightMapMatrix = LightmapMatrixGet();
            _effect.Parameters["Matrix"].SetValue(lightMapMatrix);

            // Obtain the original rendering states
            var originalRenderTargets = GraphicsDevice.GetRenderTargets();

            // Set and clear the target
            GraphicsDevice.SetRenderTarget(_map);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.Stencil, AmbientColor, 0, 1);

            // Make sure we're culling the right way!
            GraphicsDevice.RasterizerState = KryptonEngine.RasterizerStateGetFromCullMode(CullMode);

            // put the render target's size into a more friendly format
            var targetSize = new Vector2(_map.Width, _map.Height);

            // Render Light Maps
            foreach (var light in Lights)
            {
                // Loop through each light within the view frustum
                if (!light.Bounds.Intersects(_bounds))
                {
                    continue;
                }

                // Clear the stencil and set the scissor rect (because we're stretching geometry past the light's reach)
                GraphicsDevice.Clear(
                    options: ClearOptions.Stencil,
                    color: Color.Black,
                    depth: 0,
                    stencil: 0);

                GraphicsDevice.ScissorRectangle =
                    ScissorRectCreateForLight(
                        light: light,
                        matrix: lightMapMatrix,
                        targetSize: targetSize);
                    
                // Draw the light!
                light.Draw(RenderHelper, Hulls);
            }

            if (_bluriness > 0)
            {
                // Blur the shadow map horizontally to the blur target
                GraphicsDevice.SetRenderTarget(_mapBlur);

                RenderHelper.BlurTextureToTarget(
                    _map,
                    LightMapSize.Full,
                    BlurTechnique.Horizontal,
                    _bluriness);

                // Blur the shadow map vertically back to the final map
                GraphicsDevice.SetRenderTarget(_map);

                RenderHelper.BlurTextureToTarget(
                    _mapBlur,
                    LightMapSize.Full,
                    BlurTechnique.Vertical,
                    _bluriness);
            }

            // Reset to the original rendering states
            GraphicsDevice.SetRenderTargets(originalRenderTargets);
        }

        /// <summary>
        /// Returns the final, modified matrix used to render the lightmap.
        /// </summary>
        /// <returns></returns>
        private Matrix LightmapMatrixGet()
        {
            if (!SpriteBatchCompatablityEnabled)
            {
                // Return krypton's matrix
                return _wvp;
            }

            var xScale = GraphicsDevice.Viewport.Width > 0
                ? 1f / GraphicsDevice.Viewport.Width
                : 0f;

            var yScale = GraphicsDevice.Viewport.Height > 0
                ? -1f / GraphicsDevice.Viewport.Height
                : 0f;

            // This is the default matrix used to render sprites via spritebatch
            var matrixSpriteBatch = new Matrix
            {
                M11 = xScale*2f,
                M22 = yScale*2f,
                M33 = 1f,
                M44 = 1f,
                M41 = -1f - xScale,
                M42 = 1f - yScale,
            };

            // Return krypton's matrix, compensated for use with SpriteBatch
            return _wvp*matrixSpriteBatch;
        }

        /// <summary>
        /// Gets a pixel-space rectangle which contains the light passed in
        /// </summary>
        /// <param name="light">The light used to create the rectangle</param>
        /// <param name="matrix">the WorldViewProjection matrix being used to render</param>
        /// <param name="targetSize">The rendertarget's size</param>
        /// <returns></returns>
        private static Rectangle ScissorRectCreateForLight(
            ILight2D light,
            Matrix matrix,
            Vector2 targetSize)
        {
            // This needs refining, but it works as is (I believe)
            var lightBounds = light.Bounds;

            var min = VectorToPixel(lightBounds.Min, matrix, targetSize);
            var max = VectorToPixel(lightBounds.Max, matrix, targetSize);

            var min2 = Vector2.Min(min, max);
            var max2 = Vector2.Max(min, max);

            min = Vector2.Clamp(min2, Vector2.Zero, targetSize);
            max = Vector2.Clamp(max2, Vector2.Zero, targetSize);

            return new Rectangle(
                x: (int) min.X,
                y: (int) min.Y,
                width: (int) (max.X - min.X),
                height: (int) (max.Y - min.Y));
        }

        /// <summary>
        /// Takes a screen-space vector and puts it in to pixel space
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="matrix"></param>
        /// <param name="targetSize"></param>
        /// <returns></returns>
        private static Vector2 VectorToPixel(
            Vector2 vector,
            Matrix matrix,
            Vector2 targetSize)
        {
            Vector2.Transform(ref vector, ref matrix, out vector);

            vector.X = (1 + vector.X) * (targetSize.X / 2f);
            vector.Y = (1 - vector.Y) * (targetSize.Y / 2f);

            return vector;
        }

        /// <summary>
        /// Takes a screen-space size vector and converts it to a pixel-space size vector
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="matrix"></param>
        /// <param name="targetSize"></param>
        /// <returns></returns>
        private static Vector2 ScaleToPixel(
            Vector2 vector,
            Matrix matrix,
            Vector2 targetSize)
        {
            vector.X *= matrix.M11 * (targetSize.X / 2f);
            vector.Y *= matrix.M22 * (targetSize.Y / 2f);

            return vector;
        }

        /// <summary>
        /// Retrieves a rasterize state by using the cull mode as a lookup
        /// </summary>
        /// <param name="cullMode">The cullmode used to lookup the rasterize state</param>
        /// <returns></returns>
        private static RasterizerState RasterizerStateGetFromCullMode(CullMode cullMode)
        {
            switch (cullMode)
            {
                case CullMode.CullCounterClockwiseFace:
                    return RasterizerState.CullCounterClockwise;

                case CullMode.CullClockwiseFace:
                    return RasterizerState.CullClockwise;
                    
                default:
                    return RasterizerState.CullNone;
            }
        }

        /// <summary>
        /// Presents the light map to the current render target
        /// </summary>
        private void LightMapPresent()
        {
            RenderHelper.DrawTextureToTarget(
                _map,
                _lightMapSize,
                BlendTechnique.Multiply);
        }
    }
}
