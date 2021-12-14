using OvermorrowMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class HemoLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodrazor Greaves");
            Tooltip.SetDefault("7% increased movement speed");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 18;
            item.value = Item.sellPrice(gold: 1);
            item.rare = ItemRarityID.Orange;
            item.defense = 6;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<HemoBreastplate>() && head.type == ModContent.ItemType<HemoHelmet>();
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += .7f;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "All piercing weapons have a chance to inflict two stacks of Bleeding";
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            modPlayer.HemoArmor = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MutatedFlesh>(), 35);
            recipe.AddIngredient(ModContent.ItemType<BloodGem>(), 1);
            recipe.AddIngredient(ItemID.IronGreaves, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MutatedFlesh>(), 35);
            recipe.AddIngredient(ModContent.ItemType<BloodGem>(), 1);
            recipe.AddIngredient(ItemID.LeadGreaves, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();

        }
    }
}