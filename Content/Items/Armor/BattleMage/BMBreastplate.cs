using OvermorrowMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.BattleMage
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
            Item.width = 30;
            Item.height = 22;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 8;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Melee) += 5;
            player.GetCritChance(DamageClass.Magic) += 10;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldChainmail, 1)
                .AddIngredient(ItemID.ManaCrystal, 3)
                .AddIngredient<ManaBar>(8)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.PlatinumChainmail, 1)
                .AddIngredient(ItemID.ManaCrystal, 3)
                .AddIngredient<ManaBar>(8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}