using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Netcode;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.NPCs;
using OvermorrowMod.Quests.Requirements;
using OvermorrowMod.Quests.State;
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
        public string PlayerUUID { get; private set; } = Guid.NewGuid().ToString();

        public string SelectedLocation { get; set; } = null;

        public IEnumerable<BaseQuestState> CurrentQuests => Quests.State.GetActiveQuests(this);

        public bool IsDoingQuest(string questId)
        {
            return Quests.State.IsDoingQuest(this, questId);
        }

        public void AddQuest(BaseQuest quest)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && Main.LocalPlayer == Player)
                NetworkMessageHandler.Quests.TakeQuest(-1, -1, quest.QuestID);

            Quests.State.AddQuest(this, quest);
        }

        public BaseQuestState QuestByNPC(int npcId)
        {
            return CurrentQuests.FirstOrDefault(q => npcId == q.Quest.QuestGiver);
        }

        public void CompleteQuest(string questId)
        {
            var quest = CurrentQuests.FirstOrDefault(q => q.Quest.QuestID == questId);
            // Should not happen!
            if (quest == null) throw new ArgumentException($"Player is not doing {questId}");
            // Send message to server if the quest is being completed for the current player
            if (Main.netMode == NetmodeID.MultiplayerClient && Main.LocalPlayer == Player)
                NetworkMessageHandler.Quests.CompleteQuest(-1, -1, questId);

            quest.Quest.CompleteQuest(Player, true);
        }

        public void TickQuestRequirements(string questId)
        {
            var quest = CurrentQuests.FirstOrDefault(q => q.Quest.QuestID == questId);
            foreach (var req in quest.Quest.Requirements)
            {
                req.TryCompleteRequirement(this, quest);
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag["questStates"] = Quests.State.GetPerPlayerQuests(this).ToList();
            tag["PlayerUUID"] = PlayerUUID;
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            PlayerUUID = tag.GetString("PlayerUUID");
            if (PlayerUUID == null) PlayerUUID = Guid.NewGuid().ToString();

            var questStates = tag.GetList<TagCompound>("questStates");
            Quests.State.LoadPlayer(this, questStates);
        }

        private int markerCounter = 0;
        public override void PreUpdate()
        {
            foreach (var (_, req) in Quests.State.GetActiveRequirementsOfType<TravelRequirementState>(this))
            {
                if (!req.IsCompleted)
                {
                    if (markerCounter % 30 == 0)
                    {
                        Particle.CreateParticle(Particle.ParticleType<Pulse>(), (req.Requirement as TravelRequirement).Location * 16f,
                            Vector2.Zero, Color.Yellow, 1, 0.3f, 0, 0, 480);
                    }

                    if (Player.active && Player.Distance((req.Requirement as TravelRequirement).Location * 16) < 50)
                    {
                        req.IsCompleted = true;
                    }
                }
            }
            markerCounter++;

            base.PreUpdate();
        }
    }
}
