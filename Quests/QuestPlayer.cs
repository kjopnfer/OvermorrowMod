using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Quests
{
    public class QuestPlayer : ModPlayer
    {
        public override void ResetEffects()
        {
            // This resets the Quest's condition if the player no longer satisfies it so they can't turn it in once it is no longer satisfied
            foreach (Quest ActiveQuest in OvermorrowModFile.ActiveQuests)
            {
                ActiveQuest.IsCompleted = false;
            }
        }

        // Loops through the Quests and checks if their condition is true
        public override void PostUpdateMiscEffects()
        {
            foreach (Quest ActiveQuest in OvermorrowModFile.ActiveQuests)
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
            }
        }
    }
}