using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.IO;
using Terraria.Enums;
using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.NPCs.Bosses.Apollus
{
    public class ApollusBoss : ModNPC
    {
        public override string Texture => "OvermorrowMod/NPCs/Bosses/Apollus/Apollus";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Apollus");
            Main.npcFrameCount[npc.type] = 5;
        }
        public override void SetDefaults()
        {
            npc.width = 123;
            npc.height = 123;
            npc.defense = 50;
            npc.lifeMax = 2800;
            npc.knockBackResist = 0f;
            npc.value = (float)Item.buyPrice(gold: 50);
            npc.boss = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
        }
        public int maxRuneCircle = 3;
        public int timer = 0;
        public override void AI()
        {
            Player player = Main.player[npc.target];
            bool expertMode = Main.expertMode;
            switch (npc.ai[0])
            {
                case 0:
                    {
                        int projam1 = MethodHelper.GetProjAmount(ProjectileType<ArrowRuneCircle>());
                        if (projam1 == 0)
                        {
                            Projectile.NewProjectile(player.Center.X, player.Center.Y - 100f, 0f, 0f, ProjectileType<ArrowRuneCircle>(), 10, 0f);
                        }
                        npc.TargetClosest();
                        if(++npc.ai[1] == 360)
                        {
                            npc.ai[1] = 0;
                            npc.ai[2] = 1;
                            goto case 3;
                        }
                    }
                    break;
                case 1:
                    {
                        if (++npc.ai[1] % 30 == 0)
                        {
                            int projectiles = 3;
                            int random = Main.rand.Next(5);
                            for (int j = 0; j < projectiles; j++)
                            {
                                Projectile.NewProjectile(npc.Center, new Vector2(0f, 8.5f).RotatedBy((j * MathHelper.TwoPi / projectiles) + (npc.ai[2] * 30) + random), ProjectileType<ApollusArrow>(), 2, 10f, Main.myPlayer);
                            }
                            npc.ai[2] += 1;
                        }

                        npc.TargetClosest();
                        if (npc.ai[1] == 420)
                        {
                            npc.ai[1] = 0;
                            npc.ai[2] = 1;
                            goto case 3;
                        }

                    }
                    break;
                case 2:
                    {
                        if (++npc.ai[1] % 45 == 0)
                        {
                            int projectiles = 6;
                            for (int j = 0; j < projectiles; j++)
                            {
                                Projectile.NewProjectile(npc.Center, new Vector2(0f, 5f).RotatedBy((j * MathHelper.TwoPi / projectiles) + (npc.ai[2] * 15)), ProjectileType<HomingArrow>(), 2, 10f, Main.myPlayer);
                            }
                            npc.ai[2] += 1;
                        }
                        npc.TargetClosest();

                        if (npc.ai[1] == 180)
                        {
                            npc.ai[1] = 0;
                            npc.ai[2] = 1;
                            goto case 3;
                        }
                    }
                    break;
                case 3:
                    {
                        if(player.dead || !player.active) { break; }
                        npc.TargetClosest();
                        npc.Teleport(MethodHelper.GetRandomVector((int)player.Center.X + 150, (int)player.Center.Y + 150, (int)player.Center.X - 150, (int)player.Center.Y - 150)); //  new Vector2((float)Main.rand.Next((int)player.Center.X - 150, (int)player.Center.Y + 150), (float)Main.rand.Next((int)player.Center.X + 150, (int)player.Center.Y - 150))
                        npc.ai[0] = Main.rand.Next(0, 3);
                    }
                    break;
            }
        }
    }
}