using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Krypton.Common
{
    /// <summary>
    /// A set of extension methods to help draw textures to the current render target
    /// </summary>
    public static class DrawToTargetExtensions
    {
        /// <summary>
        /// Draws textures to the render target using a single SpriteBatch call
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="blendState">The blend state.</param>
        /// <param name="texture1">Texture 1.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if texture is null
        /// </exception>
        public static void DrawToTarget(
            this SpriteBatch spriteBatch,
            BlendState blendState,
            Texture2D texture1)
        {
            if (texture1 == null)
            {
                throw new ArgumentNullException(nameof(texture1));
            }

            spriteBatch.BeginDrawToTarget(blendState);
            spriteBatch.DrawToTarget(texture1);
            spriteBatch.End();
        }

        /// <summary>
        /// Draws textures to the render target using a single SpriteBatch call
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="blendState">The blend state.</param>
        /// <param name="texture1">Texture 1.</param>
        /// <param name="texture2">Texture 2.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if any of the textures are null
        /// </exception>
        public static void DrawToTarget(
            this SpriteBatch spriteBatch,
            BlendState blendState,
            Texture2D texture1,
            Texture2D texture2)
        {
            if (texture1 == null)
            {
                throw new ArgumentNullException(nameof(texture1));
            }

            if (texture2 == null)
            {
                throw new ArgumentNullException(nameof(texture2));
            }

            spriteBatch.BeginDrawToTarget(blendState);
            spriteBatch.DrawToTarget(texture1);
            spriteBatch.DrawToTarget(texture2);
            spriteBatch.End();
        }

        /// <summary>
        /// Draws textures to the render target using a single SpriteBatch call
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="blendState">The blend state.</param>
        /// <param name="texture1">Texture 1.</param>
        /// <param name="texture2">Texture 2.</param>
        /// <param name="texture3">texture 3.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if any of the textures are null
        /// </exception>
        public static void DrawToTarget(
            this SpriteBatch spriteBatch,
            BlendState blendState,
            Texture2D texture1,
            Texture2D texture2,
            Texture2D texture3)
        {
            if (texture1 == null)
            {
                throw new ArgumentNullException(nameof(texture1));
            }

            if (texture2 == null)
            {
                throw new ArgumentNullException(nameof(texture2));
            }

            if (texture3 == null)
            {
                throw new ArgumentNullException(nameof(texture3));
            }

            spriteBatch.BeginDrawToTarget(blendState);
            spriteBatch.DrawToTarget(texture1);
            spriteBatch.DrawToTarget(texture2);
            spriteBatch.DrawToTarget(texture3);
            spriteBatch.End();
        }

        /// <summary>
        /// Draws textures to the render target using a single SpriteBatch call
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="blendState">The blend state.</param>
        /// <param name="textures">The textures.</param>
        /// <exception cref="ArgumentNullException">
        /// Throw if the textures parameter is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Throw if the textures parameter is empty
        /// </exception>
        public static void DrawToTarget(
            this SpriteBatch spriteBatch,
            BlendState blendState,
            params Texture2D[] textures)
        {
            if (textures == null)
            {
                throw new ArgumentNullException(nameof(textures));
            }

            if (textures.Length == 0)
            {
                throw new ArgumentException("textures cannot be empty", nameof(textures));
            }

            spriteBatch.BeginDrawToTarget(blendState);

            foreach (var texture in textures)
            {
                spriteBatch.DrawToTarget(texture);
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Draws the texture to the current render target using the SpriteBatch
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="texture">The texture.</param>
        private static void DrawToTarget(
            this SpriteBatch spriteBatch,
            Texture2D texture)
        {
            spriteBatch.Draw(
                texture: texture,
                destinationRectangle: spriteBatch.GraphicsDevice.Viewport.Bounds,
                color: Color.White);
        }

        /// <summary>
        /// Begin's the SpriteBatch with appropriate settings
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="blendState">The blend state.</param>
        private static void BeginDrawToTarget(
            this SpriteBatch spriteBatch,
            BlendState blendState)
        {
            spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: blendState,
                samplerState: SamplerState.LinearClamp,
                depthStencilState: DepthStencilState.None,
                rasterizerState: RasterizerState.CullNone,
                effect: null,
                transformMatrix: Matrix.Identity);
        }
    }
}
