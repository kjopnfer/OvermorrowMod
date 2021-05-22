using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Materials
{
    public class HarpyLeg : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Harpy Leg");
            Tooltip.SetDefault("'dude what'");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
            item.rare = ItemRarityID.Green;
            item.maxStack = 99;
        }
    }
}