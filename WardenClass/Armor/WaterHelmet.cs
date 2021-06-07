using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using OvermorrowMod.Items.Materials;
using WardenClass;

namespace OvermorrowMod.WardenClass.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class WaterHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sullen Binder's Gaze");
            Tooltip.SetDefault("");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 24;
            item.value = Item.sellPrice(gold: 1);
            item.rare = ItemRarityID.Orange;
            item.defense = 3;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<WaterArmor>() && legs.type == ModContent.ItemType<WaterLegs>();
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Releases healing water orbs whenever you consume souls";
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            modPlayer.HemoArmor = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DrippingFlesh>(), 40);
            recipe.AddIngredient(ModContent.ItemType<BloodGem>(), 1);
            recipe.AddIngredient(ItemID.IronHelmet, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DrippingFlesh>(), 40);
            recipe.AddIngredient(ModContent.ItemType<BloodGem>(), 1);
            recipe.AddIngredient(ItemID.LeadHelmet, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();

        }
    }
}