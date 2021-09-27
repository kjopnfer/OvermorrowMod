using OvermorrowMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class BMBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Battle Mage Breastplate");
            Tooltip.SetDefault("5% increased melee critical strike chance\n10% magic critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 22;
            item.value = Item.sellPrice(gold: 1);
            item.rare = ItemRarityID.Blue;
            item.defense = 8;
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeCrit += 5;
            player.magicCrit += 10;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.GoldChainmail, 1);
            recipe.AddIngredient(ItemID.ManaCrystal, 3);
            recipe.AddIngredient(ModContent.ItemType<ManaBar>(), 8);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PlatinumChainmail, 1);
            recipe.AddIngredient(ItemID.ManaCrystal, 3);
            recipe.AddIngredient(ModContent.ItemType<ManaBar>(), 8);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}