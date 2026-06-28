using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace OvermorrowMod.Core.Particles.Modular
{
    /// <summary>
    /// Pure per-frame advance for a particle. Every module (drag, gravity, turbulence, spin,
    /// scale-over-life, color-over-life, fade) always runs, so behaviors compose freely.
    /// </summary>
    public static class ParticleSimStep
    {
        public static void Step(ParticleSpec spec, ParticleState s)
        {
            // Motion modules
            if (spec.Drag != 0f) s.Velocity *= 1f - spec.Drag;
            s.Velocity += spec.Gravity;
            if (spec.Turbulence > 0f) s.Velocity += Main.rand.NextVector2Circular(spec.Turbulence, spec.Turbulence);
            s.Position += s.Velocity;

            // Orientation module
            switch (spec.Orientation)
            {
                case ParticleOrientation.FaceVelocity:
                    s.Rotation = s.Velocity.ToRotation();
                    break;
                case ParticleOrientation.Spin:
                    // Curve the path: rotate the velocity each frame and face the new direction.
                    s.Velocity = s.Velocity.RotatedBy(s.AngularVel);
                    s.Rotation = s.Velocity.ToRotation();
                    break;
            }

            float t = s.Lifetime <= 0f ? 1f : MathHelper.Clamp(s.Age / s.Lifetime, 0f, 1f);

            // Scale-over-life
            s.Scale = MathHelper.Lerp(s.StartScale, spec.EndScale, Ease(spec.ScaleEasing, t));

            // Color-over-life
            s.Color = Color.Lerp(spec.StartColor, spec.EndColor, t);

            // Alpha fade in/out
            float a = 1f;
            if (spec.AlphaFadeInFrac > 0f) a *= MathHelper.Clamp(t / spec.AlphaFadeInFrac, 0f, 1f);
            if (spec.AlphaFadeOutFrac > 0f) a *= MathHelper.Clamp((1f - t) / spec.AlphaFadeOutFrac, 0f, 1f);
            s.Alpha = a;

            s.Age++;
            if (s.Age > s.Lifetime) s.Dead = true;
        }

        private static float Ease(ParticleEasing easing, float t) => easing switch
        {
            ParticleEasing.EaseIn => t * t,
            ParticleEasing.EaseOut => 1f - (1f - t) * (1f - t),
            ParticleEasing.Sine => (float)Math.Sin(t * MathHelper.PiOver2),
            _ => t,
        };
    }
}
