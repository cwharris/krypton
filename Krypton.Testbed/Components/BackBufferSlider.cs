using System;
using Microsoft.Xna.Framework;

namespace Krypton.Testbed.Components
{
    public class BackBufferSlider : DrawableGameComponent
    {
        private float _r;
        private float _g;
        private float _b;
        
        public BackBufferSlider(Game game)
            : base(game)
        {
        }
        
        public override void Update(GameTime gameTime)
        {
            _r = ((float) Math.Cos(gameTime.TotalGameTime.TotalSeconds*1) + 2)/3;
            _g = ((float) Math.Cos(gameTime.TotalGameTime.TotalSeconds*2) + 2)/3;
            _b = ((float) Math.Cos(gameTime.TotalGameTime.TotalSeconds*3) + 2)/3;
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(_r, _g, _b));
        }
    }
}
