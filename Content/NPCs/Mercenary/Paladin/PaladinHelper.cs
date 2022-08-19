using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.GameContent;

namespace OvermorrowMod.Content.NPCs.Mercenary.Paladin
{
    public partial class Paladin : Mercenary
    {
        /// <summary>
        /// Checks if the NPC's velocity is above 0 and the previous direction is the same as the current one, return true
        /// </summary>
        /// <returns>A boolean value determining if the NPC is moving the same direction</returns>
        bool SameDirection() => NPC.velocity.X != 0 && NPC.direction == NPC.oldDirection;
    }
}