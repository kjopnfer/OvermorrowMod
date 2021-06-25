using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.IO;
using Terraria.Enums;
using Microsoft.Xna.Framework;

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
                        int projam1 = MethodHelper.GetProjAmount(ModContent.ProjectileType<ArrowRuneCircle>());
                        if (projam1 == 0)
                        {
                            Projectile.NewProjectile(player.Center.X, player.Center.Y - 100f, 0f, 0f, ModContent.ProjectileType<ArrowRuneCircle>(), 10, 0f);
                        }
                        npc.TargetClosest();
                        if(++npc.ai[1] == 360)
                        {
                            npc.ai[1] = 0;
                            goto case 2;
                        }
                    }
                    break;
                case 1:
                    {
                        int projam2 = MethodHelper.GetProjAmount(ModContent.ProjectileType<ArrowRunner>());
                        if(projam2 == 0)
                        {
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<ArrowRunner>(), 10, 1f);
                        }
                        npc.TargetClosest();
                        if (++npc.ai[1] == 360)
                        {
                            npc.ai[1] = 0;
                            goto case 2;
                        }
                    }
                    break;
                case 2:
                    {
                        if(player.dead || !player.active) { break; }
                        npc.TargetClosest();
                        npc.Teleport(MethodHelper.GetRandomVector((int)player.Center.X + 150, (int)player.Center.Y + 150, (int)player.Center.X - 150, (int)player.Center.Y - 150)); //  new Vector2((float)Main.rand.Next((int)player.Center.X - 150, (int)player.Center.Y + 150), (float)Main.rand.Next((int)player.Center.X + 150, (int)player.Center.Y - 150))
                        npc.ai[0] = Main.rand.Next(0, 2);
                    }
                    break;
            }
        }
    }
}
/*
 
                case 2:
                    {
                        Main.NewText("entered case 2");
                        int projam3 = MethodHelper.GetProjAmount(ModContent.ProjectileType<LightSpear>());
                        Vector2 pos = new Vector2(npc.Center.X - 150f, npc.Center.Y);
                        if(projam3 == 0)
                        {
                            Projectile.NewProjectile(pos, new Vector2(15f, 15f), ModContent.ProjectileType<LightSpear>(), 50, 0f);
                        }
                        if (++npc.ai[1] == 240)
                        {
                            npc.ai[1] = 0;
                            goto case 3;
                        }
                    }
                    break;
*/