using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Loot.Rarities
{
    public class LegendaryRarity : ModRarity
    {
        public override Color RarityColor => RarityColors.For(Rarity.Legendary);
    }
}
