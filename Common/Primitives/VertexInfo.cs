using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Common.Primitives
{
    public struct VertexInfo : IVertexType
    {
        private static VertexDeclaration _vertexDeclaration = new(new VertexElement[3]
        {
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position,0),
            new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color,0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate,0)
        });

        public Vector2 Position;

        public Color Color;

        public Vector3 TexCoord;

        public VertexDeclaration VertexDeclaration => _vertexDeclaration;

        public VertexInfo(Vector2 position, Color color, Vector3 texCoord)
        {
            Position = position;
            Color = color;
            TexCoord = texCoord;
        }
    }
}