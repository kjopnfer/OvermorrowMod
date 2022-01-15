using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static OvermorrowMod.Quests.Quest;

namespace OvermorrowMod.Quests
{
    public class QuestPlayer : ModPlayer
    {
        public Quest CurrentQuest;

        /// <summary>
        /// Adds a Quest to the player's current Quests
        /// </summary>
        /// <param name="type"></param>
        /// <param name="overrideCurrent"></param>
        public void SetQuest(int type, bool overrideCurrent = false)
        {
            if (overrideCurrent)
            {
                //CurrentQuest = QuestLoader.CreateQuest(type);
                CurrentQuest = QuestLoader.QuestList[type];
            }
            else if (CurrentQuest == null)
            {
                //CurrentQuest = QuestLoader.CreateQuest(type);
                CurrentQuest = QuestLoader.QuestList[type];
            }
        }

        /// <summary>
        /// Adds a Quest to the player's current Quests
        /// </summary>
        /// <param name="type"></param>
        /// <param name="overrideCurrent"></param>
        public void SetQuest(Quest quest, bool overrideCurrent = false)
        {
            if (overrideCurrent)
            {
                CurrentQuest = quest;
            }
            else if (CurrentQuest == null)
            {
                CurrentQuest = quest;
            }
        }

        public bool TryClaimCurrentQuest()
        {
            if (CurrentQuest == null || !CurrentQuest.CheckCompleted(player))
            {
                Main.NewText("Quest Hasnt Been Set");
            }
            else
            {
                CurrentQuest.GiveRewards(player);
                CurrentQuest.GiveExtraRewards(player);
                return true;
            }
            return false;
        }
        /*public override void PostUpdateMiscEffects()
        {
            foreach (Quest ActiveQuest in OvermorrowModFile.ActiveQuests)
            {
                foreach (Requirement QuestRequirement in ActiveQuest.QuestRequirements)
                {

                    foreach (Item item in player.inventory)
                    {
                        if (item.type != QuestRequirement.ItemID)
                        {
                            continue;
                        }

                        if (item.stack >= QuestRequirement.Stack)
                        {
                            Main.NewText("eh::" + QuestRequirement.Stack);
                            //QuestRequirement.Satisfied = true;
                            //QuestRequirement.SetSatisfied(true);
                        }
                        else
                        {
                            Main.NewText("else : " + QuestRequirement.Stack);
                            QuestRequirement.SetSatisfied(false);
                        }
                    }
                }

                // Assume the player fulfills all requirements, otherwise, loop through the bools.
                // If one of them is false, set the completion status to false
                ActiveQuest.IsCompleted = true;
                foreach (Requirement QuestRequirement in ActiveQuest.QuestRequirements)
                {
                    if (!QuestRequirement.Satisfied)
                    {
                        Main.NewText("falsoo");
                        ActiveQuest.IsCompleted = false;
                    }
                }
            }
        }*/
    }
}