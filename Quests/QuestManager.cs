using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Quests
{
    public class QuestManager : ModWorld
    {
        public void AddNPCQuest(int ID, Quest QuestType)
        {
            if (QuestType.QuestGiver() == ID)
            {

            }
        }

        public override void PostUpdate()
        {
            // Add stuff here to have a chance to randomly assign Quests to NPCs
            /*for (int i = 0; i < Main.maxNPCs; i++)
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