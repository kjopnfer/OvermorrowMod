using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic
{
    public class SporeMagic : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spore");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.timeLeft = 200;
            projectile.penetrate = 1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }



        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(20, 60);
        }

        public override void AI()
        {
            projectile.rotation += 0.3f;

            projectile.ai[0]++;
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 3f)
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.JungleGrass, projectile.velocity.X, projectile.velocity.Y, 75, new Color(), 1.4f);
                Main.dust[num1110].position = (Main.dust[num1110].position + projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            {
                Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
                Main.PlaySound(SoundID.Item10, projectile.position);
                projectile.velocity.X = -projectile.velocity.X;
                projectile.velocity.Y = -projectile.velocity.Y;
            }
            return false;
        }
    }
}