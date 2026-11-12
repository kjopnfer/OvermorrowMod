using Microsoft.Xna.Framework;

namespace OvermorrowMod.Core.Particles.Modular
{
    /// <summary>
    /// The mutable per-particle state advanced by <see cref="ParticleSimStep"/>. Shared by the
    /// world path (<see cref="ModularParticle"/>) and the contextual/UI path so behavior is identical.
    /// </summary>
    public class ParticleState
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Age;
        public float Lifetime;
        public float Scale;
        public float StartScale;
        public float Rotation;
        public float AngularVel;
        public float Alpha = 1f;
        public Color Color = Color.White;
        public string Texture;
        public bool Dead;
    }
}
