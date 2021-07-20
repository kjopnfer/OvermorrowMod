using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using OvermorrowMod.Items.Materials;
using WardenClass;

namespace OvermorrowMod.WardenClass.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class HemoBreastplate : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodrazor Breastplate");
            Tooltip.SetDefault("Soul Essence gain chance increased by 5%");
        }

        public override void SetDefaults()
        {
            item.width = 34;
            item.height = 24;
            item.value = Item.sellPrice(gold: 1);
            item.rare = ItemRarityID.Orange;
            item.defense = 7;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ModContent.ItemType<HemoHelmet>() && legs.type == ModContent.ItemType<HemoLeggings>();
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            modPlayer.soulGainBonus += 5;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Piercing weapons have a chance to inflict two stacks of Bleeding";
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            modPlayer.HemoArmor = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DrippingFlesh>(), 45);
            recipe.AddIngredient(ModContent.ItemType<BloodGem>(), 1);
            recipe.AddIngredient(ItemID.IronChainmail, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DrippingFlesh>(), 45);
            recipe.AddIngredient(ModContent.ItemType<BloodGem>(), 1);
            recipe.AddIngredient(ItemID.LeadChainmail, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}