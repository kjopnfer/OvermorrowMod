using OvermorrowMod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System;
using OvermorrowMod.Effects.Prim;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    /**
     * This file is for a dummy projectile that handles the drawing of the boss barriers
     * It doesn't handle the collision for the textures since that is done within BossBarrier.cs
     * One projectile can handle all barriers, or you can use multiple projectiles for each barrier
     */
    public class snek : ModProjectile
    {
        public ref float BarrierProgress => ref projectile.ai[0];
        public override bool CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("snek BARRIER EEEeeeeEK");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 12;
            projectile.timeLeft = 540;
        }

        public override void AI()
        {
            // TODO: Set the projectile to persist while the boss is active or a condition is met

            if (BarrierProgress < 240)
            {
                BarrierProgress++;
            }
        }

        public Texture2D TrailTexture1 = ModContent.GetTexture(AssetDirectory.FullTrail + "Trail0v2");
        public Texture2D TrailTexture2 = ModContent.GetTexture(AssetDirectory.FullTrail + "Trail7");
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture(AssetDirectory.Boss + "SandstormBoss/snek");
            int progress = (int)(BarrierProgress / 240f * texture.Height);

            /* 
             * Because of how vanilla works with drawing their tiles, the following in conjunction with DrawBehind doesn't work:
             * spriteBatch.Draw(texture, new Vector2(projectile.Center.X, projectile.Center.Y - progress) - Main.screenPosition, Color.Pink);
             * This is because Terraria decides to draw black shit all over tiles that aren't lighted up and any SpriteBatch calls are just drawn over those for some reason
             * That effectively renders DrawBehind useless and until the actual projectile's texture meets any visibly lighted tiles then it won't draw behind
             * The following code uses the DrawOrigin to not have to deal with sliding the texture through the tiles and instead moves the drawing area downwards to seem like it is sliding up
             */
            spriteBatch.Draw(texture, new Rectangle((int)projectile.Center.X - texture.Width / 2 - (int)Main.screenPosition.X, (int)projectile.Center.Y - progress - (int)Main.screenPosition.Y, texture.Width, progress),
            new Rectangle(0, 0, texture.Width, progress), lightColor);

            Vector2 drawCenter = projectile.Center + Vector2.Lerp(new Vector2(62 - texture.Width / 2, 0), new Vector2(62 - texture.Width / 2, -188), progress / 240f);

            if (BarrierProgress > 35)
            {
                DrawSand(drawCenter);
                DrawSand(drawCenter + Vector2.UnitX * 60);
            }

            texture = ModContent.GetTexture(AssetDirectory.Boss + "SandstormBoss/snek_Front");
            spriteBatch.Draw(texture, new Rectangle((int)projectile.Center.X - texture.Width / 2 - (int)Main.screenPosition.X, (int)projectile.Center.Y - progress - (int)Main.screenPosition.Y, texture.Width, progress),
            new Rectangle(0, 0, texture.Width, progress), lightColor);
        }

        public void DrawSand(Vector2 drawCenter)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            float mult = 0.55f + (float)Math.Sin(Main.GlobalTime/* * 2*/) * 0.1f;

            BeamPacket packet = new BeamPacket();
            packet.Pass = "Texture";
            Vector2 start = drawCenter;
            Vector2 end = drawCenter + Vector2.UnitY * TRay.CastLength(drawCenter, Vector2.UnitY, 5000);
            float width = 25 * projectile.scale;
            Vector2 offset = (start - end).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * width;

            Color BeamColor = new Color(95, 73, 50);
            BeamPacket.SetTexture(0, TrailTexture1);
            float off = -Main.GlobalTime % 1;
            packet.Add(start + offset * 3 * mult, BeamColor, new Vector2(0 + off, 0));
            packet.Add(start - offset * 3 * mult, BeamColor, new Vector2(0 + off, 1));
            packet.Add(end + offset * 3 * mult, BeamColor, new Vector2(1 + off, 0));

            packet.Add(start - offset * 3 * mult, BeamColor, new Vector2(0 + off, 1));
            packet.Add(end - offset * 3 * mult, BeamColor, new Vector2(1 + off, 1));
            packet.Add(end + offset * 3 * mult, BeamColor, new Vector2(1 + off, 0));
            packet.Send();

            BeamColor = new Color(180, 128, 70);
            BeamPacket packet2 = new BeamPacket();
            packet2.Pass = "Texture";
            BeamPacket.SetTexture(0, TrailTexture2);
            packet2.Add(start + offset * 2 * mult, BeamColor, new Vector2(0 + off, 0));
            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0 + off, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1 + off, 0));

            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0 + off, 1));
            packet2.Add(end - offset * 2 * mult, BeamColor, new Vector2(1 + off, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1 + off, 0));
            packet2.Send();

            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < Main.rand.Next(2, 4); i++)
                {
                    Vector2 RandomPosition = end + new Vector2(Main.rand.Next(-10, 10), 0);
                    Vector2 RandomVelocity = -Vector2.One.RotatedByRandom(MathHelper.Pi) * Main.rand.Next(1, 3);
                    Particle.CreateParticle(Particle.ParticleType<Smoke2>(), RandomPosition, RandomVelocity, new Color(182, 128, 70), Main.rand.NextFloat(0.15f, 0.25f), Main.rand.NextFloat(0.6f, 1f), 0, 0, Main.rand.Next(30, 60));
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}