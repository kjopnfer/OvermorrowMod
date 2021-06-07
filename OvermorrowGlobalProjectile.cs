using OvermorrowMod.Projectiles.Boss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod
{
    public class OvermorrowGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        private bool spawnedBlood = false;

        public override void Kill(Projectile projectile, int timeLeft)
        {
            Player player = Main.player[projectile.owner];
            var modPlayer = player.GetModPlayer<OvermorrowModPlayer>();
            if (modPlayer.ShatteredOrb && !spawnedBlood && projectile.magic)
            {
                int explodeChance = Main.rand.Next(6);

                // This thing is absolutely insane without the explosion chance
                if (explodeChance == 0)
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