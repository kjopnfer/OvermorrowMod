using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.EvilBoss
{
    public class CrystalBulletRang : ModProjectile
    {

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 16;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 63;
            projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            projectile.knockBack = 0;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.velocity.X = 6;
        }


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Bullet");
        }
    }
}