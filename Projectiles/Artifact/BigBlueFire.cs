using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Particles;
using OvermorrowMod.WardenClass;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Artifact
{
    public class BigBlueFire : ArtifactProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Torch God's Wrath");
            Main.projFrames[projectile.type] = 9;
        }

        public override void SafeSetDefaults()
        {
            projectile.width = 30;
            projectile.height = 31;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 100;

            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.localAI[0]++;

            Dust dust;
            Vector2 position = projectile.Center;
            dust = Terraria.Dust.NewDustPerfect(position, 59, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1.5f);
            dust.noGravity = true;


            if (projectile.localAI[0] % 30 == 0)
            {
                DelegateMethods.v3_1 = new Vector3(0.6f, 1f, 1f) * 0.2f;
                Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * 10f, 8f, DelegateMethods.CastLightOpen);
                float num108 = 16f;
                for (int num109 = 0; (float)num109 < num108; num109++)
                {
                    Vector2 spinningpoint5 = Vector2.UnitX * 0f;
                    spinningpoint5 += -Vector2.UnitY.RotatedBy((float)num109 * ((float)Math.PI * 2f / num108)) * new Vector2(1f, 4f);
                    spinningpoint5 = spinningpoint5.RotatedBy(projectile.velocity.ToRotation());
                    int num110 = Dust.NewDust(projectile.Center, 0, 0, 158);
                    Main.dust[num110].scale = 1.5f;
                    Main.dust[num110].noGravity = true;
                    Main.dust[num110].position = projectile.Center + spinningpoint5;
                    Main.dust[num110].velocity = projectile.velocity * 0f + spinningpoint5.SafeNormalize(Vector2.UnitY) * 1f;
                }
            }

            // No glow gradient texture because projectile centering decides to be annoying
            // And I don't want to bother with figuring out the nuances of how the game decides to center the sprite for some ungodly reason

            projectile.rotation = projectile.velocity.ToRotation();

            if (++projectile.frameCounter >= 4)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }

        }


        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void Kill(int timeLeft)
        {
            Particle.CreateParticle(Particle.ParticleType<Shockwave>(), projectile.Center, Vector2.Zero, Color.LightBlue, 1, 0.5f, 0, 1f);
            Particle.CreateParticle(Particle.ParticleType<Shockwave>(), projectile.Center, Vector2.Zero, Color.Cyan, 1, 1, 0, 1f);

            int num651 = Main.rand.Next(4, 10);
            for (int num652 = 0; num652 < num651; num652++)
            {
                int num653 = Dust.NewDust(projectile.Center, 0, 0, Main.rand.NextBool(2) ? 180 : 6, 0f, 0f, 100);
                Dust dust = Main.dust[num653];
                dust.velocity *= 1.6f;
                Main.dust[num653].velocity.Y -= 1f;
                dust = Main.dust[num653];
                dust.velocity += -projectile.velocity * (Main.rand.NextFloat() * 2f - 1f) * 0.5f;
                Main.dust[num653].scale = 2f;
                Main.dust[num653].fadeIn = 0.5f;
                Main.dust[num653].noGravity = true;
            }

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                float distance = Vector2.Distance(projectile.Center, Main.player[i].Center);
                if (distance <= 1050 && projectile.ai[0] > -1)
                {
                    Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 15;
                }
            }

            Main.PlaySound(SoundID.Item14);
        }
    }

    public class SmallBlueFire : ArtifactProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Torch God's Wrath");
            Main.projFrames[projectile.type] = 9;
        }

        public override void SafeSetDefaults()
        {
            projectile.width = 15;
            projectile.height = 15;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 100;

            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.localAI[0]++;

            Dust dust;
            Vector2 position = projectile.Center;
            dust = Terraria.Dust.NewDustPerfect(position, 59, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1.5f);
            dust.noGravity = true;

            if (projectile.localAI[0] % 30 == 0)
            {
                DelegateMethods.v3_1 = new Vector3(0.6f, 1f, 1f) * 0.2f;
                Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * 10f, 8f, DelegateMethods.CastLightOpen);
                float num108 = 16f;
                for (int num109 = 0; (float)num109 < num108; num109++)
                {
                    Vector2 spinningpoint5 = Vector2.UnitX * 0f;
                    spinningpoint5 += -Vector2.UnitY.RotatedBy((float)num109 * ((float)Math.PI * 2f / num108)) * new Vector2(1f, 4f);
                    spinningpoint5 = spinningpoint5.RotatedBy(projectile.velocity.ToRotation());
                    int num110 = Dust.NewDust(projectile.Center, 0, 0, 158);
                    Main.dust[num110].scale = 1.5f;
                    Main.dust[num110].noGravity = true;
                    Main.dust[num110].position = projectile.Center + spinningpoint5;
                    Main.dust[num110].velocity = projectile.velocity * 0f + spinningpoint5.SafeNormalize(Vector2.UnitY) * 1f;
                }
            }

            projectile.rotation = projectile.velocity.ToRotation();

            if (++projectile.frameCounter >= 4)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }

        }


        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void Kill(int timeLeft)
        {
            Particle.CreateParticle(Particle.ParticleType<Shockwave>(), projectile.Center, Vector2.Zero, Color.Cyan, 1, 0.35f, 0, 1f);

            int num651 = Main.rand.Next(4, 10);
            for (int num652 = 0; num652 < num651; num652++)
            {
                int num653 = Dust.NewDust(projectile.Center, 0, 0, Main.rand.NextBool(2) ? 180 : 6, 0f, 0f, 100);
                Dust dust = Main.dust[num653];
                dust.velocity *= 1.6f;
                Main.dust[num653].velocity.Y -= 1f;
                dust = Main.dust[num653];
                dust.velocity += -projectile.velocity * (Main.rand.NextFloat() * 2f - 1f) * 0.5f;
                Main.dust[num653].scale = 2f;
                Main.dust[num653].fadeIn = 0.5f;
                Main.dust[num653].noGravity = true;
            }
        }
    }
}