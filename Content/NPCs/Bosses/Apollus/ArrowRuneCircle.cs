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
            projectile.width = 62;
            projectile.height = 62;
            projectile.tileCollide = false;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.timeLeft = 600;
            projectile.penetrate = -1;
            projectile.scale = 1f;
        }
        public override void AI()
        {
            if (projectile.ai[0] == -10)
            {
                projectile.ai[0] = 0;
                whoAmiStore = (int)projectile.ai[1];
                projectile.ai[1] = 0;
            }
            if (projectile.ai[0] == -20)
            {
                projectile.ai[0] = 2;
                whoAmiStore = (int)projectile.ai[1];
                projectile.ai[1] = 0;
            }
            NPC owner = Main.npc[whoAmiStore];
            if (!owner.active)
            {
                projectile.Kill();
                return;
            }
            if (projectile.damage == 15)
            {
                projectile.ai[0] = 2;
            }
            switch (projectile.ai[0])
            {
                case 0: // spawn animation
                    {
                        if (projectile.ai[1] == 0)
                        {
                            projectile.scale = 0.01f;
                        }
                        if (projectile.ai[1] > 2 && projectile.ai[1] < 45)
                        {
                            projectile.scale = MathHelper.Lerp(projectile.scale, 1, 0.05f);
                            rotationCounter = MathHelper.Lerp(0.001f, 5f, 0.05f);
                            projectile.rotation += rotationCounter;
                        }
                        if (projectile.ai[1] == 45)
                        {
                            projectile.ai[0] = 1;
                        }
                    }
                    break;
                case 1: // shoot upwards
                    {
                        projectile.rotation += rotationCounter;
                        if (OvermorrowWorld.downedKnight)
                        {
                            if (projectile.ai[1] % 25 == 0)
                            {
                                int shootSpeed = Main.rand.Next(6, 8);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    float distance = 2000f;
                                    for (int k = 0; k < Main.maxPlayers; k++)
                                    {
                                        if (Main.player[k].active && !Main.player[k].dead)
                                        {
                                            Vector2 newMove = Main.player[k].Center - projectile.Center;
                                            float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                                            if (distanceTo < distance)
                                            {
                                                if (Main.player[k].active && !Main.player[k].dead)
                                                {
                                                    Projectile.NewProjectile(projectile.Center, projectile.DirectionTo(Main.player[k].Center) * shootSpeed, ModContent.ProjectileType<ApollusArrowNormal>(), 13, 3f, Main.myPlayer, 0, 0);
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
                            if (projectile.ai[1] % 45 == 0)
                            {
                                for (int i = 0; i < Main.rand.Next(2, 5); i++)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, Main.rand.Next(-3, 3), Main.rand.Next(-5, -3), ProjectileType<ApollusGravityArrow>(), projectile.damage, 10f, Main.myPlayer);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case 2: // spawn animation
                    {
                        NPC projectileOwner = Main.npc[whoAmiStore];
                        projectile.position = projectileOwner.Center + new Vector2(-35 + (566 * directionalStore), -35);
                        if (projectile.ai[1] == 0)
                        {
                            projectile.scale = 0.01f;
                            directionalStore = (int)projectile.knockBack;
                            projectile.knockBack = 10;
                            projectile.damage = 12;
                        }
                        if (projectile.ai[1] > 2 && projectile.ai[1] < 45)
                        {
                            projectile.scale = MathHelper.Lerp(projectile.scale, 1, 0.05f);
                            rotationCounter = MathHelper.Lerp(0.001f, 5f, 0.05f);
                            projectile.rotation += rotationCounter;
                        }
                        if (projectile.ai[1] == 45)
                        {
                            projectile.ai[0] = 3;
                        }
                    }
                    break;
                case 3: // shoot to the sides
                    {
                        Player projectileOwner = Main.player[projectile.owner];
                        projectile.position = projectileOwner.Center + new Vector2(-35 + (566 * directionalStore), -35);
                        projectile.rotation += rotationCounter;
                        if (projectile.ai[1] % 45 == 0)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(projectile.Center, new Vector2(Main.rand.Next(-5, -3) * directionalStore, Main.rand.Next(-3, 3) * directionalStore), ProjectileType<ApollusArrowNormal>(), projectile.damage, 2, Main.myPlayer);
                                }
                            }
                        }
                    }
                    break;
            }
            projectile.ai[1]++;
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
