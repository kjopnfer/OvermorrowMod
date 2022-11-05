using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.Galvan
{

    [AutoloadEquip(EquipType.Body)]
    public class GalvanArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galvan Breastplate");
            Tooltip.SetDefault("7% increased move speed \n2% increased ranged damage");
        }

        public override void SetDefaults()
        {
            Item.width = 26; //filler 
            Item.height = 20;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.07f;
            player.GetDamage(DamageClass.Ranged) += 0.02f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SunplateBlock, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
