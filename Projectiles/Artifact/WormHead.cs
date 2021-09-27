using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using OvermorrowMod.WardenClass;

namespace OvermorrowMod.Projectiles.Artifact
{
    public class WormHead : ArtifactProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corruptor");
        }

        public override void SafeSetDefaults()
        {
            projectile.width = 30;
            projectile.height = 42;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 900;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 20;

        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;
            if (projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref projectile.velocity);
                projectile.localAI[0] = 1f;
            }
            Vector2 move = Vector2.Zero;
            float distance = 600f;
            bool target = false;
            for (int k = 0; k < 200; k++)
            {
                if (Main.npc[k].CanBeChasedBy(this))
                {
                    Vector2 newMove = Main.npc[k].Center - projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance)
                    {
                        move = newMove;
                        distance = distanceTo;
                        target = true;
                    }
                }
            }

            if (target)
            {
                AdjustMagnitude(ref move);
                projectile.velocity += (5 * projectile.velocity + move) / 11f;
                AdjustMagnitude(ref projectile.velocity);
            }

            if (projectile.velocity.X > 6)
            {
                projectile.velocity.X = 6;
            }
            if (projectile.velocity.X < -6)
            {
                projectile.velocity.X = -6;
            }

            if (projectile.velocity.Y > 6)
            {
                projectile.velocity.Y = 6;
            }
            if (projectile.velocity.Y < -6)
            {
                projectile.velocity.Y = -6;
            }

            projectile.velocity.Y = projectile.velocity.Y + 0.06f;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            projectile.ai[0]++;
            if (projectile.ai[0] == 3)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X / 7, projectile.velocity.Y / 7, ModContent.ProjectileType<WormBody>(), projectile.damage / 2, 1f, projectile.owner, 0f);
                projectile.ai[0] = 0;
            }
        }


        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 6f)
            {
                vector *= 6f / magnitude;
            }
        }

        public override void Kill(int timeLeft)
        {
            Vector2 origin = projectile.Center;
            float radius = 10;
            int numLocations = 30;
            for (int i = 0; i < 30; i++)
            {
                Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                int dust = Dust.NewDust(position, 2, 2, DustID.ChlorophyteWeapon, dustvelocity.X, dustvelocity.Y, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }

            Main.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost);
        }
    }
}