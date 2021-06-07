using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using OvermorrowMod.Items.Materials;
using WardenClass;

namespace OvermorrowMod.WardenClass.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class WaterLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sullen Binder Greaves");
            Tooltip.SetDefault("");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 18;
            item.value = Item.sellPrice(gold: 1);
            item.rare = ItemRarityID.Orange;
            item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DrippingFlesh>(), 35);
            recipe.AddIngredient(ModContent.ItemType<BloodGem>(), 1);
            recipe.AddIngredient(ItemID.IronGreaves, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DrippingFlesh>(), 35);
            recipe.AddIngredient(ModContent.ItemType<BloodGem>(), 1);
            recipe.AddIngredient(ItemID.LeadGreaves, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();

        }
    }
}