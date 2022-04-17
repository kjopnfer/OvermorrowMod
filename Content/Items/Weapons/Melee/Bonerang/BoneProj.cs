using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.Bonerang
{
    public class BoneProj : ModProjectile
    {
        private int timer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bone");
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 18;
            Projectile.timeLeft = 50;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            timer++;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            {
                Projectile.velocity.X = -Projectile.velocity.X;
                Projectile.velocity.Y = -Projectile.velocity.Y;
            }
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.velocity.X *= -1;
            Projectile.velocity.Y *= -1;
        }
    }
}
