using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Summon;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Projectiles.Accessory
{
    public class GraniteShield : ModProjectile
    {
        public override bool CanDamage() => false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Shield");
        }

        public override void SetDefaults()
        {
            projectile.width = 55;
            projectile.height = 55;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (player.HasBuff(ModContent.BuffType<GraniteShieldBuff>()))
            {
                projectile.timeLeft = 2;
            }

            // Determines the maximum damage stored within the shield before it breaks
            int maxStored = 10;
            if (NPC.downedPlantBoss)
            {
                maxStored = 40;
            }
            else if (Main.hardMode)
            {
                maxStored = 20;
            }

            if (projectile.localAI[0] >= maxStored)
            {
                Main.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
                projectile.ai[0] = 1;
                projectile.ai[1] = 255;
                projectile.alpha = 255;
                projectile.localAI[0] = 0;
            }

            if (projectile.alpha > 0 && projectile.alpha < 255)
            {
                projectile.alpha -= 10;
            }

            if (projectile.ai[1] > 0)
            {
                projectile.ai[1]--;
            }
            else
            {
                if (projectile.ai[0] == 1)
                {
                    Main.PlaySound(SoundID.Item70);
                    projectile.ai[0] = 0;
                    projectile.alpha = 250;
                }
            }


            //Factors for calculations
            Vector2 direction = player.DirectionTo(Main.MouseWorld);
            float dist = 65f;

            projectile.Center = player.Center + direction * dist;
            projectile.rotation = (float)direction.ToRotation();

            if (projectile.ai[1] == 0)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile incomingProjectile = Main.projectile[i];
                    if (incomingProjectile.active && incomingProjectile.hostile)
                    {
                        if (projectile.Hitbox.Intersects(incomingProjectile.Hitbox))
                        {
                            incomingProjectile.velocity *= -1;
                            incomingProjectile.friendly = true;
                            incomingProjectile.hostile = false;

                            projectile.localAI[0] += 1;

                            incomingProjectile.damage *= 2;
                        }
                    }
                }
            }
        }
    }
}
