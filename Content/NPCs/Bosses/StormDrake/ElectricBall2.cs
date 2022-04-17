using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.StormDrake
{
    public class ElectricBall2 : ModProjectile, ITrailEntity
    {
        public override string Texture => AssetDirectory.Empty;

        private Vector2 center;
        public int direction;
        public float Multiplier = 0.75f;
        public Color TrailColor(float progress) => Color.Cyan;
        public float TrailSize(float progress) => 48;
        public Type TrailType()
        {
            return typeof(LightningTrail);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Ball");
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 265;//400;//450;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                center = Projectile.Center;
            }

            Lighting.AddLight(Projectile.Center, 0, 0.5f, 0.5f);
            int num434 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Vortex, 0f, 0f, 100);
            Main.dust[num434].noLight = true;
            Main.dust[num434].noGravity = true;
            Main.dust[num434].velocity = Projectile.velocity;
            Main.dust[num434].position -= Vector2.One * 4f;
            Main.dust[num434].scale = 0.8f;

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            Projectile.position = center + new Vector2(Projectile.ai[0], 0).RotatedBy(MathHelper.ToRadians(Projectile.ai[1] * direction));
            Projectile.position -= new Vector2(Projectile.width / 2, Projectile.height / 2);

            Projectile.ai[0] += 6.5f * Multiplier;//3;//2.5f;//2;//1;
            Projectile.ai[1] += 1.125f * Multiplier;//1.25f;//1.5f;//2;//1;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Electrified, Main.expertMode ? 120 : 60);
        }
    }
}