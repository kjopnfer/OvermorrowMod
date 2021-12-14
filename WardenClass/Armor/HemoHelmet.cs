using OvermorrowMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class HemoHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodrazor Mask");
            Tooltip.SetDefault("Increases Piercing damage by 3");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 20;
            item.value = Item.sellPrice(gold: 1);
            item.rare = ItemRarityID.Orange;
            item.defense = 5;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<HemoBreastplate>() && legs.type == ModContent.ItemType<HemoLeggings>();
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            modPlayer.piercingDamageAdd += 3;
            //modPlayer.piercingDamageMult *= 1.08f;
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
            recipe.AddIngredient(ModContent.ItemType<MutatedFlesh>(), 40);
            recipe.AddIngredient(ModContent.ItemType<BloodGem>(), 1);
            recipe.AddIngredient(ItemID.IronHelmet, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MutatedFlesh>(), 40);
            recipe.AddIngredient(ModContent.ItemType<BloodGem>(), 1);
            recipe.AddIngredient(ItemID.LeadHelmet, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();

        }
    }
}