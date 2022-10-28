using Microsoft.Xna.Framework;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Quests.State;
using System;
using Terraria;

namespace OvermorrowMod.Quests.Requirements
{
    public class TravelRequirement : IQuestRequirement
    {
        private Vector2? location = null;
        public Vector2 Location { get
            {
                if (location == null) location = locationGenerator();
                return location.Value;
            } }
        private readonly Func<Vector2> locationGenerator;
        public string ID { get; }

        public TravelRequirement(Func<Vector2> locationGenerator, string ID)
        {
            this.locationGenerator = locationGenerator;
            this.ID = ID;
        }

        public string Description => $"Travel to {location}";

        public bool IsCompleted(QuestPlayer player, BaseQuestState state)
        {
            var reqState = state.GetRequirementState(this) as TravelRequirementState;

            return reqState.Traveled;
        }

        public void ResetState(Player player)
        {
            if (player.GetModPlayer<QuestPlayer>().SelectedLocation == ID)
            {
                player.GetModPlayer<QuestPlayer>().SelectedLocation = null;
            }
            location = null;
        }

        public BaseRequirementState GetNewState()
        {
            return new TravelRequirementState(this);
        }
    }
}