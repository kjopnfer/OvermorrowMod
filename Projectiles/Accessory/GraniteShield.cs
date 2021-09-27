using OvermorrowMod.Buffs.Summon;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Accessory
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
            if (Main.player[projectile.owner].HasBuff(ModContent.BuffType<SpiderWebBuff>()))
            {
                projectile.timeLeft = 2;
            }

            // Determines the maximum damage stored within the shield before it breaks
            float maxStored = 10;
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

            if (projectile.ai[1] > 0)
            {
                projectile.alpha--;
                projectile.ai[1]--;
            }
            else
            {
                if (projectile.ai[0] == 1)
                {
                    Main.PlaySound(SoundID.Item70);
                    projectile.ai[0] = 0;
                }
            }


            //Factors for calculations
            double rad = (Main.player[projectile.owner].Center - Main.MouseWorld).ToRotation(); // rotation
            double dist = 65; // distance from the owner

            projectile.position.X = (Main.player[projectile.owner].Center.X - (int)(Math.Cos(rad) * dist) - projectile.width / 2) + 7;
            projectile.position.Y = Main.player[projectile.owner].Center.Y - (int)(Math.Sin(rad) * dist) - projectile.height / 2;

            projectile.rotation = (float)rad;

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
