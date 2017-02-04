using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Krypton.Design
{
    public interface ILight
    {
        bool On { get; }

        IEnumerable<Vector2> Outline { get; }

        void Draw(
            LightmapEffect lightmapEffect,
            ILightmapPass pass,
            ILightmapDrawContext helper,
            IEnumerable<IShadowHull> hulls);
    }
}
