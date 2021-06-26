using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.GraniteMini
{
    public class GraniteBolt : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.SapphireBolt);
            aiType = ProjectileID.SapphireBolt;
            projectile.tileCollide = false;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 120;
        }
        public override string Texture => "Terraria/Projectile_" + ProjectileID.SapphireBolt;
    }
}