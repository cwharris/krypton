using System;
using Microsoft.Xna.Framework;

namespace Krypton.Common
{
    public static class RandomExtensions
    {
        public static Vector2 NextVector(
            this Random random,
            float min,
            float max)
        {
            var size = max - min;
            return new Vector2(
                (float) (random.NextDouble()*size) + min,
                (float) (random.NextDouble()*size) + min);
        }
        
        public static float NextAngle(this Random random)
        {
            return ((float) random.NextDouble()*MathHelper.TwoPi) - MathHelper.Pi;
        }
    }
}
