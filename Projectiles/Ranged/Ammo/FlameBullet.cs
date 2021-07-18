using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Buffs.Debuffs;

namespace OvermorrowMod.Projectiles.Ranged.Ammo
{
    public class FlameBullet : ModProjectile
    {
        private int length = 1;
        private int timer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flame");
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.timeLeft = 30;
            projectile.penetrate = 3;
            projectile.alpha = 255;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = false;
        }
        public override void AI()
        {

            projectile.ai[0]++;
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 3f)
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width + Main.rand.Next(-30, 31), projectile.height + Main.rand.Next(-30, 31), 6, projectile.velocity.X, projectile.velocity.Y, 75, new Color(), 2.4f);
                Main.dust[num1110].position = (Main.dust[num1110].position + projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.2f;
            }
        }




        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.Next(2) == 1)
            {
                target.AddBuff(24, 400);
            }
            projectile.damage /=2;
        }
    }
}
