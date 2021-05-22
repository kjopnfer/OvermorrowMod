using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Materials
{
    public class CrystalMana : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystallized Mana");
        }

        public override void SetDefaults()
        {
            item.width = 9;
            item.height = 14;
            item.rare = ItemRarityID.Green;
            item.maxStack = 99;
        }
    }
}