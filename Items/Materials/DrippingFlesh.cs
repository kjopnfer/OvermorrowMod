using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Materials
{
    public class DrippingFlesh : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dripping Flesh");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 26;
            item.rare = ItemRarityID.Green;
            item.maxStack = 999;
        }
    }
}