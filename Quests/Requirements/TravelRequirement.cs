using Microsoft.Xna.Framework;
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

        public bool completed = false;

        public TravelRequirement(Func<Vector2> locationGenerator, string ID)
        {
            this.locationGenerator = locationGenerator;
            this.ID = ID;
        }

        public string Description => $"Travel to {location}";

        public bool IsCompleted(Player player)
        {
            // For debugging purposes
            if (completed) return true;

            foreach (var location in QuestSystem.PlayerTraveled)
            {
                if (location == ID) return true;
            }

            return false;
        }

        public void ResetState(Player player)
        {
            if (QuestSystem.PlayerTraveled.Contains(ID))
            {
                QuestSystem.PlayerTraveled.Remove(ID);
            }

            if (player.GetModPlayer<QuestPlayer>().SelectedLocation == ID)
            {
                player.GetModPlayer<QuestPlayer>().SelectedLocation = null;
            }
            location = null;
        }
    }
}