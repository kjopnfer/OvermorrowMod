using OvermorrowMod.Common;
using OvermorrowMod.Common.Players;
using OvermorrowMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Armor.BattleMage
{
    [AutoloadEquip(EquipType.Head)]
    public class BMHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Battle Mage Mask");
            Tooltip.SetDefault("5% increased melee damage and 10% increased magic damage");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 22;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 6;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<BMBreastplate>() && legs.type == ModContent.ItemType<BMLegs>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.1f;
            player.GetDamage(DamageClass.Melee) += 0.05f;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Increases mana cost by 20%\nYou cannot regenerate mana\nMelee attacks restore mana proportional to the damage dealt";
            player.manaRegen -= 99999;
            player.manaRegenBonus -= 99999;
            player.manaCost += 0.20f;
            player.GetModPlayer<OvermorrowModPlayer>().BMSet = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GoldHelmet, 1)
                .AddIngredient(ItemID.ManaCrystal, 2)
                .AddIngredient<ManaBar>(6)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.PlatinumHelmet, 1)
                .AddIngredient(ItemID.ManaCrystal, 2)
                .AddIngredient<ManaBar>(6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}