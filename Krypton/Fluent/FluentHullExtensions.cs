using Microsoft.Xna.Framework;

namespace Krypton.Fluent
{
    public static class FluentHullExtensions
    {
        public static ShadowHull Position(
            this ShadowHull shadowHull,
            Vector2 position)
        {
            shadowHull.Position = position;
            return shadowHull;
        }
        public static ShadowHull Position(
            this ShadowHull shadowHull,
            float x,
            float y)
        {
            shadowHull.Position = new Vector2(x, y);
            return shadowHull;
        }

        public static ShadowHull Position(
            this ShadowHull shadowHull,
            double x,
            double y)
        {
            shadowHull.Position = new Vector2((float)x, (float)y);
            return shadowHull;
        }

        public static ShadowHull Angle(
            this ShadowHull shadowHull,
            float angle)
        {
            shadowHull.Angle = angle;
            return shadowHull;
        }

        public static ShadowHull Scale(
            this ShadowHull shadowHull,
            Vector2 scale)
        {
            shadowHull.Scale = scale;
            return shadowHull;
        }
    }
}
