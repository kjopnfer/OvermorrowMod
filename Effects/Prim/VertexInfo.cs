using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Effects.Prim
{
    public struct VertexInfo : IVertexType
    {
        public Vector2 Position;
        public Color Color;
        public Vector2 TexCoord;
        private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0), new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0), new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0));
        public VertexDeclaration VertexDeclaration => _vertexDeclaration;
        public VertexInfo(Vector2 position, Color color, Vector2 texcoord)
        {
            Position = position;
            Color = color;
            TexCoord = texcoord;
        }
    }
}