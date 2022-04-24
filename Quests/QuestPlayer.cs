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
        public string SelectedLocation = null;

        private readonly List<BaseQuest> activeQuests = new List<BaseQuest>();
        public HashSet<string> CompletedQuests { get; } = new HashSet<string>();

        public IEnumerable<BaseQuest> CurrentQuests => activeQuests.Concat(
            Quests.PerPlayerActiveQuests.GetValueOrDefault(PlayerUUID) ?? Enumerable.Empty<BaseQuest>());

        public HashSet<string> LocalCompletedQuests { get; } = new HashSet<string>();

        public Dictionary<int, int> KilledNPCs = new Dictionary<int, int>();

        public bool IsDoingQuest(string questId)
        {
            return CurrentQuests.Any(q => q.QuestID == questId);
        }

        public void AddQuest(BaseQuest quest)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && Main.LocalPlayer == Player)
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
            if (Main.netMode == NetmodeID.MultiplayerClient && Main.LocalPlayer == Player)
                NetworkMessageHandler.Quests.CompleteQuest(-1, -1, questId);

            quest.CompleteQuest(Player, true);
            RemoveQuest(quest);
        }
        public override void SaveData(TagCompound tag)
        {
            tag["CompletedQuests"] = CompletedQuests.ToList();
            tag["CurrentQuests"] = activeQuests.Select(q => q.QuestID).ToList();
            tag["PlayerUUID"] = PlayerUUID;
            tag["KilledIDs"] = KilledNPCs.Keys.ToList();
            tag["KilledCounts"] = KilledNPCs.Values.ToList();
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
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
            var modPlayer = Player.GetModPlayer<QuestPlayer>();
            foreach (var quest in modPlayer.CurrentQuests)
            {
                if (quest.Type != QuestType.Travel) continue;

                foreach (IQuestRequirement requirement in quest.Requirements)
                {
                    // Check if the travel requirement isn't completed, if it isn't then:
                    if (requirement is TravelRequirement travelRequirement && !QuestSystem.PlayerTraveled.Contains(travelRequirement.ID))
                    {
                        if (MarkerCounter++ % 30 == 0)
                        {
                            Particle.CreateParticle(Particle.ParticleType<Pulse2>(), travelRequirement.location, Vector2.Zero, Color.Yellow, 1, 0.3f, 0, 0, 480);
                        }

                        if (Player.active && Player.Distance(travelRequirement.location) < 50)
                        {
                            QuestSystem.PlayerTraveled.Add(travelRequirement.ID);
                        }      
                    }
                }
            }

            base.PreUpdate();
        }
    }
}
