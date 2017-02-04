using Microsoft.Xna.Framework;

namespace Krypton.Common
{
    public static class ViewportExtensions
    {
        public static Rectangle ScissorRectCreateForLight(
            Vector2 min,
            Vector2 max,
            Matrix matrix,
            Vector2 targetSize)
        {
            min = VectorToPixel(min, matrix, targetSize);
            max = VectorToPixel(max, matrix, targetSize);

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

        private static Vector2 VectorToPixel(
            Vector2 position,
            Matrix matrix,
            Vector2 targetSize)
        {
            Vector2.Transform(
                ref position,
                ref matrix,
                out position);

            position.X = (1 + position.X) * (targetSize.X / 2f);
            position.Y = (1 - position.Y) * (targetSize.Y / 2f);

            return position;
        }
    }
}
