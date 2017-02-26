using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Krypton.Hull
{
    [DebuggerDisplay("{Position} {Normal} {Color}")]
    public struct ShadowHullVertex : IVertexType
    {
        public Vector2 Position;

        public Vector2 Normal;

        public Color Color;
        
        public VertexDeclaration VertexDeclaration => Declaration;
        
        public ShadowHullVertex(
            Vector2 position,
            Vector2 normal) :
            this(
                position: position,
                normal: normal,
                opacity: 1)
        {
        }
        
        public ShadowHullVertex(
            Vector2 position,
            Vector2 normal,
            float opacity) :
            this(
                position: position,
                normal: normal,
                color: new Color(0, 0, 0, opacity))
        {
        }
        
        public ShadowHullVertex(
            Vector2 position,
            Vector2 normal,
            Color color)
        {
            Position = position;
            Normal = normal;
            Color = color;
        }
        
        private static readonly VertexDeclaration Declaration =
            new VertexDeclaration(
                new VertexElement(
                    offset: 0,
                    elementFormat: VertexElementFormat.Vector2,
                    elementUsage: VertexElementUsage.Position,
                    usageIndex: 0),
                new VertexElement(
                    offset: 8,
                    elementFormat: VertexElementFormat.Vector2,
                    elementUsage: VertexElementUsage.Normal,
                    usageIndex: 0),
                new VertexElement(
                    offset: 16,
                    elementFormat: VertexElementFormat.Color,
                    elementUsage: VertexElementUsage.Color,
                    usageIndex: 0));
    }
}
