using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.GraniteMini
{
    public class GranLaser : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Magic/GraniteSpike";
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
                float num116 = 16f;
                for (int num117 = 0; (float)num117 < 16; num117++)
                {
                    Vector2 spinningpoint7 = new Vector2(-20, -2);
                    spinningpoint7 += -Vector2.UnitY.RotatedBy((float)num117 * ((float)Math.PI * 2f / num116)) * new Vector2(1f, 4f);
                    spinningpoint7 = spinningpoint7.RotatedBy(projectile.velocity.ToRotation());
                    Vector2 position = projectile.Center;
                    Dust dust = Dust.NewDustPerfect(position, 63, new Vector2(0f, 0f), 0, new Color(0, 242, 255), 1f);
                    dust.noLight = true;
                    dust.noGravity = true;
                    dust.position = projectile.Center + spinningpoint7;
                }
            }
            Lighting.AddLight(projectile.Center, 0.5f, 0.5f, 0);
        }
    }
}