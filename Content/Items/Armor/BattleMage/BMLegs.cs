using OvermorrowMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.BattleMage
{
    [AutoloadEquip(EquipType.Legs)]
    public class BMLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Battle Mage Robes");
            Tooltip.SetDefault("Increases maximum mana by 50");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 18;
            item.value = Item.sellPrice(gold: 1);
            item.rare = ItemRarityID.Blue;
            item.defense = 7;
        }

        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += 50;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.GoldGreaves, 1);
            recipe.AddIngredient(ItemID.ManaCrystal, 2);
            recipe.AddIngredient(ModContent.ItemType<ManaBar>(), 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PlatinumGreaves, 1);
            recipe.AddIngredient(ItemID.ManaCrystal, 2);
            recipe.AddIngredient(ModContent.ItemType<ManaBar>(), 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}