using Microsoft.Xna.Framework;

namespace OvermorrowMod.Effects.Explosions
{
    public class CircularExplosionInstance
    {
        public int InitialTick { get; }
        public VertexPositionCenterTick[] Vertices { get; }
        public Vector2 Center { get; }
        public CircularExplosionGenerator Generator { get; }

        public CircularExplosionInstance(CircularExplosionGenerator generator, Vector2 center)
        {
            Vertices = generator.GenerateVertices(center);
            InitialTick = generator.CurrentTick;
            Center = center;
            Generator = generator;
        }
    }
}
