using Microsoft.Xna.Framework;

namespace Krypton.Design
{
    public interface ILightmapDrawContext : IShadowHullDrawContext
    {
        void ClearShadowHulls();
        void PrepareToDrawNextShadowHull();
        void DrawShadowHulls();

        void DrawUnitQuad();
        void DrawClippedFov(
            Vector2 position,
            float rotation,
            float size,
            Color color,
            float fov);
    }
}
