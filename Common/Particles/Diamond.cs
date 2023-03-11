using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Primitives;
using System;
using Terraria;

namespace OvermorrowMod.Common.Particles
{
    public class Diamond : CustomParticle
    {
        public float maxTime = 90f;
        public override void OnSpawn()
        {
            if (particle.scale != 0f)
            {
                particle.customData[0] = particle.scale;
            }
            else
                particle.customData[0] = Main.rand.NextFloat(1f, 3f);
            particle.scale = 0;
        }
        public override void Update()
        {
            float progress = ((float)particle.activeTime / maxTime);
            float p = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 2, 0f, 1f);
            particle.scale = MathHelper.Lerp(0f, particle.customData[0], p);
            particle.alpha = p;
            particle.velocity *= 0.99f;
            particle.rotation = MathHelper.Pi / 2 + particle.velocity.X / 10f;
            if (particle.activeTime > maxTime) particle.Kill();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos2 = (particle.rotation + MathHelper.Pi / 2).ToRotationVector2() * 5f * particle.scale;
            PrimitivePacket packet = new PrimitivePacket(
                new[]
                {
                    PrimitiveHelper.AsVertex(
                        particle.position + particle.rotation.ToRotationVector2() * 10f * particle.scale,
                        particle.color,
                        Vector2.Zero),
                    PrimitiveHelper.AsVertex(
                        particle.position + pos2,
                        particle.color,
                        Vector2.Zero),
                    PrimitiveHelper.AsVertex(
                        particle.position - pos2,
                        particle.color,
                        Vector2.Zero),
                    PrimitiveHelper.AsVertex(
                        particle.position - particle.rotation.ToRotationVector2() * 10f * particle.scale,
                        particle.color,
                        Vector2.Zero)
                },
                PrimitiveType.TriangleStrip,
                4);

            packet.Send();
        }
    }

}