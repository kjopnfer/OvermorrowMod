using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.VanillaOverrides.Elements
{
    public class ElementalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public HashSet<Element> ElementTypes = new HashSet<Element>() { Element.None };

        public override void SetDefaults(Item item)
        {
            switch (item.type)
            {
                case ItemID.WandofSparking:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
            }
        }
    }
}