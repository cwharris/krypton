using System.Collections.Generic;

namespace Krypton.Design
{
    public interface ILightmapGenerator
    {
        void DrawLightmap(
            ILightmapPass pass,
            IList<ILight> lights,
            IList<IShadowHull> hulls);
    }
}
