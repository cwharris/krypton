using Microsoft.Xna.Framework;

namespace Krypton.Common
{
    public static class Vector2Extensions
    {
        public static Vector2 Clockwise(this Vector2 v)
        {
            return new Vector2(v.Y, -v.X);
        }

        public static Vector2 CounterClockwise(this Vector2 v)
        {
            return new Vector2(-v.Y, v.X);
        }

        public static Vector2 Unit(this Vector2 v)
        {
            v.Normalize();
            return v;
        }
    }
}
