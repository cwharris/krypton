using System.Collections.Generic;
using Krypton.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Krypton
{
    public class LightmapGenerator : ILightmapGenerator
    {
        private readonly GraphicsDevice _device;
        private readonly LightmapEffect _lightmapEffect;
        private readonly ILightmapDrawContext _drawContext;

        public Color AmbientColor { get; set; }

        public LightmapGenerator(
            GraphicsDevice device,
            LightmapEffect lightmapEffect,
            ILightmapDrawContext drawContext)
        {
            _device = device;
            _lightmapEffect = lightmapEffect;
            _drawContext = drawContext;
        }

        public void DrawLightmap(
            ILightmapPass pass,
            IList<ILight> lights,
            IList<IShadowHull> hulls)
        {
            foreach (var light in lights)
            {
                _device.Clear(
                    options: ClearOptions.Stencil,
                    color: Color.Black,
                    depth: 0,
                    stencil: 0);

                light.Draw(
                    lightmapEffect: _lightmapEffect,
                    pass: pass,
                    helper: _drawContext,
                    hulls: hulls);
            }
        }
    }
}
