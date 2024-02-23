using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using OvermorrowMod.Quests;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
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
                        new DialogueChoice("Fine", null, StartQuest)
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
                        new DialogueChoice("Take on a powerful monster", "quest_complete_monster"),
                        new DialogueChoice("Discover an uncharted biome", "quest_complete_biome"),
                        new DialogueChoice("Build a cozy place to live", "quest_complete_build"),
                    }
                ),


            };

        }

        private void StartQuest(Player player, NPC npc)
        {
            QuestPlayer questPlayer = player.GetModPlayer<QuestPlayer>();
            QuestNPC questNPC = npc.GetGlobalNPC<QuestNPC>();

            var quest = npc.GetGlobalNPC<QuestNPC>().GetCurrentQuest(npc, out var isDoing);

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
    }
}