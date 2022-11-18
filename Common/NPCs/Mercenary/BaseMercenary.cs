using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.NPCs.Mercenary
{
    public enum MercenaryType
    {
        Melee,
        Magic,
        Ranged,
        Summon
    }
    public abstract class BaseMercenary
    {
        // mercenary needs to store who they are
        public virtual int MercenaryID { get; }
        public virtual MercenaryType MercenaryType { get; }

        // the mercenary must store their levels
        public int MercenaryLevel;

        // the mercenary must store their experience
        public int MercenaryExperience;

        // the mercenary must store their equipment
        protected virtual List<int> MercenaryEquipment { get; } = new List<int>();

        // mercenary stats are stored in a dictionary?
        // dictionary is saved by the player
    }
}