using Microsoft.Xna.Framework;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Quests.State;
using System;
using Terraria;

namespace OvermorrowMod.Quests.Requirements
{
    public class TravelRequirement : BaseQuestRequirement<TravelRequirementState>
    {
        private Vector2? location = null;
        public Vector2 Location { get
            {
                if (location == null) location = locationGenerator();
                return location.Value;
            } }
        private readonly Func<Vector2> locationGenerator;

        public TravelRequirement(Func<Vector2> locationGenerator, string id) : base(id)
        {
            this.locationGenerator = locationGenerator;
        }

        public override string Description => $"Travel to {location}";

        protected override bool IsRequirementCompletable(QuestPlayer player, BaseQuestState state, TravelRequirementState reqState)
        {
            // In this class, we are using the completable requirement state, but it is set to completed externally,
            // so we don't actually need to do anything in here. Just return false from here so that we don't get "turn in" on
            // NPCs, and do nothing in `CompleteRequirement`
            return false;
        }

        protected override void CompleteRequirement(QuestPlayer player, BaseQuestState state, TravelRequirementState reqState)
        {
        }
    }
}