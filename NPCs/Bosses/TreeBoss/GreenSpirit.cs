using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Effects.Prim;
using OvermorrowMod.Effects.Prim.Trails;
using OvermorrowMod.Particles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.TreeBoss
{
    public class GreenSpirit : ModProjectile, ITrailEntity
    {
        public Color TrailColor(float progress) => new Color(66, 245, 167);
        public float TrailSize(float progress) => 20;
        
        public bool TrailActive()
        {
            return true;
        }

        public Type TrailType()
        {
            return typeof(SoulTrail);
        }

        private bool RunOnce = true;
        private float Radius;
        public Player RotationCenter;
        public Vector2 OldPosition;
        public override string Texture => "Terraria/Item_" + ProjectileID.LostSoulFriendly;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit Blades");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.timeLeft = 180 * 10;
            projectile.extraUpdates = 10;
        }

        public override void AI()
        {
            if (RunOnce)
            {
                Radius = projectile.ai[1];
                projectile.ai[1] = 0;

                RunOnce = false;
            }

            #region Dust Code
            if (projectile.ai[1] == 0)
            {
                Main.PlaySound(SoundID.Item25, projectile.Center);

                Vector2 vector23 = projectile.Center + Vector2.One * -20f;
                int num137 = 40;
                int num138 = num137;
                for (int num139 = 0; num139 < 4; num139++)
                {
                    int num140 = Dust.NewDust(vector23, num137, num138, 89, 0f, 0f, 100, default(Color), 0.25f);
                    Main.dust[num140].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
                }

                for (int num141 = 0; num141 < 10; num141++)
                {
                    int num142 = Dust.NewDust(vector23, num137, num138, 107, 0f, 0f, 200, default(Color), 0.7f);
                    Main.dust[num142].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
                    Main.dust[num142].noGravity = true;
                    Main.dust[num142].noLight = true;
                    Dust dust = Main.dust[num142];
                    dust.velocity *= 3f;
                    dust = Main.dust[num142];
                    dust.velocity += projectile.DirectionTo(Main.dust[num142].position) * (2f + Main.rand.NextFloat() * 4f);
                    num142 = Dust.NewDust(vector23, num137, num138, 107, 0f, 0f, 100, default(Color), 0.25f);
                    Main.dust[num142].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
                    dust = Main.dust[num142];
                    dust.velocity *= 2f;
                    Main.dust[num142].noGravity = true;
                    Main.dust[num142].fadeIn = 1f;
                    Main.dust[num142].color = Color.Crimson * 0.5f;
                    Main.dust[num142].noLight = true;
                    dust = Main.dust[num142];
                    dust.velocity += projectile.DirectionTo(Main.dust[num142].position) * 8f;
                }

                for (int num143 = 0; num143 < 10; num143++)
                {
                    int num144 = Dust.NewDust(vector23, num137, num138, 107, 0f, 0f, 0, default(Color), 0.7f);
                    Main.dust[num144].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy(projectile.velocity.ToRotation()) * num137 / 2f;
                    Main.dust[num144].noGravity = true;
                    Main.dust[num144].noLight = true;
                    Dust dust = Main.dust[num144];
                    dust.velocity *= 3f;
                    dust = Main.dust[num144];
                    dust.velocity += projectile.DirectionTo(Main.dust[num144].position) * 2f;
                }

                for (int num145 = 0; num145 < 50; num145++)
                {
                    int num146 = Dust.NewDust(vector23, num137, num138, 89, 0f, 0f, 0, default(Color), 0.25f);
                    Main.dust[num146].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy(projectile.velocity.ToRotation()) * num137 / 2f;
                    Main.dust[num146].noGravity = true;
                    Dust dust = Main.dust[num146];
                    dust.velocity *= 3f;
                    dust = Main.dust[num146];
                    dust.velocity += projectile.DirectionTo(Main.dust[num146].position) * 3f;
                }
            }
            #endregion

            if (!RotationCenter.active)
            {
                projectile.Kill();
            }

            if (projectile.timeLeft > 80 * 10)
            {
                projectile.rotation = projectile.DirectionTo(RotationCenter.Center).ToRotation();

                projectile.Center = RotationCenter.Center + new Vector2(Radius, 0).RotatedBy(projectile.ai[0]);
                OldPosition = RotationCenter.Center;
            }
            else if (projectile.timeLeft == 45 * 10)
            {
                Main.PlaySound(SoundID.DD2_PhantomPhoenixShot, projectile.Center);
                projectile.velocity = Vector2.Normalize(OldPosition - projectile.Center) * 12;
            }



            projectile.ai[1]++;
            projectile.localAI[0]++;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("Terraria/Projectile_644");
            Rectangle rect = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);

            //spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), new Color(54, 255, 64), projectile.rotation + MathHelper.PiOver2, drawOrigin, new Vector2(MathHelper.Lerp(0.3f, 1f, (float)Math.Sin(projectile.localAI[0] / 10f)), 1f), SpriteEffects.None, 0);
            //spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), new Color(54, 255, 64), projectile.rotation, drawOrigin, new Vector2(MathHelper.Lerp(0.3f, 1f, (float)Math.Sin(projectile.localAI[0] / 10f)), 1f), SpriteEffects.None, 0);
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), new Color(54, 255, 64) * 0.5f, projectile.rotation + MathHelper.PiOver2, drawOrigin, new Vector2(0.4f, 2f), SpriteEffects.None, 0);
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), new Color(54, 255, 64) * 0.5f, projectile.rotation, drawOrigin, new Vector2(0.4f, 1f), SpriteEffects.None, 0);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D SoulTexture = ModContent.GetTexture("OvermorrowMod/Textures/Extra_89");

            Main.spriteBatch.Draw(SoulTexture, projectile.Center - Main.screenPosition, null, new Color(0, 255, 191), projectile.rotation + MathHelper.PiOver2, SoulTexture.Size() / 2, new Vector2(0.5f, 1), SpriteEffects.None, 0f);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            Vector2 origin = projectile.Center;
            float radius = 5;
            int numLocations = 3;

            for (int i = 0; i < 3; i++)
            {
                Vector2 position = origin + Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                Vector2 dustvelocity = new Vector2(0f, 0.5f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * 2;

                Particle.CreateParticle(Particle.ParticleType<Glow>(), position, dustvelocity, Color.Cyan, 1, 0.5f, MathHelper.ToRadians(360f / numLocations * i), 1f);
            }
        }


    }
}