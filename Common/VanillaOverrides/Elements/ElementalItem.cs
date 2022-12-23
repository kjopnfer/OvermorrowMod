using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.VanillaOverrides.Elements
{
    public class ElementalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public List<Element> ElementTypes = new List<Element>() { Element.None };

    }
}