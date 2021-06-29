using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.GraniteMini
{
    public class GranLaser : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.EyeBeam);
            aiType = ProjectileID.EyeBeam;
            projectile.tileCollide = false;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 160;
        }
        public override void AI()
        {
            if (++projectile.ai[1] % 2 == 0)
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center, 57, new Vector2(0f, 0f), 0, new Color(11, 177, 214), 1f);
            }
            Lighting.AddLight(projectile.Center, 0.5f, 0.5f, 0);
        }
    }
}