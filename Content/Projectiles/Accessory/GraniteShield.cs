using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Summon;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Projectiles.Accessory
{
    public class GraniteShield : ModProjectile
    {
        public override bool? CanDamage() => false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Shield");
        }

        public override void SetDefaults()
        {
            Projectile.width = 55;
            Projectile.height = 55;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.HasBuff(ModContent.BuffType<GraniteShieldBuff>()))
            {
                Projectile.timeLeft = 2;
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

            if (Projectile.localAI[0] >= maxStored)
            {
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
                Projectile.ai[0] = 1;
                Projectile.ai[1] = 255;
                Projectile.alpha = 255;
                Projectile.localAI[0] = 0;
            }

            if (Projectile.alpha > 0 && Projectile.alpha < 255)
            {
                Projectile.alpha -= 10;
            }

            if (Projectile.ai[1] > 0)
            {
                Projectile.ai[1]--;
            }
            else
            {
                if (Projectile.ai[0] == 1)
                {
                    SoundEngine.PlaySound(SoundID.Item70);
                    Projectile.ai[0] = 0;
                    Projectile.alpha = 250;
                }
            }


            //Factors for calculations
            Vector2 direction = player.DirectionTo(Main.MouseWorld);
            float dist = 65f;

            Projectile.Center = player.Center + direction * dist;
            Projectile.rotation = (float)direction.ToRotation();

            if (Projectile.ai[1] == 0)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile incomingProjectile = Main.projectile[i];
                    if (incomingProjectile.active && incomingProjectile.hostile)
                    {
                        if (Projectile.Hitbox.Intersects(incomingProjectile.Hitbox))
                        {
                            incomingProjectile.velocity *= -1;
                            incomingProjectile.friendly = true;
                            incomingProjectile.hostile = false;

                            Projectile.localAI[0] += 1;

                            incomingProjectile.damage *= 2;
                        }
                    }
                }
            }
        }
    }
}
