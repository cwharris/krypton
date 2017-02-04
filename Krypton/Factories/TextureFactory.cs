using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Krypton.Factories
{
    public static class TextureFactory
    {
        public static Texture2D CreatePoint(
            GraphicsDevice device,
            int size)
        {
            return CreateConic(device, size, MathHelper.TwoPi, 0);
        }
        
        public static Texture2D CreateConic(
            GraphicsDevice device,
            int size,
            float fov)
        {
            return CreateConic(device, size, fov, 0);
        }
        
        public static Texture2D CreateConic(
            GraphicsDevice device,
            int size,
            float fov,
            float nearPlaneDistance)
        {
            var data1D = new Color[size * size];

            var halfSize = size / 2f;

            fov /= 2;

            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    var vector = new Vector2(x - halfSize, y - halfSize);

                    var distance = vector.Length();

                    var angle = Math.Abs(Math.Atan2(vector.Y, vector.X));

                    var illumination = 0f;

                    if (distance <= halfSize && distance >= nearPlaneDistance && angle <= fov)
                    {
                        illumination = (halfSize - distance) / halfSize;
                    }

                    data1D[x + (y * size)] = new Color(illumination, illumination, illumination, illumination);
                }
            }

            var tex = new Texture2D(device, size, size);

            tex.SetData(data1D);

            return tex;
        }
    }
}
