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
            Tooltip.SetDefault("Increases jump height");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 18;
            item.value = Item.sellPrice(gold: 1);
            item.rare = ItemRarityID.Green;
            item.defense = 5;
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            player.jumpSpeedBoost += 2f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<WaterBar>(), 12);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}