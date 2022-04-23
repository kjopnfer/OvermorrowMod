using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace OvermorrowMod.Quests.Requirements
{
    public class TravelRequirement : IQuestRequirement
    {
        public Vector2 location;
        public string ID;

        public bool completed = false;

        public TravelRequirement(Vector2 location, string ID)
        {
            this.location = location;
            this.ID = ID;
        }

        public string Description => $"Travel to {location}";

        public bool IsCompleted(Player player)
        {
            // For debugging purposes
            if (completed) return true;

            foreach (var location in QuestWorld.PlayerTraveled)
            {
                if (location == ID) return true;
            }

            return false;
        }

        public void ResetState(Player player)
        {
            if (QuestWorld.PlayerTraveled.Contains(ID))
            {
                QuestWorld.PlayerTraveled.Remove(ID);
            }

            if (player.GetModPlayer<QuestPlayer>().SelectedLocation == ID)
            {
                player.GetModPlayer<QuestPlayer>().SelectedLocation = null;
            }
        }
    }
}