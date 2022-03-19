using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.WarpRocket
{
    public class TPproj : ModProjectile
    {
        Vector2 SavedPlyPos;
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 500;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acorn");
        }

        public override void AI()
        {
            SavedPlyPos = Main.player[projectile.owner].Center;
            projectile.velocity.X *= 0.98f;
            projectile.velocity.Y += 0.45f;
            projectile.rotation = projectile.velocity.ToRotation();



            float num116 = 16f;
            for (int num117 = 0; (float)num117 < 16; num117++)
            {
                Vector2 spinningpoint7 = Vector2.UnitX * 0f;
                spinningpoint7 += -Vector2.UnitY.RotatedBy((float)num117 * ((float)Math.PI * 2f / num116)) * new Vector2(1f, 4f);
                spinningpoint7 = spinningpoint7.RotatedBy(projectile.velocity.ToRotation());
                Vector2 position = projectile.Center;
                Dust dust = Terraria.Dust.NewDustPerfect(position, 10, new Vector2(0f, 0f), 0, new Color(), 1f);
                dust.noLight = true;
                dust.noGravity = true;
                dust.position = projectile.Center + spinningpoint7;
            }


        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.Center = SavedPlyPos;
        }



        public override void Kill(int timeLeft)
        {
            Main.player[projectile.owner].AddBuff(32, 60, true);
            Main.player[projectile.owner].AddBuff(36, 60, true);
            Main.player[projectile.owner].immuneTime = 5;
            Main.player[projectile.owner].immune = true;
            Vector2 position = projectile.Center + new Vector2(0, -10);
            int radius = 5;     //this is the explosion radius, the highter is the value the bigger is the explosion

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {

                    if (Math.Sqrt(x * x + y * y) <= radius + 0.5)   //this make so the explosion radius is a circle
                    {
                        Color alpha = Color.LightBlue;
                        Dust.NewDust(position, 5, 5, DustID.Enchanted_Gold, 0.0f, 0.0f, 120, alpha, 1f);
                    }
                }
            }
            Main.player[projectile.owner].position.X = projectile.Center.X - Main.player[projectile.owner].width;
            Main.player[projectile.owner].position.Y = projectile.Center.Y - Main.player[projectile.owner].height - 10;
            Main.PlaySound(SoundID.Item6, projectile.Center);
        }
    }
}