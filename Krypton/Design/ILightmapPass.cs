using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Krypton.Design
{
    public interface ILightmapPass
    {
        Viewport Viewport { get; }
        Matrix Matrix { get; }
        
        void OnPassStart();
        void OnPassRunning();
        void OnPassComplete();
    }
}
