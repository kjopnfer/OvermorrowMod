using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace OvermorrowMod.Core.Particles.Modular
{
    /// <summary>
    /// Rolls a <see cref="ParticleSpec"/> into individual particle states (shape + ranges) and spawns
    /// them. World effects use <see cref="EmitWorld"/>; UI/contextual effects use a ParticleSystem instance.
    /// </summary>
    public static class ParticleEmitter
    {
        /// <summary>
        /// Produces one particle's initial state from the spec's emission shape and randomized ranges.
        /// </summary>
        public static ParticleState Roll(ParticleSpec spec, Vector2 origin)
        {
            var rng = Main.rand;

            // Spawn position + (for area shapes) the outward radial direction from the shape.
            Vector2 pos = origin;
            Vector2 radial = Vector2.Zero;
            bool haveRadial = false;
            switch (spec.Shape)
            {
                case EmitShape.Circle:
                    Vector2 off = rng.NextVector2Circular(spec.ShapeRadius, spec.ShapeRadius);
                    pos += off;
                    if (off != Vector2.Zero) { radial = Vector2.Normalize(off); haveRadial = true; }
                    break;
                case EmitShape.Ring:
                    radial = rng.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                    pos += radial * spec.ShapeRadius;
                    haveRadial = true;
                    break;
            }

            // Base launch direction (before the spread fan is applied).
            Vector2 dir = spec.DirectionMode switch
            {
                EmitDirection.FixedAngle => MathHelper.ToRadians(spec.Angle).ToRotationVector2(),
                EmitDirection.TowardCursor => (Main.MouseWorld - pos).SafeNormalize(Vector2.UnitX),
                // OutwardFromShape: radial for area shapes, else the configured Angle.
                _ => haveRadial ? radial : MathHelper.ToRadians(spec.Angle).ToRotationVector2(),
            };

            // Fan the direction. Cone shape uses ConeSpread; everything else uses SpreadDeg.
            float fan = MathHelper.ToRadians(spec.Shape == EmitShape.Cone ? spec.ConeSpread : spec.SpreadDeg);
            if (fan > 0f) dir = dir.RotatedBy(rng.NextFloat(-fan / 2f, fan / 2f));

            float speed = rng.NextFloat(spec.SpeedMin, spec.SpeedMax);
            var state = new ParticleState
            {
                Position = pos,
                Velocity = dir * speed,
                Lifetime = rng.Next(spec.LifetimeMin, spec.LifetimeMax + 1),
                StartScale = rng.NextFloat(spec.StartScaleMin, spec.StartScaleMax),
                AngularVel = rng.NextFloat(spec.AngularVelMin, spec.AngularVelMax),
                Rotation = MathHelper.ToRadians(rng.NextFloat(spec.StartRotationMin, spec.StartRotationMax)),
                Color = spec.StartColor,
            };
            state.Scale = state.StartScale;
            return state;
        }

        /// <summary>
        /// Spawns a burst of the spec's Count into the world via the global ParticleManager.
        /// </summary>
        public static void EmitWorld(ParticleSpec spec, Vector2 worldPos)
        {
            int count = Math.Max(1, spec.Count);
            for (int i = 0; i < count; i++)
                EmitWorldOne(spec, worldPos);
        }

        /// <summary>
        /// Spawns a single particle into the world (for hold-to-stream callers that pace their own rate).
        /// </summary>
        public static void EmitWorldOne(ParticleSpec spec, Vector2 worldPos)
        {
            ParticleState state = Roll(spec, worldPos);
            var particle = new ModularParticle(spec, state);
            ParticleManager.CreateParticleDirect(particle, state.Position, Vector2.Zero, state.Color,
                state.Alpha, state.Scale, state.Rotation, spec.DrawLayer, spec.Additive);
        }
    }
}
