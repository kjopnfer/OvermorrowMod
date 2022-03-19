using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.IorichStaff
{
    public class NatureBolt : ModProjectile
    {
        public enum SpiritPoints
        {
            East = 0,
            NorthEast = 1,
            North = 2,
            NorthWest = 3,
            West = 4,
            SouthWest = 5,
            South = 6,
            SouthEast = 7
        }
        private int RotationDirection;
        private float RotationOffset;
        private float RandomOffset;
        private int ReadyCount = 0;

        private NPC target = null;

        public override string Texture => AssetDirectory.Empty;
        public override bool CanDamage() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nature Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 420;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.magic = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (Main.LocalPlayer != player) return;

            if (target == null)
            {
                if (Main.myPlayer == projectile.owner) projectile.Center = Main.MouseWorld;
            }
            else
            {
                projectile.Center = target.Center;
            }

            Lighting.AddLight(projectile.Center, 0f, 1f, 0f);
            projectile.localAI[0] += 4; // Rotation counter
            int DUST_RADIUS = 15;

            if (player.HeldItem.type == ModContent.ItemType<IorichStaff>())
            {
                projectile.timeLeft = 5;
            }

            Dust dust = Terraria.Dust.NewDustPerfect(projectile.Center, 107, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
            dust.noGravity = true;

            if (Main.mouseRight && target == null)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && !npc.friendly && npc.getRect().Intersects(projectile.getRect()))
                    {
                        target = npc;
                        return;
                    }
                }
            }

            if (target != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 dustPos = projectile.Center + new Vector2(DUST_RADIUS, 0).RotatedBy(MathHelper.ToRadians(i * 10 + projectile.localAI[0]));
                    Dust dust2 = Dust.NewDustPerfect(dustPos, 107, Vector2.Zero, 0, default, 1f);
                    dust2.noGravity = true;
                }

                for (int i = 0; i < 2; i++)
                {
                    Vector2 dustPos = projectile.Center - new Vector2(DUST_RADIUS, 0).RotatedBy(MathHelper.ToRadians(i * 10 + projectile.localAI[0]));
                    Dust dust2 = Dust.NewDustPerfect(dustPos, 107, Vector2.Zero, 0, default, 1f);
                    dust2.noGravity = true;
                }

                if (player.channel && projectile.ai[0] < 8)
                {
                    if (projectile.ai[1]++ == 0)
                    {
                        RotationDirection = Main.rand.NextBool(2) ? 1 : -1;
                        RotationOffset = Main.rand.Next(4) * MathHelper.PiOver2;

                        projectile.netUpdate = true;
                        RandomOffset = Main.rand.Next(0, 10) * 40; // Random number between 0 and 360 with 40 degree increments
                    }

                    SpiritPoints[] values = (SpiritPoints[])Enum.GetValues(typeof(SpiritPoints));
                    if (projectile.ai[1] % 20 == 0)
                    {
                        int RADIUS = target.width + 155;

                        float Rotation = RotationDirection * (int)values[(int)projectile.ai[0]++] * MathHelper.PiOver4;
                        Vector2 SpawnLocation = new Vector2(RADIUS, 0).RotatedBy(Rotation + RotationOffset);

                        int proj = Projectile.NewProjectile(target.Center + SpawnLocation, Vector2.Zero, ModContent.ProjectileType<NatureSpike>(), projectile.damage, 0f, projectile.owner, Rotation + RotationOffset, RADIUS);
                        ((NatureSpike)Main.projectile[proj].modProjectile).RotationCenter = target;
                        ((NatureSpike)Main.projectile[proj].modProjectile).RandomOffset = RandomOffset;
                    }
                }
            }

            if (projectile.ai[0] == 8)
            {
                // Check if all the projectiles are in position
                ReadyCount = 0;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.type == ModContent.ProjectileType<NatureSpike>() && proj.owner == player.whoAmI)
                    {
                        if (((NatureSpike)proj.modProjectile).Ready)
                        {
                            ReadyCount++;
                        }
                    }
                }

                // All projectiles are ready and can fire
                if (ReadyCount == 8)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];
                        if (proj.active && proj.type == ModContent.ProjectileType<NatureSpike>() && proj.owner == player.whoAmI)
                        {
                            ((NatureSpike)proj.modProjectile).Converge = true;

                        }
                    }

                    projectile.Kill();
                }
            }
        }
    }
}