using Krypton.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Krypton
{
    public class LightmapPass : ILightmapPass
    {
        public virtual Viewport Viewport { get; }

        public virtual Matrix Matrix { get; }

        public LightmapPass(
            Viewport viewport,
            Matrix matrix)
        {
            Viewport = viewport;
            Matrix = matrix;
        }

        public virtual void OnPassStart()
        {
        }

        public virtual void OnPassRunning()
        {
        }

        public virtual void OnPassComplete()
        {
        }
    }
}
