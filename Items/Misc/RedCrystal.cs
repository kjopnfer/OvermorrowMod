using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Misc
{
    public class RedCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Resonance Crystal of Primality");
            Tooltip.SetDefault("Used to enhance your strength");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 26;
            item.rare = ItemRarityID.Green;
            item.maxStack = 99;
        }
    }
}