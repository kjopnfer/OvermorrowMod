using Terraria.ID;

namespace OvermorrowMod.Core.Loot
{
    public static class RarityUtils
    {
        public static int ToVanilla(Rarity rarity)
        {
            return rarity switch
            {
                Rarity.Rare => ItemRarityID.Blue,
                Rarity.Epic => ItemRarityID.LightPurple,
                Rarity.Legendary => ItemRarityID.Yellow,
                _ => ItemRarityID.White,
            };
        }
    }
}
