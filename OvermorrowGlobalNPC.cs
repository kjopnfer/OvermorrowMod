using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod
{
    public class OvermorrowGlobalNPC : GlobalNPC
    {
        
        public override void NPCLoot(NPC npc)
        {
            if (npc.type == NPCID.Harpy)
            {
                int dropChance = Main.rand.Next(10);
                if(dropChance == 0) // 10% drop chance
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("HarpyLeg"));
                }
            }
            if(Main.netMode == NetmodeID.Server)
            {
                if (npc.type != NPCID.WaterSphere && npc.type != NPCID.ChaosBall && npc.type != NPCID.BurningSphere && npc.type != NPCID.SolarFlare && npc.type != NPCID.VileSpit)
                {
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        if (npc.playerInteraction[i])
                        {
                            Player player = Main.player[i];
                            var modPlayer = WardenDamagePlayer.ModPlayer(player);
                            if (modPlayer.ReaperBook)
                            {
                                if (!XPPacket.Write(1, npc.target))
                                {
                                    player.GetModPlayer<WardenDamagePlayer>().AddSoul(1);
                                }
                            }
                        }
                    }
                }
            }else if(Main.netMode == NetmodeID.SinglePlayer)
            {
                if (npc.type != NPCID.WaterSphere && npc.type != NPCID.ChaosBall && npc.type != NPCID.BurningSphere && npc.type != NPCID.SolarFlare && npc.type != NPCID.VileSpit)
                {
                    Player player = Main.LocalPlayer;
                    var modPlayer = WardenDamagePlayer.ModPlayer(player);
                    if (modPlayer.soulResourceCurrent < modPlayer.soulResourceMax2 && modPlayer.ReaperBook)
                    {
                        if (Main.rand.Next(4) == 0) // 25% chance to gain Soul Essence on death
                        {
                            modPlayer.soulResourceCurrent += 1;
                            modPlayer.soulList.Add(Projectile.NewProjectile(npc.position, new Vector2(0, 0), mod.ProjectileType("SoulEssence"), 0, 0f, player.whoAmI, Main.rand.Next(70, 95), 0f));
                            CombatText.NewText(new Rectangle((int)npc.position.X, (int)npc.position.Y + 50, npc.width, npc.height), Color.DarkCyan, "Soul Essence Gained", true, false);
                        }
                    }
                }
            }
        }

        /*public override void HitEffect(NPC npc, int hitDirection, double damage)
        {
            // There might be server shenanigans we'll see
            if (npc.life <= 0)
            {
                if (npc.type != NPCID.WaterSphere && npc.type != NPCID.ChaosBall && npc.type != NPCID.BurningSphere && npc.type != NPCID.SolarFlare && npc.type != NPCID.VileSpit)
                {
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        if (npc.playerInteraction[i])
                        {
                            Player player = Main.player[i];
                            var modPlayer = WardenDamagePlayer.ModPlayer(player);
                            if (modPlayer.soulResourceCurrent < modPlayer.soulResourceMax2 && modPlayer.ReaperBook)
                            {
                                if (Main.rand.Next(4) == 0) // 25% chance to gain Soul Essence on death
                                {
                                    modPlayer.soulResourceCurrent += 1;
                                    modPlayer.soulList.Add(Projectile.NewProjectile(npc.position, new Vector2(0, 0), mod.ProjectileType("SoulEssence"), 0, 0f, player.whoAmI, Main.rand.Next(70, 95), 0f));
                                    CombatText.NewText(new Rectangle((int)npc.position.X, (int)npc.position.Y + 50, npc.width, npc.height), Color.DarkCyan, "Soul Essence Gained", true, false);
                                }
                            }
                        }
                    }
                }
            }
            /*if (Main.netMode == 2)
            {
                if (npc.life <= 0)
                {
                    if (npc.type != NPCID.WaterSphere && npc.type != NPCID.ChaosBall && npc.type != NPCID.BurningSphere && npc.type != NPCID.SolarFlare && npc.type != NPCID.VileSpit)
                    {
                        for (int i = 0; i < Main.maxPlayers; i++)
                        {
                            if (npc.playerInteraction[i])
                            {
                                Player player = Main.player[i];
                                var modPlayer = WardenDamagePlayer.ModPlayer(player);
                                if (modPlayer.soulResourceCurrent < modPlayer.soulResourceMax2)
                                {
                                    modPlayer.soulResourceCurrent += 1;
                                }
                                CombatText.NewText(new Rectangle((int)npc.position.X, (int)npc.position.Y + 50, npc.width, npc.height), Color.DarkCyan, "1 Soul Essence", true, false);
                            }
                        }
                    }
                }
            }
        }*/
    }
}