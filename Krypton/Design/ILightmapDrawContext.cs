using Microsoft.Xna.Framework;

namespace Krypton.Design
{
    public interface ILightmapDrawContext
    {
        void Draw();

        void Clear();

        void DrawUnitQuad();

        void DrawClippedFov(
            Vector2 position,
            float rotation,
            float size,
            Color color,
            float fov);

        void AddIndex(int index);

        void AddVertex(HullVertex hullVertex);

        void SetStartVertex();
    }
}
