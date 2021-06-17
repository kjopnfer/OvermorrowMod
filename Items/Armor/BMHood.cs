using OvermorrowMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Armor
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
            item.width = 24;
            item.height = 22;
            item.value = Item.sellPrice(gold: 1);
            item.rare = ItemRarityID.Blue;
            item.defense = 6;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<BMBreastplate>() && legs.type == ModContent.ItemType<BMLegs>();
        }

        public override void UpdateEquip(Player player)
        {
            player.magicDamage += 0.1f;
            player.meleeDamage += 0.05f;
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
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.GoldHelmet, 1);
            recipe.AddIngredient(ItemID.ManaCrystal, 2);
            recipe.AddIngredient(ModContent.ItemType<ManaBar>(), 6);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PlatinumHelmet, 1);
            recipe.AddIngredient(ItemID.ManaCrystal, 2);
            recipe.AddIngredient(ModContent.ItemType<ManaBar>(), 6);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}