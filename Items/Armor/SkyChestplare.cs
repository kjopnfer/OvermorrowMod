using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Items.Armor
{

    [AutoloadEquip(EquipType.Body)]
    public class SkyChestplare : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galvan Breastplate");
            Tooltip.SetDefault("7% increased move speed \n2% increased ranged damage");
        }

        public override void SetDefaults()
        {
            item.width = 26; //filler 
            item.height = 20;
            item.value = Item.sellPrice(0, 2, 50, 0);
            item.rare = ItemRarityID.Blue;
            item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.07f;
            player.rangedDamage += 0.02f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe1 = new ModRecipe(mod);
            recipe1.AddIngredient(ItemID.SunplateBlock, 10);
            recipe1.AddTile(TileID.Anvils);
            recipe1.SetResult(this, 1);
            recipe1.AddRecipe();
        }
    }
}
