using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Content.NPCs.Bosses.Apollus
{
    public class ArrowRuneCircle : ModProjectile
    {
        float rotationCounter;
        int directionalStore;
        int whoAmiStore;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rune Circle");
        }
        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 62;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == -10)
            {
                Projectile.ai[0] = 0;
                whoAmiStore = (int)Projectile.ai[1];
                Projectile.ai[1] = 0;
            }
            if (Projectile.ai[0] == -20)
            {
                Projectile.ai[0] = 2;
                whoAmiStore = (int)Projectile.ai[1];
                Projectile.ai[1] = 0;
            }
            NPC owner = Main.npc[whoAmiStore];
            if (!owner.active)
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.damage == 15)
            {
                Projectile.ai[0] = 2;
            }
            switch (Projectile.ai[0])
            {
                case 0: // spawn animation
                    {
                        if (Projectile.ai[1] == 0)
                        {
                            Projectile.scale = 0.01f;
                        }
                        if (Projectile.ai[1] > 2 && Projectile.ai[1] < 45)
                        {
                            Projectile.scale = MathHelper.Lerp(Projectile.scale, 1, 0.05f);
                            rotationCounter = MathHelper.Lerp(0.001f, 5f, 0.05f);
                            Projectile.rotation += rotationCounter;
                        }
                        if (Projectile.ai[1] == 45)
                        {
                            Projectile.ai[0] = 1;
                        }
                    }
                    break;
                case 1: // shoot upwards
                    {
                        Projectile.rotation += rotationCounter;
                        if (OvermorrowWorld.downedKnight)
                        {
                            if (Projectile.ai[1] % 25 == 0)
                            {
                                int shootSpeed = Main.rand.Next(6, 8);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    float distance = 2000f;
                                    for (int k = 0; k < Main.maxPlayers; k++)
                                    {
                                        if (Main.player[k].active && !Main.player[k].dead)
                                        {
                                            Vector2 newMove = Main.player[k].Center - Projectile.Center;
                                            float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                                            if (distanceTo < distance)
                                            {
                                                if (Main.player[k].active && !Main.player[k].dead)
                                                {
                                                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Projectile.DirectionTo(Main.player[k].Center) * shootSpeed, ModContent.ProjectileType<ApollusArrowNormal>(), 13, 3f, Main.myPlayer, 0, 0);
                                                }
                                                distance = distanceTo;
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Projectile.ai[1] % 45 == 0)
                            {
                                for (int i = 0; i < Main.rand.Next(2, 5); i++)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-3, 3), Main.rand.Next(-5, -3), ProjectileType<ApollusGravityArrow>(), Projectile.damage, 10f, Main.myPlayer);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case 2: // spawn animation
                    {
                        NPC ProjectileOwner = Main.npc[whoAmiStore];
                        Projectile.position = ProjectileOwner.Center + new Vector2(-35 + (566 * directionalStore), -35);
                        if (Projectile.ai[1] == 0)
                        {
                            Projectile.scale = 0.01f;
                            directionalStore = (int)Projectile.knockBack;
                            Projectile.knockBack = 10;
                            Projectile.damage = 12;
                        }
                        if (Projectile.ai[1] > 2 && Projectile.ai[1] < 45)
                        {
                            Projectile.scale = MathHelper.Lerp(Projectile.scale, 1, 0.05f);
                            rotationCounter = MathHelper.Lerp(0.001f, 5f, 0.05f);
                            Projectile.rotation += rotationCounter;
                        }
                        if (Projectile.ai[1] == 45)
                        {
                            Projectile.ai[0] = 3;
                        }
                    }
                    break;
                case 3: // shoot to the sides
                    {
                        Player ProjectileOwner = Main.player[Projectile.owner];
                        Projectile.position = ProjectileOwner.Center + new Vector2(-35 + (566 * directionalStore), -35);
                        Projectile.rotation += rotationCounter;
                        if (Projectile.ai[1] % 45 == 0)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, new Vector2(Main.rand.Next(-5, -3) * directionalStore, Main.rand.Next(-3, 3) * directionalStore), ProjectileType<ApollusArrowNormal>(), Projectile.damage, 2, Main.myPlayer);
                                }
                            }
                        }
                    }
                    break;
            }
            Projectile.ai[1]++;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
    }
}
