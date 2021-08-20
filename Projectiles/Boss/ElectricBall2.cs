using System;
using Microsoft.Xna.Framework;
using OvermorrowMod.Effects.Prim;
using OvermorrowMod.Effects.Prim.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Boss
{
    public class ElectricBall2 : ModProjectile, ITrailEntity
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";

        private Vector2 center;
        public int direction;
        public float multiplier = 0.75f;
        public Type TrailType()
        {
            return typeof(LightningTrail);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Ball");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 38;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 375;//400;//450;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                center = projectile.Center;
            }

            Lighting.AddLight(projectile.Center, 0, 0.5f, 0.5f);
            int num434 = Dust.NewDust(projectile.Center, 0, 0, 229, 0f, 0f, 100);
            Main.dust[num434].noLight = true;
            Main.dust[num434].noGravity = true;
            Main.dust[num434].velocity = projectile.velocity;
            Main.dust[num434].position -= Vector2.One * 4f;
            Main.dust[num434].scale = 0.8f;

            if (++projectile.frameCounter >= 5)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }

            projectile.position = center + new Vector2(projectile.ai[0], 0).RotatedBy(MathHelper.ToRadians(projectile.ai[1] * direction));
            projectile.position -= new Vector2(projectile.width / 2, projectile.height / 2);

            projectile.ai[0] += 3.5f * multiplier;//3;//2.5f;//2;//1;
            projectile.ai[1] += 1.125f * multiplier;//1.25f;//1.5f;//2;//1;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft >= 60)
            {
                return Color.White;
            }
            else
            {
                return null;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Electrified, Main.expertMode ? 180 : 90);
        }
    }
}