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
    public class FeydenFree : DialogueWindow
    {
        public FeydenFree() : base()
        {
            Dialogue = new[]
            {
                new DialogueNode(
                    "start",
                    new Text[]
                    {
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Feyden").Value,
                            "Alright, you're not as useless as I thought. Thanks for the help. Just... don't expect me to write you a hero's ballad or anything.", 150),
                    },
                    new DialogueChoice[]
                    {
                        new DialogueChoice("Can't let a damsel in distress handle slimes alone, right?", null, "feyden_1")
                    }
                ),

                new DialogueNode(
                    "feyden_1",
                    new Text[]
                    {
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Feyden").Value,
                            "Damsel? Please. I've dealt with worse. But, I'll give you credit for showing up when I needed a hand.", 100),
                    },
                    new DialogueChoice[]
                    {
                        new DialogueChoice("Foraging in a slime den? You've got some guts.", null, "feyden_2")
                    }
                ),

                new DialogueNode(
                    "feyden_2",
                    new Text[]
                    {
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Feyden").Value,
                            "Yeah I know, Enza would want me to stick to the safer parts of the forest, but safe is for the meek.", 100),
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Feyden").Value,
                            "We need some real grub, not the usual bland stuff. Plus, mushrooms from here are rare and extra tasty.", 100),
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Feyden").Value,
                            "Our village is in a sorry state, you know. Bandits, wild creatures, you name it. And the tavern's running low on supplies. Figured I'd brave the cave for something to spice up our meals.", 180),
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Feyden").Value,
                            "Anyways, I should head back before someone notices I'm gone.", 60),
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Feyden").Value,
                            "You might as well tag along, at the very least I owe you a drink for your help.", 80),
                    },
                    new DialogueChoice[]
                    {
                        new DialogueChoice(ModContent.Request<Texture2D>(AssetDirectory.UI + "Dialogue_QuestIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, "That works for me.", EscortQuest)
                    }
                ),

                new DialogueNode(
                    "quest_complete",
                    new Text[]
                    {
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Feyden").Value,
                            "Well, here we are. Don't mind Oakley, you really saved my skin back there. I owe you one.", 90),
                    },
                    new DialogueChoice[]
                    {
                        new DialogueChoice("No problem.", null, "quest_complete_1")
                    }
                ),

                new DialogueNode(
                    "quest_complete_1",
                    new Text[]
                    {
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Feyden").Value,
                            "Well, not everyone's willing to jump into the fray like you did. Most folks around here are on edge with the bandit trouble and all.", 120),
                    },
                    new DialogueChoice[]
                    {
                        new DialogueChoice("Bandits? Maybe I can lend a hand.", null, "quest_complete_2")
                    }
                ),

                new DialogueNode(
                    "quest_complete_2",
                    new Text[]
                    {
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Feyden").Value,
                            "You're turning into quite the village savior. Just watch out for Enza. The one with antlers. If she catches wind of our little escapade, she'll give me an earful.", 150),
                    },
                    new DialogueChoice[]
                    {
                        new DialogueChoice(ModContent.Request<Texture2D>(AssetDirectory.UI + "Dialogue_QuestIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, "Got it. Let's keep this between us then.", QuestReward)
                    }
                ),
            };

        }

        private void EscortQuest(Player player, NPC npc)
        {
            QuestPlayer questPlayer = player.GetModPlayer<QuestPlayer>();
            QuestNPC questNPC = npc.GetGlobalNPC<QuestNPC>();

            var quest = npc.GetGlobalNPC<QuestNPC>().GetCurrentQuest(npc, out var isDoing);

            questPlayer.SetTravelLocation(quest, "sojourn_travel");
            questPlayer.CompleteQuest(questPlayer.GetQuestID<FeydenRescue>());

            var feyden = npc.ModNPC as Feyden;
            feyden.followPlayer = questPlayer.Player;
        }

        private void QuestReward(Player player, NPC npc)
        {
            var feyden = npc.ModNPC as Feyden;
            feyden.followPlayer = null;

            QuestPlayer questPlayer = player.GetModPlayer<QuestPlayer>();
            var quest = npc.GetGlobalNPC<QuestNPC>().GetCurrentQuest(npc, out var isDoing);

            questPlayer.CompleteQuest(quest.QuestID);
        }
    }
}