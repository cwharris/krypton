using Microsoft.Xna.Framework;

namespace Krypton.Common
{
    public struct BoundingRect
    {
        public Vector2 Min;
        public Vector2 Max;

        public float Left => Min.X;
        public float Right => Max.X;
        public float Top => Max.Y;
        public float Bottom => Min.Y;

        public float Width => Max.X - Min.X;
        public float Height => Max.Y - Min.Y;

        public static BoundingRect Empty { get; }
        public static BoundingRect MinMax { get; }

        static BoundingRect()
        {
            Empty = new BoundingRect();
            MinMax = new BoundingRect(
                float.MinValue*Vector2.One,
                float.MaxValue*Vector2.One);
        }

        public Vector2 Center => (Min + Max) / 2;

        public bool IsZero => Min.X == 0 &&
                              Min.Y == 0 &&
                              Max.X == 0 &&
                              Max.Y == 0;

        public BoundingRect(
            float x,
            float y,
            float width,
            float height)
        {
            Min.X = x;
            Min.Y = y;
            Max.X = x + width;
            Max.Y = y + height;
        }

        public BoundingRect(
            Vector2 min,
            Vector2 max)
        {
            Min = min;
            Max = max;
        }

        public bool Contains(
            float x,
            float y)
        {
            return
                Min.X <= x &&
                Min.Y <= y &&
                Max.X >= x &&
                Max.Y >= y;
        }

        public bool Contains(Vector2 vector)
        {
            return
                Min.X <= vector.X &&
                Min.Y <= vector.Y &&
                Max.X >= vector.X &&
                Max.Y >= vector.Y;
        }

        public void Contains(
            ref Vector2 rect,
            out bool result)
        {
            result =
                Min.X <= rect.X &&
                Min.Y <= rect.Y &&
                Max.X >= rect.X &&
                Max.Y >= rect.Y;
        }

        public bool Contains(BoundingRect rect)
        {
            return
                Min.X <= rect.Min.X &&
                Min.Y <= rect.Min.Y &&
                Max.X >= rect.Max.X &&
                Max.Y >= rect.Max.Y;
        }

        public void Contains(
            ref BoundingRect rect,
            out bool result)
        {
            result =
                Min.X <= rect.Min.X &&
                Min.Y <= rect.Min.Y &&
                Max.X >= rect.Max.X &&
                Max.Y >= rect.Max.Y ;
        }

        public bool Intersects(BoundingRect rect)
        {
            return
                Min.X < rect.Max.X &&
                Min.Y < rect.Max.Y &&
                Max.X > rect.Min.X &&
                Max.Y > rect.Min.Y;
        }

        public void Intersects(
            ref BoundingRect rect,
            out bool result)
        {
            result =
                Min.X < rect.Max.X &&
                Min.Y < rect.Max.Y &&
                Max.X > rect.Min.X &&
                Max.Y > rect.Min.Y;
        }

        public static BoundingRect Intersect(
            BoundingRect rect1,
            BoundingRect rect2)
        {
            BoundingRect result;

            var num8 = rect1.Max.X;
            var num7 = rect2.Max.X;
            var num6 = rect1.Max.Y;
            var num5 = rect2.Max.Y;
            var num2 = rect1.Min.X > rect2.Min.X ? rect1.Min.X : rect2.Min.X;
            var num = rect1.Min.Y > rect2.Min.Y ? rect1.Min.Y : rect2.Min.Y;
            var num4 = num8 < num7 ? num8 : num7;
            var num3 = num6 < num5 ? num6 : num5;

            if (num4 > num2 && num3 > num)
            {
                result.Min.X = num2;
                result.Min.Y = num;
                result.Max.X = num4;
                result.Max.Y = num3;

                return result;
            }

            result.Min.X = 0;
            result.Min.Y = 0;
            result.Max.X = 0;
            result.Max.Y = 0;

            return result;
        }

        public static void Intersect(
            ref BoundingRect rect1,
            ref BoundingRect rect2,
            out BoundingRect result)
        {
            var num8 = rect1.Max.X;
            var num7 = rect2.Max.X;
            var num6 = rect1.Max.Y;
            var num5 = rect2.Max.Y;
            var num2 = rect1.Min.X > rect2.Min.X ? rect1.Min.X : rect2.Min.X;
            var num = rect1.Min.Y > rect2.Min.Y ? rect1.Min.Y : rect2.Min.Y;
            var num4 = num8 < num7 ? num8 : num7;
            var num3 = num6 < num5 ? num6 : num5;

            if (num4 > num2 && num3 > num)
            {
                result.Min.X = num2;
                result.Min.Y = num;
                result.Max.X = num4;
                result.Max.Y = num3;
            }

            result.Min.X = 0;
            result.Min.Y = 0;
            result.Max.X = 0;
            result.Max.Y = 0;
        }

        public static BoundingRect Union(
            BoundingRect rect1,
            BoundingRect rect2)
        {
            BoundingRect result;

            var num6 = rect1.Max.X;
            var num5 = rect2.Max.X;
            var num4 = rect1.Max.Y;
            var num3 = rect2.Max.Y;
            var num2 = rect1.Min.X < rect2.Min.X ? rect1.Min.X : rect2.Min.X;
            var num = rect1.Min.Y < rect2.Min.Y ? rect1.Min.Y : rect2.Min.Y;
            var num8 = num6 > num5 ? num6 : num5;
            var num7 = num4 > num3 ? num4 : num3;

            result.Min.X = num2;
            result.Min.Y = num;
            result.Max.X = num8;
            result.Max.Y = num7;

            return result;
        }

        public static void Union(
            ref BoundingRect rect1,
            ref BoundingRect rect2,
            out BoundingRect result)
        {
            var num6 = rect1.Max.X;
            var num5 = rect2.Max.X;
            var num4 = rect1.Max.Y;
            var num3 = rect2.Max.Y;
            var num2 = rect1.Min.X < rect2.Min.X ? rect1.Min.X : rect2.Min.X;
            var num = rect1.Min.Y < rect2.Min.Y ? rect1.Min.Y : rect2.Min.Y;
            var num8 = num6 > num5 ? num6 : num5;
            var num7 = num4 > num3 ? num4 : num3;

            result.Min.X = num2;
            result.Min.Y = num;
            result.Max.X = num8;
            result.Max.Y = num7;
        }

        public bool Equals(BoundingRect other)
        {
            return
                Min.X == other.Min.X &&
                Min.Y == other.Min.Y &&
                Max.X == other.Max.X &&
                Max.Y == other.Max.Y;
        }

        public override int GetHashCode()
        {
            return Min.GetHashCode() + Max.GetHashCode();
        }

        public static bool operator ==(
            BoundingRect a,
            BoundingRect b)
        {
            return
                a.Min.X == b.Min.X &&
                a.Min.Y == b.Min.Y &&
                a.Max.X == b.Max.X &&
                a.Max.Y == b.Max.Y;
        }

        public static bool operator !=(
            BoundingRect a,
            BoundingRect b)
        {
            return
                a.Min.X != b.Min.X ||
                a.Min.Y != b.Min.Y ||
                a.Max.X != b.Max.X ||
                a.Max.Y != b.Max.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is BoundingRect && this == (BoundingRect) obj;
        }
    }
}
