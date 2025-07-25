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

            if (item.ModItem is IBowDrawEffects drawEffects)
            {
                player.GetModPlayer<BowPlayer>().AddBowDrawEffect(drawEffects);
            }

            if (item.ModItem is IGunModifier gunModifier)
            {
                player.GetModPlayer<GunPlayer>().AddGunModifier(gunModifier);
            }
        }
    }
}