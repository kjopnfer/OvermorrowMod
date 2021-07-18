using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Buffs.Summon;
using OvermorrowMod.Projectiles.Accessory;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Projectiles.Accessory
{
    public class GraniteShield : ModProjectile
    {
        private int maxStored;
        private int storedDamage = 0;
        private int disabledCounter = 0;
        private bool disabledReflect = false;
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
            if (NPC.downedPlantBoss) 
            {
                maxStored = 40;
            }
            else if (Main.hardMode)
            {
                maxStored = 20;
            }
            else
            {
                maxStored = 10;
            }

            if (storedDamage >= maxStored)
            {
                Main.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
                disabledReflect = true;
                disabledCounter = 255;
                projectile.alpha = 255;
                storedDamage = 0;
            }

            if (disabledCounter > 0)
            {
                projectile.alpha--;
                disabledCounter--;
            }
            else
            {
                if (disabledReflect)
                {
                    Main.PlaySound(SoundID.Item70);
                    disabledReflect = false;
                }
            }


            //Factors for calculations
            double rad = (Main.player[projectile.owner].Center - Main.MouseWorld).ToRotation(); // rotation
            double dist = 65; // distance from the owner

            projectile.position.X = (Main.player[projectile.owner].Center.X - (int)(Math.Cos(rad) * dist) - projectile.width / 2) + 7;
            projectile.position.Y = Main.player[projectile.owner].Center.Y - (int)(Math.Sin(rad) * dist) - projectile.height / 2;

            projectile.rotation = (float)rad;

            if (disabledCounter == 0)
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

                            storedDamage += 1;

                            incomingProjectile.damage *= 2;
                        }
                    }
                }
            }
        }
    }
}
