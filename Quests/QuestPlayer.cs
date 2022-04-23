using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Netcode;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.NPCs;
using OvermorrowMod.Quests.Requirements;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Quests
{
    public class QuestPlayer : ModPlayer
    {
        public string PlayerUUID { get; private set; } = null;

        private readonly List<BaseQuest> activeQuests = new List<BaseQuest>();
        public HashSet<string> CompletedQuests { get; } = new HashSet<string>();

        public IEnumerable<BaseQuest> CurrentQuests => activeQuests.Concat(Quests.PerPlayerActiveQuests[PlayerUUID]);

        public HashSet<string> LocalCompletedQuests { get; } = new HashSet<string>();

        public Dictionary<int, int> KilledNPCs = new Dictionary<int, int>();

        public bool IsDoingQuest(string questId)
        {
            return CurrentQuests.Any(q => q.QuestID == questId);
        }

        public void AddQuest(BaseQuest quest)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && Main.LocalPlayer == player)
                NetworkMessageHandler.Quests.TakeQuest(-1, -1, quest.QuestID);
            if (quest.Repeatability == QuestRepeatability.OncePerWorldPerPlayer || quest.Repeatability == QuestRepeatability.OncePerWorld)
            {
                Quests.PerPlayerActiveQuests[PlayerUUID].Add(quest);
            }
            else
            {
                activeQuests.Add(quest);
            }
        }

        public void RemoveQuest(BaseQuest quest)
        {
            activeQuests.Remove(quest);
            Quests.PerPlayerActiveQuests[PlayerUUID].Remove(quest);
        }

        public BaseQuest QuestByNPC(int npcId)
        {
            return CurrentQuests.FirstOrDefault(q => npcId == q.QuestGiver);
        }

        public void CompleteQuest(string questId)
        {
            var quest = CurrentQuests.FirstOrDefault(q => q.QuestID == questId);
            // Should not happen!
            if (quest == null) throw new ArgumentException($"Player is not doing {questId}");
            // Send message to server if the quest is being completed for the current player
            if (Main.netMode == NetmodeID.MultiplayerClient && Main.LocalPlayer == player)
                NetworkMessageHandler.Quests.CompleteQuest(-1, -1, questId);

            quest.CompleteQuest(player, true);
            RemoveQuest(quest);
        }

        public override TagCompound Save()
        {
            return new TagCompound
            {
                ["CompletedQuests"] = CompletedQuests.ToList(),
                ["CurrentQuests"] = activeQuests.Select(q => q.QuestID).ToList(),
                ["PlayerUUID"] = PlayerUUID,
                ["KilledIDs"] = KilledNPCs.Keys.ToList(),
                ["KilledCounts"] = KilledNPCs.Values.ToList(),
            };
        }

        public override void Load(TagCompound tag)
        {
            CompletedQuests.Clear();
            activeQuests.Clear();

            var completedQuests = tag.GetList<string>("CompletedQuests");
            foreach (var quest in completedQuests)
            {
                if (!Quests.QuestList.TryGetValue(quest, out var qInst) || qInst.Repeatability != QuestRepeatability.OncePerPlayer)
                    continue;
                CompletedQuests.Add(quest);
            }

            var currentQuests = tag.GetList<string>("CurrentQuests");
            foreach (var questId in currentQuests)
            {
                if (Quests.QuestList.TryGetValue(questId, out var quest)
                    && (quest.Repeatability == QuestRepeatability.Repeatable || quest.Repeatability == QuestRepeatability.OncePerPlayer))
                {
                    activeQuests.Add(quest);
                }
            }

            PlayerUUID = tag.GetString("PlayerUUID");
            if (PlayerUUID == null) PlayerUUID = Guid.NewGuid().ToString();

            if (!Quests.PerPlayerCompletedQuests.ContainsKey(PlayerUUID))
            {
                Quests.PerPlayerCompletedQuests[PlayerUUID] = new HashSet<string>();
            }
            if (!Quests.PerPlayerActiveQuests.ContainsKey(PlayerUUID))
            {
                Quests.PerPlayerActiveQuests[PlayerUUID] = new List<BaseQuest>();
            }

            var IDs = tag.GetList<int>("KilledIDs");
            var counts = tag.GetList<int>("KilledCounts");
            KilledNPCs = IDs.Zip(counts, (k, v) => new { Key = k, Value = v }).ToDictionary(x => x.Key, x => x.Value);
        }

        int MarkerCounter = 0;
        public override void PreUpdate()
        {
            var modPlayer = player.GetModPlayer<QuestPlayer>();
            foreach (var quest in modPlayer.CurrentQuests)
            {
                if (quest.Type != QuestType.Travel) continue;

                foreach (IQuestRequirement requirement in quest.Requirements)
                {
                    // Check if the travel requirement isn't completed, if it isn't then:
                    if (requirement is TravelRequirement travelRequirement && !QuestWorld.PlayerTraveled.Contains(travelRequirement.ID))
                    {
                        if (MarkerCounter++ % 30 == 0)
                        {
                            Particle.CreateParticle(Particle.ParticleType<Pulse2>(), travelRequirement.location, Vector2.Zero, Color.Yellow, 1, 0.3f, 0, 0, 480);
                        }

                        if (player.active && player.Distance(travelRequirement.location) < 50)
                        {
                            QuestWorld.PlayerTraveled.Add(travelRequirement.ID);
                        }      
                    }
                }
            }

            // So that we don't have to run the check every single tick for every single player,
            // We use this to place any markers that have been despawned either from leaving the world or somehow killing them
            //if (Main.dayTime && Main.time == 0)
            /*{
                foreach (Player player in Main.player)
                {
                    if (!player.active) continue;

                    var modPlayer = player.GetModPlayer<QuestPlayer>();
                    foreach (var quest in modPlayer.CurrentQuests)
                    {
                        if (quest.Type != QuestType.Travel) continue;

                        foreach (IQuestRequirement requirement in quest.Requirements)
                        {
                            bool exists = false;

                            // Check if the travel requirement isn't completed, if it isn't then:
                            if (requirement is TravelRequirement travelRequirement && !travelRequirement.completed)
                            {
                                // Loop through the NPC array, check if the marker for the Quest exists
                                foreach (NPC npc in Main.npc)
                                {
                                    if (npc.modNPC is QuestMarker marker && marker.LocationID == travelRequirement.ID)
                                    {
                                        exists = true;
                                    }
                                }
                                
                                // The marker doesn't exist, so spawn it in with the QuestName and the ID
                                if (!exists)
                                {
                                    Vector2 SpawnLocation = travelRequirement.location;
                                    int marker = NPC.NewNPC((int)SpawnLocation.X, (int)SpawnLocation.Y, ModContent.NPCType<QuestMarker>());
                                    ((QuestMarker)Main.npc[marker].modNPC).QuestName = quest.QuestName;
                                    ((QuestMarker)Main.npc[marker].modNPC).LocationID = travelRequirement.ID;
                                }
                            }
                        }
                    }
                }
            }*/

            base.PreUpdate();
        }
    }
}
