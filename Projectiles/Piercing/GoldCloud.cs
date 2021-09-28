using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs;
using OvermorrowMod.NPCs.Bosses.StormDrake;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Piercing
{
    public class GoldCloud : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("HeavenCloud");
            Main.projFrames[projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.RainCloudRaining);
            aiType = 0;
            projectile.alpha = 255;
            projectile.timeLeft = 90;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<LightningCloud>());
            }

            if (player.HasBuff(ModContent.BuffType<LightningCloud>()))
            {
                projectile.timeLeft = 2;
            }

            projectile.Center = player.Center - new Vector2(0, MathHelper.Lerp(75, 85, (float)Math.Sin(projectile.ai[0] / 60f)));

            projectile.damage = 0;
            projectile.velocity = Vector2.Zero;

            if (projectile.ai[0] < 80)
            {
                projectile.alpha -= 4;
            }

            projectile.ai[0]++;

            if (projectile.ai[0] % 180 == 0)
            {
                Vector2 targetPos = projectile.position;
                float targetDist = 400f;
                bool target = false;
                for (int k = 0; k < 200; k++)
                {
                    NPC npc = Main.npc[k];
                    if (npc.CanBeChasedBy(this))
                    {
                        float distance = Vector2.Distance(npc.Center, projectile.Center);
                        if ((distance < targetDist || !target) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                        {
                            targetDist = distance;
                            targetPos = npc.Center;
                            target = true;
                        }
                    }
                }

                if (target)
                {
                    Vector2 shootVelocity = targetPos - projectile.Center;
                    shootVelocity.Normalize();
                    Projectile.NewProjectile(projectile.Center, shootVelocity, ModContent.ProjectileType<DivineLightning>(), 36, 10f, projectile.owner);

                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        float distance = Vector2.Distance(projectile.Center, Main.player[i].Center);
                        if (distance <= 1050)
                        {
                            Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 30;
                        }
                    }
                }
            }

            if (++projectile.frameCounter >= 8)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }
        }
    }
}