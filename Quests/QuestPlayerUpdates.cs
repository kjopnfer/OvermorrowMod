using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Quests.Requirements;
using OvermorrowMod.Quests.State;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Quests
{
    public partial class QuestPlayer : ModPlayer
    {
        private void RequirementCompleteAction(string id)
        {
            switch (id)
            {
                case "axe":
                    // make it put the popup for the axe
                    break;
                case "slime":
                    // make it put the popup for the slime
                    break;
            }
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
                        if (clause is ItemRequirement)
                        {
                            if (clause.CanHandInRequirement(this, questState) && !clause.IsCompleted(this, questState))
                            {
                                clause.TryCompleteRequirement(this, questState);
                                RequirementCompleteAction(clause.ID);

                                Main.NewText(clause.ID + " can be completed");
                            }
                        }
                        //Main.NewText(clause.ID + " " + clause.IsCompleted(this, quest));
                    }

                }

            }

            if (FindActiveQuest("GuideCampfire"))
            {
                /*var npc = Main.npc[Main.LocalPlayer.talkNPC];
                var quest = npc.GetGlobalNPC<QuestNPC>().GetCurrentQuest(npc, out var isDoing);

                
                if (!isDoing) return;

                QuestPlayer questPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
                var questState = Quests.Quests.State.GetActiveQuestState(questPlayer, quest);*/

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
        }

    }
}