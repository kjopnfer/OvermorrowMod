using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.WorldGeneration;
using OvermorrowMod.Core;
using OvermorrowMod.Quests.Requirements;
using OvermorrowMod.Quests.State;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Quests
{
    public partial class QuestPlayer : ModPlayer
    {
        public bool grabbedAxe = false;
        public bool showCampfireArrow = false;
        public bool reachedSlimeCave = false;
        DialoguePlayer dialoguePlayer => Player.GetModPlayer<DialoguePlayer>();

        // TODO: Make this not cringe by putting them in the Quest or Requirements or something
        private void RequirementCompleteAction(string id)
        {
            switch (id)
            {
                case "axe":
                    grabbedAxe = true;
                    break;
                case "wood":
                    dialoguePlayer.AddNPCPopup(NPCID.Guide, ModUtils.GetXML(AssetDirectory.Popup + "GuideCampGel.xml"));

                    float offset = 64 * (Main.rand.NextBool() ? 16 : -16); // Spawn a slime on the left or right side 64 tiles away
                    Vector2 position = dialoguePlayer.Player.Center + new Vector2(offset, -720);
                    Vector2 spawnPosition = ModUtils.FindNearestGround(position) * 16;

                    NPC.NewNPC(null, (int)spawnPosition.X, (int)spawnPosition.Y, NPCID.GreenSlime, 0);
                    break;
                case "gel":
                    dialoguePlayer.AddNPCPopup(NPCID.Guide, ModUtils.GetXML(AssetDirectory.Popup + "GuideCampTorch.xml"));

                    break;
                case "torches":
                    showCampfireArrow = true;
                    break;
            }
        }

        public void CompleteMiscRequirement(string id)
        {
            foreach (var (_, req) in Quests.State.GetActiveRequirementsOfType<MiscRequirementState>(this))
            {
                if (!req.IsCompleted && req.Requirement.ID == id)
                    req.IsCompleted = true;
            }
        }

        /// <summary>
        /// Returns an active Quest's ID given the Quest's display name. Returns null if not found.
        /// </summary>
        public string GetQuestIDByName(string name)
        {
            var quest = Quests.State.GetActiveQuests(this).Where(q => q.Quest.QuestName == name)?.ToList();
            if (quest == null || !quest.Any()) return null;

            return quest[0].Quest.QuestID;
        }

        /// <summary>
        /// Sets the Player's travel marker to one of the TravelRequirements given their ID
        /// </summary>
        public void SetTravelLocation(BaseQuest quest, string id)
        {
            foreach (var req in quest.GetAllRequirements())
            {
                if (req.ID == id && req is TravelRequirement) SelectedLocation = req.ID;
            }
        }

        private void GeneralUpdateActions()
        {
            #region Slime Cave
            if (Player.Center.X >= GuideCamp.SlimeCaveEntrance.X - (80 * 16) && !reachedSlimeCave)
            {
                reachedSlimeCave = true;
                dialoguePlayer.AddNPCPopup(NPCID.Guide, ModUtils.GetXML(AssetDirectory.Popup + "FeydenHelp.xml"));

                var possibleQuests = Quests.QuestList.Values
                .Where(q => q.QuestName == "A Call for Help")
                .GroupBy(q => q.Priority)
                .Max()
                ?.ToList();

                if (possibleQuests == null || !possibleQuests.Any()) return;
                var quest = possibleQuests[0];

                SetTravelLocation(quest, "feyden_cave");
                AddQuest(quest);
            }

            if (OvermorrowWorld.savedFeyden) CompleteMiscRequirement("feyden_fight");
            #endregion
        }

        private void AutoCompleteRequirements()
        {
            foreach (var questState in CurrentQuests)
            {
                foreach (var requirement in questState.Quest.Requirements)
                {
                    if (!(requirement is ChainRequirement chainRequirement)) continue;

                    foreach (var clause in chainRequirement.AllClauses)
                    {
                        if (clause is ItemRequirement || clause is MiscRequirement)
                        {
                            //Main.NewText(clause.ID + " canHandIn: " + clause.CanHandInRequirement(this, questState) + " isCompleted: " + clause.IsCompleted(this, questState));

                            if (clause.CanHandInRequirement(this, questState) && !clause.IsCompleted(this, questState))
                            {
                                clause.TryCompleteRequirement(this, questState);
                                RequirementCompleteAction(clause.ID);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateTravelMarkers()
        {
            foreach (var (_, req) in Quests.State.GetActiveRequirementsOfType<TravelRequirementState>(this))
            {
                if (!req.IsCompleted)
                {
                    if (markerCounter % 30 == 0)
                    {
                        Particle.CreateParticle(Particle.ParticleType<Pulse>(), (req.Requirement as TravelRequirement).Location,
                            Vector2.Zero, Color.Yellow, 1, 0.3f, 0, 0, 480);
                    }

                    if (Player.active && Player.Distance((req.Requirement as TravelRequirement).Location) < 50) req.IsCompleted = true;
                }
            }

            markerCounter++;
        }
    }
}