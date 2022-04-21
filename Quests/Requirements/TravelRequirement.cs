using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace OvermorrowMod.Quests.Requirements
{
    public class TravelRequirement : IQuestRequirement
    {
        public Vector2 location;
        public bool completed = false;

        public TravelRequirement(Vector2 location)
        {
            this.location = location;
        }

        public string Description => $"Travel to {location}";

        public bool IsCompleted(Player player)
        {
            return completed;
        }

        public void ResetState(Player player) { }
    }
}