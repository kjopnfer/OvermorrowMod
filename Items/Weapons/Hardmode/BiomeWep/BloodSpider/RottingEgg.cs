
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.Hardmode.BiomeWep.BloodSpider
{
    //ported from my tAPI mod because I'm lazy
    public class RottingEgg : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 46;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.hostile = false;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 150; //The amount of time the projectile is alive for
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Vector2 value1 = new Vector2(0f, 0f);
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X / 3f, projectile.velocity.Y / 3f, mod.ProjectileType("HealingBulletEgg"), projectile.damage, 1f, projectile.owner, 0f);
        }
    }
}

