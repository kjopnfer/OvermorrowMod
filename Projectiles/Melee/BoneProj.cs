using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Melee
{
    public class BoneProj : ModProjectile
    {
        private int length = 1;
        private int timer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bone");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 18;
            projectile.timeLeft = 50;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            timer++;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            {
                projectile.velocity.X = -projectile.velocity.X;
                projectile.velocity.Y = -projectile.velocity.Y;
            }
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.velocity.X *= -1;
            projectile.velocity.Y *= -1;
        }
    }
}
