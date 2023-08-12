using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.TreeGuns
{
    public class SnotRocket : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.timeLeft = 200;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            DrawOffsetX = -6;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Terraria.Dust.NewDustPerfect(Projectile.Center, 46, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
        }

        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SnotExplosion>(), Projectile.damage, 10f, Projectile.owner);
        }
    }

    public class SnotExplosion : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            Projectile.alpha = 255;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3;
        }

        public override void Kill(int timeLeft)
        {
            Vector2 position = Projectile.Center;
            SoundEngine.PlaySound(SoundID.NPCDeath1, position);

            for (int num864 = 0; num864 < 30; num864++)
            {
                int num865 = Dust.NewDust(new Vector2(position.X, position.Y), Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default(Color), 1.5f);
                Dust dust = Main.dust[num865];
                dust.velocity *= 1.4f;
            }
            for (int num866 = 0; num866 < 20; num866++)
            {
                int num867 = Dust.NewDust(new Vector2(position.X, position.Y), Projectile.width, Projectile.height, DustID.ChlorophyteWeapon, 0f, 0f, 100, default(Color), 2.5f);
                Main.dust[num867].noGravity = true;
                Dust dust = Main.dust[num867];
                dust.velocity *= 3.5f;
                num867 = Dust.NewDust(new Vector2(position.X, position.Y), Projectile.width, Projectile.height, DustID.ChlorophyteWeapon, 0f, 0f, 100, default(Color), 1f);
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
                var source = Projectile.GetSource_Death();
                int num870 = Gore.NewGore(source, new Vector2(position.X, position.Y), default(Vector2), Main.rand.Next(61, 64));
                Gore gore = Main.gore[num870];
                gore.velocity *= num869;
                Main.gore[num870].velocity.X += 1f;
                Main.gore[num870].velocity.Y += 1f;
                num870 = Gore.NewGore(source, new Vector2(position.X, position.Y), default(Vector2), Main.rand.Next(61, 64));
                gore = Main.gore[num870];
                gore.velocity *= num869;
                Main.gore[num870].velocity.X -= 1f;
                Main.gore[num870].velocity.Y += 1f;
                num870 = Gore.NewGore(source, new Vector2(position.X, position.Y), default(Vector2), Main.rand.Next(61, 64));
                gore = Main.gore[num870];
                gore.velocity *= num869;
                Main.gore[num870].velocity.X += 1f;
                Main.gore[num870].velocity.Y -= 1f;
                num870 = Gore.NewGore(source, new Vector2(position.X, position.Y), default(Vector2), Main.rand.Next(61, 64));
                gore = Main.gore[num870];
                gore.velocity *= num869;
                Main.gore[num870].velocity.X -= 1f;
                Main.gore[num870].velocity.Y -= 1f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 300);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Poisoned, 300);
        }
    }
}