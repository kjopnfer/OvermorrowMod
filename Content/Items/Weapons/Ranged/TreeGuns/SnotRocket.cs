using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.TreeGuns
{
    public class SnotRocket : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.timeLeft = 200;
            projectile.penetrate = 1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.tileCollide = true;
            drawOffsetX = -6;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();

            Terraria.Dust.NewDustPerfect(projectile.Center, 46, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
        }

        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<SnotExplosion>(), projectile.damage, 10f, projectile.owner);
        }
    }

    public class SnotExplosion : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            projectile.alpha = 255;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.width = 64;
            projectile.height = 64;
            projectile.penetrate = -1;
            projectile.timeLeft = 3;
        }

        public override void Kill(int timeLeft)
        {

            Vector2 position = projectile.Center;
            Main.PlaySound(SoundID.NPCDeath1, (int)position.X, (int)position.Y);

            for (int num864 = 0; num864 < 30; num864++)
            {
                int num865 = Dust.NewDust(new Vector2(position.X, position.Y), projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 100, default(Color), 1.5f);
                Dust dust = Main.dust[num865];
                dust.velocity *= 1.4f;
            }
            for (int num866 = 0; num866 < 20; num866++)
            {
                int num867 = Dust.NewDust(new Vector2(position.X, position.Y), projectile.width, projectile.height, DustID.ChlorophyteWeapon, 0f, 0f, 100, default(Color), 2.5f);
                Main.dust[num867].noGravity = true;
                Dust dust = Main.dust[num867];
                dust.velocity *= 3.5f;
                num867 = Dust.NewDust(new Vector2(position.X, position.Y), projectile.width, projectile.height, DustID.ChlorophyteWeapon, 0f, 0f, 100, default(Color), 1f);
                dust = Main.dust[num867];
                dust.velocity *= 1.5f;
            }
            for (int num868 = 0; num868 < 2; num868++)
            {
                float num869 = 0.4f;
                if (num868 == 1)
                {
                    num869 = 0.8f;
                }
                int num870 = Gore.NewGore(new Vector2(position.X, position.Y), default(Vector2), Main.rand.Next(61, 64));
                Gore gore = Main.gore[num870];
                gore.velocity *= num869;
                Main.gore[num870].velocity.X += 1f;
                Main.gore[num870].velocity.Y += 1f;
                num870 = Gore.NewGore(new Vector2(position.X, position.Y), default(Vector2), Main.rand.Next(61, 64));
                gore = Main.gore[num870];
                gore.velocity *= num869;
                Main.gore[num870].velocity.X -= 1f;
                Main.gore[num870].velocity.Y += 1f;
                num870 = Gore.NewGore(new Vector2(position.X, position.Y), default(Vector2), Main.rand.Next(61, 64));
                gore = Main.gore[num870];
                gore.velocity *= num869;
                Main.gore[num870].velocity.X += 1f;
                Main.gore[num870].velocity.Y -= 1f;
                num870 = Gore.NewGore(new Vector2(position.X, position.Y), default(Vector2), Main.rand.Next(61, 64));
                gore = Main.gore[num870];
                gore.velocity *= num869;
                Main.gore[num870].velocity.X -= 1f;
                Main.gore[num870].velocity.Y -= 1f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 300);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 300);
        }
    }
}