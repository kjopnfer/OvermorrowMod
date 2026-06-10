using Microsoft.Xna.Framework;

namespace OvermorrowMod.Core.Loot
{
    public static class RarityColors
    {
        public static Color For(Rarity rarity)
        {
            return rarity switch
            {
                Rarity.Rare => new Color(80, 150, 255),
                Rarity.Epic => new Color(180, 100, 255),
                Rarity.Legendary => new Color(255, 175, 45),
                _ => new Color(88, 214, 141),
            };
        }
    }
}
