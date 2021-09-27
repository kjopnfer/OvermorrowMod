using OvermorrowMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class WaterArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sullen Binder Plate");
            Tooltip.SetDefault("Increases life regen");
        }

        public override void SetDefaults()
        {
            item.width = 34;
            item.height = 24;
            item.value = Item.sellPrice(gold: 1);
            item.rare = ItemRarityID.Green;
            item.defense = 5;
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            // lifeRegen is measured in 1/2 life per second. Therefore, this effect causes 2 life gained per second.
            player.lifeRegen += 4;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<WaterBar>(), 16);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}

