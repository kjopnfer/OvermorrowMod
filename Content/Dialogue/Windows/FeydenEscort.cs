using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.NPCs.Town.Sojourn;
using OvermorrowMod.Core;
using OvermorrowMod.Quests;
using OvermorrowMod.Quests.ModQuests;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Dialogue
{
    public class FeydenEscort: DialogueWindow
    {
        public FeydenEscort() : base()
        {
            Dialogue = new[]
            {
                new DialogueNode(
                    "start",
                    new Text[]
                    {
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Feyden").Value,
                            "sojourn shouldnt be far from here", 60),
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Feyden").Value,
                            "just keep heading to the east and you should find it", 60),
                    }
                ),

                new DialogueNode(
                    "quest_hint",
                    new Text[]
                    {
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Feyden").Value,
                            "sojourn shouldnt be far from here", 60),
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Feyden").Value,
                            "just keep heading to the east and you should find it", 60),
                    }
                ),

                new DialogueNode(
                    "quest_complete",
                    new Text[]
                    {
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Feyden").Value,
                            "thanks for bringing me back cool person", 60),
                    },
                    new DialogueChoice[]
                    {
                        new DialogueChoice(ModContent.Request<Texture2D>(AssetDirectory.UI + "Dialogue_QuestIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, "Turn In", QuestReward)
                    }
                ),
            };
        }

        private void QuestReward(Player player, NPC npc)
        {
            QuestPlayer questPlayer = player.GetModPlayer<QuestPlayer>();
            var quest = npc.GetGlobalNPC<QuestNPC>().GetCurrentQuest(npc, out var isDoing);

            questPlayer.CompleteQuest(quest.QuestID);
        }
    }
}