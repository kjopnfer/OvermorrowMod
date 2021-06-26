using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class SkyHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sunplate Helmet");
            Tooltip.SetDefault("7% increased move speed \n2% increased ranged damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10000;
            item.rare = ItemRarityID.Blue;
            item.defense = 3;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SkyChestplare>() && legs.type == ModContent.ItemType<SkyLegs>();
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.07f;
            player.rangedDamage += 0.02f;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.moveSpeed += 0.1f;
            player.setBonus = "10% increased movement speed";
        }

        public override void AddRecipes()
        {
            ModRecipe recipe1 = new ModRecipe(mod);
            recipe1.AddIngredient(824, 7);
            recipe1.AddTile(TileID.Anvils);
            recipe1.SetResult(this, 1);
            recipe1.AddRecipe();
        }
    }
}
