using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace OvermorrowMod.Effects.Explosions
{
    public struct VertexPositionCenterTick : IVertexType
    {
        public Vector3 Position;
        public Vector2 Center;
        public int Tick;

        public VertexPositionCenterTick(Vector3 position, Vector2 center, int tick)
        {
            Position = position;
            Center = center;
            Tick = tick;
        }

        private readonly static VertexDeclaration declaration = new VertexDeclaration(new VertexElement[]
        {
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(Marshal.SizeOf(typeof(Vector3)), VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(Marshal.SizeOf(typeof(Vector3)) + Marshal.SizeOf(typeof(Vector2)),
                VertexElementFormat.Byte4, VertexElementUsage.TextureCoordinate, 1),
        });

        public VertexDeclaration VertexDeclaration => declaration;
    }
}
