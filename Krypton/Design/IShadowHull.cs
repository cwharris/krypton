using Microsoft.Xna.Framework;

namespace Krypton.Design
{
    public interface IShadowHull
    {
        Vector2 Position { get; }
        float RadiusSquared { get; }
        void Draw(IShadowHullDrawContext drawContext);
    }
}
