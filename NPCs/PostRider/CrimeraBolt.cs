using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.PostRider
{
    public class CrimeraBolt : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.RubyBolt);
            aiType = ProjectileID.RubyBolt;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 150;
        }
        public override string Texture => "Terraria/Projectile_" + ProjectileID.RubyBolt;

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Projectile.NewProjectile(target.Center.X, target.Center.Y, -projectile.velocity.X, -projectile.velocity.Y, mod.ProjectileType("BloodHealNPC"), 2, 1f, projectile.owner, 0f);
        }
    }
}