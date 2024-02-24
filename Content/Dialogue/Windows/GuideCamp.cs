using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Core;
using OvermorrowMod.Quests;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Dialogue
{
    public class GuideCamp : DialogueWindow
    {
        public GuideCamp() : base()
        {
            Dialogue = new[]
            {
                new DialogueNode(
                    "start",
                    new Text[]
                    {
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "GuideWave", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                            "Came across you unconscious so I took care of you for a bit, no big deal.", 60),
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Guide", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                            "RIP, looks like my fire got put out, wanna help me start it again?", 60)
                    },
                    new DialogueChoice[]
                    {
                        new DialogueChoice(ModContent.Request<Texture2D>(AssetDirectory.UI + "Dialogue_QuestIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, "Fine", StartQuest)
                    }
                ),

                new DialogueNode(
                    "quest_hint",
                    new Text[]
                    {
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Guide", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                            "Have you gotten those torches yet?", 30),
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Guide", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                            "The campfire is right here when you're ready.", 40)
                    }
                ),

                new DialogueNode(
                    "quest_complete",
                    new Text[]
                    {
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Guide", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                            "You catch on pretty fast. I think that sums up everything I had to tell you... What are you planning to do next?", 180),
                    },
                    new DialogueChoice[]
                    {
                        new DialogueChoice(ModContent.Request<Texture2D>(AssetDirectory.UI + "Dialogue_ChestIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, "Take on a powerful monster", null, "quest_complete_monster"),
                        new DialogueChoice(ModContent.Request<Texture2D>(AssetDirectory.UI + "Dialogue_ChestIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, "Discover an uncharted biome", null, "quest_complete_biome"),
                        new DialogueChoice(ModContent.Request<Texture2D>(AssetDirectory.UI + "Dialogue_ChestIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, "Build a cozy place to live", null, "quest_complete_build"),
                    }
                ),

                new DialogueNode(
                    "quest_complete_monster",
                    new Text[]
                    {
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Guide", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                            "A monster eh? I have heard of some {giant floating eye} terrorizing travelers at night...", 180),
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Guide", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                            "Maybe look for a [caravan merchant] or someone who can confirm the rumors.", 60),
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Guide", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                            "Regardless, you'll have to get some better gear before you try anything like that. I don't get out much these days so why don't you borrow these for now. The rest you'll have to find on your own.", 240)
                    },
                    new DialogueChoice[]
                    {
                        new DialogueChoice(ModContent.Request<Texture2D>(AssetDirectory.UI + "Dialogue_QuestIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, "Turn In", QuestRewardMonster)
                    }
                ),

                new DialogueNode(
                    "quest_complete_biome",
                    new Text[]
                    {
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Guide", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                            "Quite the explorer you are! Well if uncharted biomes are what you're looking for you couldn't have come to a better place.", 180),
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Guide", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                            "There's a place they call the {Crimson} that no adventurer dares enter.", 90),
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Guide", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                            "Regardless, you'll have to get some better gear before you try anything like that. I don't get out much these days so why don't you borrow these for now. The rest you'll have to find on your own.", 240)
                    },
                    new DialogueChoice[]
                    {
                        new DialogueChoice(ModContent.Request<Texture2D>(AssetDirectory.UI + "Dialogue_QuestIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, "Turn In", QuestRewardBiome)
                    }
                ),

                new DialogueNode(
                    "quest_complete_build",
                    new Text[]
                    {
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Guide", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                            "Ah, settling down are we? You think you could find a place for these in that fancy new home of ours? Things start to get heavy after so many years.", 180),
                    },
                    new DialogueChoice[]
                    {
                        new DialogueChoice("Don't get excited, it won't be anything too fancy", null, "quest_complete_build_end"),
                        new DialogueChoice("I'll make them the best storage space you've ever seen", null, "quest_complete_build_end"),
                    }
                ),

                new DialogueNode(
                    "quest_complete_build_end",
                    new Text[]
                    {
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "GuideWave", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                            "I'm sure whatever you build it will be great. Stay safe out there!", 60),
                    },
                    new DialogueChoice[]
                    {
                        new DialogueChoice(ModContent.Request<Texture2D>(AssetDirectory.UI + "Dialogue_QuestIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, "Turn In", QuestRewardBuild)
                    }
                ),
            };
        }

        private void StartQuest(Player player, NPC npc)
        {
            QuestPlayer questPlayer = player.GetModPlayer<QuestPlayer>();
            QuestNPC questNPC = npc.GetGlobalNPC<QuestNPC>();

            var quest = npc.GetGlobalNPC<QuestNPC>().GetCurrentQuest(npc, out var isDoing);

            DialoguePlayer dialoguePlayer = player.GetModPlayer<DialoguePlayer>();
            dialoguePlayer.AddNPCPopup(NPCID.Guide, ModUtils.GetXML(AssetDirectory.Popups + "GuideCampAxe.xml"));

            questPlayer.AddQuest(quest);
            questNPC.TakeQuest();

            SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/QuestAccept")
            {
                Volume = 0.9f,
                PitchVariance = 0.2f,
                MaxInstances = 3,
            }, npc.Center);

            // Run the Quest Accepted UI
            Main.NewText("ACCEPTED QUEST: " + quest.QuestName, Microsoft.Xna.Framework.Color.Yellow);
        }

        private void QuestRewardBuild(Player player, NPC npc)
        {
            QuestPlayer questPlayer = player.GetModPlayer<QuestPlayer>();
            var quest = npc.GetGlobalNPC<QuestNPC>().GetCurrentQuest(npc, out var isDoing);

            questPlayer.CompleteQuest(quest.QuestID, "build_reward");
        }

        private void QuestRewardMonster(Player player, NPC npc)
        {
            QuestPlayer questPlayer = player.GetModPlayer<QuestPlayer>();
            var quest = npc.GetGlobalNPC<QuestNPC>().GetCurrentQuest(npc, out var isDoing);

            questPlayer.CompleteQuest(quest.QuestID, "monster_reward");
        }

        private void QuestRewardBiome(Player player, NPC npc)
        {
            QuestPlayer questPlayer = player.GetModPlayer<QuestPlayer>();
            var quest = npc.GetGlobalNPC<QuestNPC>().GetCurrentQuest(npc, out var isDoing);

            questPlayer.CompleteQuest(quest.QuestID, "biome_reward");
        }
    }
}