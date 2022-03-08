using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.SandstormBoss
{
    public class LightBullet : ModProjectile
    {
        private bool RunOnce = true;

        private Vector2 StartPosition;
        private Vector2 EndPosition;
        private Vector2 MidPoint1;
        private Vector2 MidPoint2;

        private int RandomStart;
        private int RandomDelay;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.LostSoulFriendly;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Light Warping Bullets");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 16;
            projectile.penetrate = -1;
            projectile.friendly = false;
            projectile.timeLeft = 270;
        }

        public override void AI()
        {
            if (RunOnce)
            {
                StartPosition = projectile.Center;
                EndPosition = new Vector2(projectile.ai[0], projectile.ai[1]);

                MidPoint1 = StartPosition + new Vector2(Main.rand.Next(-100, 100), Main.rand.Next(-100, 100));
                MidPoint2 = MidPoint1 + new Vector2(Main.rand.Next(-100, 100), Main.rand.Next(-100, 100));

                projectile.ai[0] = 0;
                projectile.ai[1] = 0;

                RandomStart = Main.rand.Next(8, 11) * 10;
                RandomDelay = Main.rand.Next(6, 13) * 5;
                RunOnce = false;
            }

            if (Main.rand.NextBool(3))
            {
                Vector2 RandomDirection = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 2;
                Particle.CreateParticle(Particle.ParticleType<Orb>(), projectile.Center, RandomDirection, Color.Orange, 1, Main.rand.NextFloat(0.25f, 0.4f), 0, 25);
            }

            // Moves to the position
            if (projectile.timeLeft > 110)
            {
                projectile.Center = ModUtils.Bezier(StartPosition, EndPosition, MidPoint2, MidPoint1, Utils.Clamp(projectile.ai[0]++, 0, 160) / 160f);
            }

            // Bounces up and down
            if (projectile.timeLeft <= RandomStart && projectile.timeLeft > RandomDelay)
            {
                float time = RandomStart - RandomDelay;

                if (projectile.timeLeft == RandomStart)
                {
                    projectile.ai[0] = 0;

                    StartPosition = projectile.Center;
                    EndPosition = projectile.Center;

                    MidPoint1 = StartPosition + new Vector2(0, -100);
                }

                projectile.Center = ModUtils.Bezier(StartPosition, EndPosition, MidPoint1, MidPoint1, Utils.Clamp(projectile.ai[0]++, 0, time) / time);
            }

            if (projectile.timeLeft == RandomDelay)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.UnitY, ModContent.ProjectileType<AncientElectricitiy>(), 20, 5f, Main.myPlayer);

                Vector2 end = projectile.Center + (Vector2.UnitY * TRay.CastLength(projectile.Center, Vector2.UnitY, 5000));
                NPC.NewNPC((int)(end.X), (int)(end.Y), ModContent.NPCType<Pillar>());
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = ModContent.GetTexture("OvermorrowMod/NPCs/Bosses/SandstormBoss/PillarSpawner");
            //float mult = (0.55f + (float)Math.Sin(Main.GlobalTime) * 0.1f);
            //float scale = projectile.scale * 2 * mult;

            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.Gold, 0, new Vector2(texture.Width, texture.Height) / 2, 1, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.White, 0, new Vector2(texture.Width, texture.Height) / 2, 0.5f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

}