using Terraria.ModLoader;
using OvermorrowMod.Content.NPCs.Mercenary.Paladin;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Common.NPCs.Mercenary
{
    public class PaladinStats : BaseMercenary
    {

        public override int MercenaryID => ModContent.NPCType<Paladin>();
        public override MercenaryType MercenaryType => MercenaryType.Melee;
        protected override List<int> MercenaryEquipment => new List<int>() { ItemID.Torch };
    }
}