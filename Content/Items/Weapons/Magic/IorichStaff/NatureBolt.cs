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
        public override bool? CanDamage() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nature Bolt");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 420;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Main.LocalPlayer != player) return;

            if (target == null)
            {
                if (Main.myPlayer == Projectile.owner) Projectile.Center = Main.MouseWorld;
            }
            else
            {
                Projectile.Center = target.Center;
            }

            Lighting.AddLight(Projectile.Center, 0f, 1f, 0f);
            Projectile.localAI[0] += 4; // Rotation counter
            int DUST_RADIUS = 15;

            if (player.HeldItem.type == ModContent.ItemType<IorichStaff>())
            {
                Projectile.timeLeft = 5;
            }

            Dust dust = Terraria.Dust.NewDustPerfect(Projectile.Center, 107, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
            dust.noGravity = true;

            if (Main.mouseRight && target == null)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && !npc.friendly && npc.getRect().Intersects(Projectile.getRect()))
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
                    Vector2 dustPos = Projectile.Center + new Vector2(DUST_RADIUS, 0).RotatedBy(MathHelper.ToRadians(i * 10 + Projectile.localAI[0]));
                    Dust dust2 = Dust.NewDustPerfect(dustPos, 107, Vector2.Zero, 0, default, 1f);
                    dust2.noGravity = true;
                }

                for (int i = 0; i < 2; i++)
                {
                    Vector2 dustPos = Projectile.Center - new Vector2(DUST_RADIUS, 0).RotatedBy(MathHelper.ToRadians(i * 10 + Projectile.localAI[0]));
                    Dust dust2 = Dust.NewDustPerfect(dustPos, 107, Vector2.Zero, 0, default, 1f);
                    dust2.noGravity = true;
                }

                if (player.channel && Projectile.ai[0] < 8)
                {
                    if (Projectile.ai[1]++ == 0)
                    {
                        RotationDirection = Main.rand.NextBool(2) ? 1 : -1;
                        RotationOffset = Main.rand.Next(4) * MathHelper.PiOver2;

                        Projectile.netUpdate = true;
                        RandomOffset = Main.rand.Next(0, 10) * 40; // Random number between 0 and 360 with 40 degree increments
                    }

                    SpiritPoints[] values = (SpiritPoints[])Enum.GetValues(typeof(SpiritPoints));
                    if (Projectile.ai[1] % 20 == 0)
                    {
                        int RADIUS = target.width + 155;

                        float Rotation = RotationDirection * (int)values[(int)Projectile.ai[0]++] * MathHelper.PiOver4;
                        Vector2 SpawnLocation = new Vector2(RADIUS, 0).RotatedBy(Rotation + RotationOffset);

                        int proj = Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), target.Center + SpawnLocation, Vector2.Zero, ModContent.ProjectileType<NatureSpike>(), Projectile.damage, 0f, Projectile.owner, Rotation + RotationOffset, RADIUS);
                        ((NatureSpike)Main.projectile[proj].ModProjectile).RotationCenter = target;
                        ((NatureSpike)Main.projectile[proj].ModProjectile).RandomOffset = RandomOffset;
                    }
                }
            }

            if (Projectile.ai[0] == 8)
            {
                // Check if all the Projectiles are in position
                ReadyCount = 0;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.type == ModContent.ProjectileType<NatureSpike>() && proj.owner == player.whoAmI)
                    {
                        if (((NatureSpike)proj.ModProjectile).Ready)
                        {
                            ReadyCount++;
                        }
                    }
                }

                // All Projectiles are ready and can fire
                if (ReadyCount == 8)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];
                        if (proj.active && proj.type == ModContent.ProjectileType<NatureSpike>() && proj.owner == player.whoAmI)
                        {
                            ((NatureSpike)proj.ModProjectile).Converge = true;

                        }
                    }

                    Projectile.Kill();
                }
            }
        }
    }
}