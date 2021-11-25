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
        private bool initProperties = true;
        
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
            if (initProperties)
            {
                if (Main.netMode != NetmodeID.Server && projectile.owner == Main.myPlayer)
                {
                    // This spawns the child projectile that travels in the opposite direction
                    if (projectile.ai[0] == 0)
                    {
                        Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<SmallBlueFire>(), projectile.damage, projectile.knockBack, projectile.owner, 0, projectile.whoAmI);
                    }
                }
                initProperties = false;
            }

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

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<TorchAoE>(), projectile.damage, 6f, projectile.owner);

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                float distance = Vector2.Distance(projectile.Center, Main.player[i].Center);
                if (distance <= 1050)
                {
                    Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 15;
                }
            }

            Main.PlaySound(SoundID.Item14);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 60 * 15);
            base.OnHitNPC(target, damage, knockback, crit);
        }
    }

    public class SmallBlueFire : ArtifactProjectile
    {
        private bool initProperties = true;
        private float storeDirection;
        private float trigCounter = 0;
        private float period = 60;
        private float amplitude = 40;
        private float previousR = 0;
        private Projectile childProjectile;

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
            if (initProperties)
            {
                storeDirection = projectile.velocity.ToRotation();
                if (Main.netMode != NetmodeID.Server && projectile.owner == Main.myPlayer)
                {
                    // This spawns the child projectile that travels in the opposite direction
                    if (projectile.ai[0] == 0)
                    {
                        childProjectile = Main.projectile[Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<SmallBlueFire>(), projectile.damage, projectile.knockBack, projectile.owner, 1, projectile.whoAmI)];
                    }
                    else // This is the check that the child projectile enters in
                    {
                        childProjectile = Main.projectile[(int)projectile.ai[1]];
                    }
                }
                initProperties = false;
            }

            trigCounter += 2 * (float)Math.PI / period;
            float r = amplitude * (float)Math.Sin(trigCounter) * (projectile.ai[0] == 0 ? 1 : -1);
            Vector2 instaVel = PolarVector(r - previousR, storeDirection + (float)Math.PI / 2);
            projectile.position += instaVel;
            previousR = r;
            projectile.rotation = (projectile.velocity + instaVel).ToRotation();

            projectile.localAI[0]++;

            Dust dust;
            Vector2 position = projectile.Center;
            dust = Terraria.Dust.NewDustPerfect(position, 59, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1.5f);
            dust.noGravity = true;

            if (projectile.localAI[0] % 15 == 0)
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

            //projectile.rotation = projectile.velocity.ToRotation();

            if (++projectile.frameCounter >= 4)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }

        }
        public Vector2 PolarVector(float radius, float theta)
        {
            return new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * radius;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<TorchAoE>(), projectile.damage, 6f, projectile.owner, 1f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 60 * 15);

            base.OnHitNPC(target, damage, knockback, crit);
        }
    }
}