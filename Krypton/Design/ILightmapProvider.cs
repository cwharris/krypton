using Microsoft.Xna.Framework.Graphics;

namespace Krypton.Design
{
    public interface ILightmapProvider
    {
        Texture2D Lightmap { get; }
    }
}
