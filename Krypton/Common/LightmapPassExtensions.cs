using System.Linq;
using Krypton.Design;
using Microsoft.Xna.Framework;

namespace Krypton.Common
{
    public static class LightmapPassExtensions
    {
        public static Rectangle GetScissor(
            this ILightmapPass pass,
            ILight light)
        {
            var outline = light.Outline;

            if (outline == null)
            {
                return pass.Viewport.Bounds;
            }

            var vectors = outline
                .Select(x => ScreenToPixel(pass, Vector2.Transform(x, pass.Matrix)))
                .ToList();

            var min = vectors.Aggregate(Vector2.Min);
            var max = vectors.Aggregate(Vector2.Max);

            var rect = new Rectangle(
                x: (int)min.X,
                y: (int)min.Y,
                width: (int)(max.X - min.X),
                height: (int)(max.Y - min.Y));

            return Rectangle.Intersect(
                rect,
                pass.Viewport.Bounds);
        }

        private static Vector2 ScreenToPixel(
            ILightmapPass pass,
            Vector2 v)
        {
            v.X = ((1 + v.X) / 2) * pass.Viewport.Width + pass.Viewport.X;
            v.Y = ((1 - v.Y) / 2) * pass.Viewport.Height + pass.Viewport.Y;

            return v;
        }
    }
}
