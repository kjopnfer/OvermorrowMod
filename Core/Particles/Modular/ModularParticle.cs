using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Particles.Modular
{
    /// <summary>
    /// World particle driven entirely by a <see cref="ParticleSpec"/>. Runs the shared sim step and
    /// draws with the spec's texture; the global ParticleManager handles its draw layer and blend.
    /// </summary>
    public class ModularParticle : CustomParticle
    {
        private readonly ParticleSpec spec;
        private readonly ParticleState state;

        public ModularParticle(ParticleSpec spec, ParticleState state)
        {
            this.spec = spec;
            this.state = state;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void Update()
        {
            ParticleSimStep.Step(spec, state);

            particle.position = state.Position;
            particle.scale = state.Scale;
            particle.rotation = state.Rotation;
            particle.color = state.Color;
            particle.alpha = state.Alpha;

            if (state.Dead) particle.Kill();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            ParticleDraw.Draw(spriteBatch, spec, state, state.Position - Main.screenPosition);
        }
    }
}
