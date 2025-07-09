using OvermorrowMod.Core.Interfaces;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Globals
{
    public class GlobalItems : GlobalItem
    {
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (item.ModItem is IBowModifier bowModifier)
            {
                player.GetModPlayer<BowPlayer>().AddBowModifier(bowModifier);
            }
        }
    }
}