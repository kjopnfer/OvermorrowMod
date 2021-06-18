using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace OvermorrowMod.NPCs.PostRider
{
    public class AngryFrostWave : ModProjectile
    {
        

        public override void SetDefaults()
        {
            projectile.width = 46;
            projectile.height = 80;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.scale = 1f;
            projectile.alpha = 0;
            projectile.timeLeft = 30;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(44, 120);
        }
    }
}