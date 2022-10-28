using OvermorrowMod.Common;
using OvermorrowMod.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.Galvan
{
    [AutoloadEquip(EquipType.Head)]
    public class SkyHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galvan Helmet");
            Tooltip.SetDefault("7% increased move speed \n2% increased ranged damage");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SkyChestplate>() && legs.type == ModContent.ItemType<SkyLegs>();
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.07f;
            player.GetDamage(DamageClass.Ranged) += 0.02f;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.moveSpeed += 0.1f;
            player.setBonus = "10% increased movement speed";
            player.GetModPlayer<OvermorrowModPlayer>().SkyArmor = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SunplateBlock, 7)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
