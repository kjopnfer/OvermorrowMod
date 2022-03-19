using OvermorrowMod.Content.Buffs.Debuffs;
using OvermorrowMod.Projectiles.Boss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public class OvermorrowGlobalProjectile : GlobalProjectile
    {
        // Life is pain
        public override bool InstancePerEntity => true;

        private bool spawnedBlood = false;
        public bool slowedTime = false;

        public override bool PreAI(Projectile projectile)
        {
            if (slowedTime && !projectile.friendly)
            {
                projectile.position -= projectile.velocity * 0.95f;
            }

            return base.PreAI(projectile);
        }

        public override void SetDefaults(Projectile projectile)
        {
            if (projectile.type == 590)
            {
                projectile.timeLeft = 100;
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.type == 130 || projectile.type == 131)
            {
                target.AddBuff(ModContent.BuffType<FungalInfection>(), 500);
            }
            if (projectile.type == 30)
            {
                target.immune[projectile.owner] = 0;
            }
        }

        public override void Kill(Projectile projectile, int timeLeft)
        {
            Player player = Main.player[projectile.owner];
            var modPlayer = player.GetModPlayer<OvermorrowModPlayer>();
            if (modPlayer.ShatteredOrb && !spawnedBlood && projectile.magic)
            {
                // This thing is absolutely insane without the explosion chance
                if (Main.rand.NextBool(6))
                {
                    // I'm lazy
                    // Determine if the projectiles go straight or at an angle
                    int randChoice = Main.rand.Next(2);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (randChoice == 0)
                        {
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, -6f, 6f, ModContent.ProjectileType<BloodyBallFriendly>(), projectile.damage / 4, 2f, projectile.owner, 0f, 0f);
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 6, 6f, ModContent.ProjectileType<BloodyBallFriendly>(), projectile.damage / 4, 2f, projectile.owner, 0f, 0f);
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 6f, -6f, ModContent.ProjectileType<BloodyBallFriendly>(), projectile.damage / 4, 2f, projectile.owner, 0f, 0f);
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, -6, -6f, ModContent.ProjectileType<BloodyBallFriendly>(), projectile.damage / 4, 2f, projectile.owner, 0f, 0f);
                        }
                        else
                        {
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 6f, ModContent.ProjectileType<BloodyBallFriendly>(), projectile.damage / 4, 2f, projectile.owner, 0f, 0f);
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 6, 0f, ModContent.ProjectileType<BloodyBallFriendly>(), projectile.damage / 4, 2f, projectile.owner, 0f, 0f);
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, -6f, ModContent.ProjectileType<BloodyBallFriendly>(), projectile.damage / 4, 2f, projectile.owner, 0f, 0f);
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, -6, 0f, ModContent.ProjectileType<BloodyBallFriendly>(), projectile.damage / 4, 2f, projectile.owner, 0f, 0f);
                        }
                    }
                    spawnedBlood = true;
                }
            }
        }
    }
}