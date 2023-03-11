using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Primitives;
using System;
using Terraria;

namespace OvermorrowMod.Common.Particles
{
    public class Diamond : Particle
    {
        public float maxTime = 90f;
        public override void OnSpawn()
        {
            if (Scale != 0f)
            {
                CustomData[0] = Scale;
            }
            else
                CustomData[0] = Main.rand.NextFloat(1f, 3f);
            Scale = 0;
        }
        public override void Update()
        {
            float progress = ((float)ActiveTime / maxTime);
            float p = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 2, 0f, 1f);
            Scale = MathHelper.Lerp(0f, CustomData[0], p);
            Alpha = p;
            Velocity *= 0.99f;
            Rotation = MathHelper.Pi / 2 + Velocity.X / 10f;
            if (ActiveTime > maxTime) Kill();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos2 = (Rotation + MathHelper.Pi / 2).ToRotationVector2() * 5f * Scale;
            PrimitivePacket packet = new PrimitivePacket(
                new[]
                {
                    PrimitiveHelper.AsVertex(
                        Position + Rotation.ToRotationVector2() * 10f * Scale,
                        Color,
                        Vector2.Zero),
                    PrimitiveHelper.AsVertex(
                        Position + pos2,
                        Color,
                        Vector2.Zero),
                    PrimitiveHelper.AsVertex(
                        Position - pos2,
                        Color,
                        Vector2.Zero),
                    PrimitiveHelper.AsVertex(
                        Position - Rotation.ToRotationVector2() * 10f * Scale,
                        Color,
                        Vector2.Zero)
                },
                PrimitiveType.TriangleStrip,
                4);

            packet.Send();
        }
    }

}