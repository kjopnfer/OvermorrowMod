using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Quests
{
    public class QuestPlayer : ModPlayer
    {
        public List<Quest> QuestList = new List<Quest>();

        // Loops through the Quests and checks if their condition is true
        public override void PostUpdateMiscEffects()
        {
            foreach (Quest ActiveQuest in QuestList)
            {
                ActiveQuest.IsCompleted = ActiveQuest.CheckCompleted(player);
            }
            /*foreach (Quest ActiveQuest in OvermorrowModFile.ActiveQuests)
            {
                switch (ActiveQuest.QuestID())
                {
                    case (int)Quest.ID.TutorialGuideQuest:
                        foreach (Item item in player.inventory)
                        {
                            if (item.type == ItemID.Wood && item.stack >= 10)
                            {
                                ActiveQuest.IsCompleted = true;
                            }
                        }
                        break;
                }
            }*/
        }
    }
}