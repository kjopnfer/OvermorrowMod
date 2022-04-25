using OvermorrowMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories
{
    public class PredatorsTalisman : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Predator's Talisman");
            Tooltip.SetDefault("Increases armor penetration by 5\n5% increased crit chance\nIncreases all damage by 3");
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 32;
            Item.value = Item.buyPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OvermorrowModPlayer>().PredatorTalisman = true;

            player.armorPenetration += 5;
            player.GetCritChance(DamageClass.Generic) += 5;
            player.GetDamage(DamageClass.Generic) += 3;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SharkToothNecklace)
                .AddIngredient<AnglerTooth>()
                .AddIngredient<SerpentTooth>()
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}