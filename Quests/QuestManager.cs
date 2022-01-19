using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Quests
{
    public class QuestManager : ModWorld
    {
        public override void PreUpdate()
        {
            /*if (Main.rand.NextBool(50))
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && npc.type == NPCID.Guide && npc.GetGlobalNPC<QuestNPC>().CurrentQuest == null)
                    {
                        foreach (Quest quest in OvermorrowModFile.QuestList)
                        {
                            if (quest.QuestGiver() == npc.type && !OvermorrowModFile.CompletedQuests.Contains(quest))
                            {
                                if (quest.QuestID() == (int)Quest.ID.GuideHouseQuest)
                                {
                                    npc.GetGlobalNPC<QuestNPC>().CurrentQuest = quest;
                                    Main.NewText("new guide quest");
                                }
                            }
                        }
                    }
                }
            }*/
        }

        public override void Initialize()
        {
            // Add stuff here to have a chance to randomly assign Quests to NPCs
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.type == NPCID.Guide && npc.GetGlobalNPC<QuestNPC>().CurrentQuest == null)
                {
                    foreach (Quest Quest in OvermorrowModFile.QuestList)
                    {
                        if (Quest.QuestGiver() == npc.type && !OvermorrowModFile.CompletedQuests.Contains(Quest))
                        {
                            npc.GetGlobalNPC<QuestNPC>().CurrentQuest = Quest;
                        }
                    }
                }
            }
        }
    }
}