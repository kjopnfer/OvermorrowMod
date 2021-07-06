using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Buffs.Debuffs;

namespace OvermorrowMod.Projectiles.Summon
{
    public class SpritFlame : ModProjectile
    {
        private int length = 1;
        private int timer = 0;


        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.timeLeft = 17;
            projectile.penetrate = 1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {

            projectile.rotation += 0.3f;

            projectile.ai[0]++;
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 3f)
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width + Main.rand.Next(-25, 26), projectile.height + Main.rand.Next(-25, 26), 91, projectile.velocity.X * 0.9f, projectile.velocity.Y * 0.9f, 75, new Color(), 2.7f);
                Main.dust[num1110].position = (Main.dust[num1110].position + projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.2f;
            }
        }
    }
}
