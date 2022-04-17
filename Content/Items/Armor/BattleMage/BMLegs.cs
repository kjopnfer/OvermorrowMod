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
            Item.width = 22;
            Item.height = 18;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 7;
        }

        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += 50;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldGreaves)
                .AddIngredient(ItemID.ManaCrystal, 2)
                .AddIngredient<ManaBar>(5)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.PlatinumGreaves)
                .AddIngredient(ItemID.ManaCrystal, 2)
                .AddIngredient<ManaBar>(5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}