using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace OvermorrowMod.Core.Particles.Modular
{
    /// <summary>
    /// A self-contained particle system you own and draw inline in any context (a UIElement, an item
    /// slot/tooltip, the editor preview). Particles live in local coordinates; <see cref="Draw"/> places
    /// them relative to a supplied origin, so the same spec works in world or UI space.
    /// </summary>
    public class ParticleSystem
    {
        private readonly List<(ParticleSpec Spec, ParticleState State)> _particles = new();

        public int Count => _particles.Count;

        public void Emit(ParticleSpec spec, Vector2 localPos)
        {
            int count = System.Math.Max(1, spec.Count);
            for (int i = 0; i < count; i++)
                _particles.Add((spec, ParticleEmitter.Roll(spec, localPos)));
        }

        /// <summary>
        /// Emits exactly one particle (for hold-to-stream callers that pace their own rate).
        /// </summary>
        public void EmitOne(ParticleSpec spec, Vector2 localPos)
            => _particles.Add((spec, ParticleEmitter.Roll(spec, localPos)));

        public void Update()
        {
            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                ParticleSimStep.Step(_particles[i].Spec, _particles[i].State);
                if (_particles[i].State.Dead) _particles.RemoveAt(i);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 origin, bool useShader = true)
        {
            foreach (var (spec, state) in _particles)
                ParticleDraw.Draw(spriteBatch, spec, state, origin + state.Position, useShader);
        }

        public void Clear() => _particles.Clear();
    }
}
