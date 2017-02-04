using System;
using Microsoft.Xna.Framework;

namespace Krypton.Factories
{
    public static class HullFactory
    {
        public static ShadowHull CreateCircle(
            float radius,
            int numSides)
        {
            var vertices = new Vector2[numSides];

            var angle = Math.PI*2/numSides;

            for (var i = 0; i < numSides; i++)
            {
                vertices[i] = new Vector2(
                    (float) Math.Cos(i*angle)*radius,
                    (float) Math.Sin(i*angle)*radius);
            }

            return new ShadowHull(vertices);
        }

        public static ShadowHull CreateRectangle(
            float width,
            float height)
        {
            return new ShadowHull(
                new Vector2(+width, +height),
                new Vector2(-width, +height),
                new Vector2(-width, -height),
                new Vector2(+width, -height));
        }
    }
}
