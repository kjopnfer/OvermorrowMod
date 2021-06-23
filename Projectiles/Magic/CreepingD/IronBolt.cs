using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Projectiles.Magic.CreepingD
{
    public class IronBolt : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.timeLeft = 150;
            projectile.width = 14;
            projectile.height = 14;
        }
        public override string Texture => "Terraria/Projectile_" + ProjectileID.SlushBall;


        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Color alpha = Color.White;
            {
                Dust.NewDust(projectile.Center, 0, 0, 184, projectile.oldVelocity.X, projectile.oldVelocity.Y, 0, alpha, 1f);
            }
        }
    }
}