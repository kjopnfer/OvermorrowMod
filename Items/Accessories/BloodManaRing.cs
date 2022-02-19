using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Accessories
{
    public class BloodManaRing : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Magic weapons use health instead of mana \n20% Increased Magic Damage \nYou can go into negitive health while using this item, getting hit on negitive health means death \nIncreased Regeneration \nDamage types of other classes are reduced");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 26;
            item.accessory = true;
            item.consumable = true;
            item.rare = ItemRarityID.Orange;
            item.value = Item.buyPrice(gold: 1);
        }

        public override void UpdateAccessory(Terraria.Player player, bool hideVisual)
        {
            player.magicDamage += 0.70f;
            player.lifeRegen += 8;
            player.GetModPlayer<OvermorrowModPlayer>().Bloodmana = true;
            player.allDamage -= 0.50f;
            player.statManaMax = 200;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe1 = new ModRecipe(mod);
            recipe1.AddIngredient(ItemID.BandofStarpower, 1);
            recipe1.AddIngredient(ItemID.BandofRegeneration, 1);
            recipe1.SetResult(this, 1);
            recipe1.AddRecipe();

            ModRecipe recipe2 = new ModRecipe(mod);
            recipe2.AddIngredient(ItemID.PanicNecklace, 1);
            recipe2.AddIngredient(ItemID.BandofRegeneration, 1);
            recipe2.SetResult(this, 1);
            recipe2.AddRecipe();

            ModRecipe recipe3 = new ModRecipe(mod);
            recipe3.AddIngredient(mod.ItemType("SoulFire"), 1);
            recipe3.AddIngredient(mod.ItemType("DrippingFlesh"), 1);
            recipe3.SetResult(this, 1);
            recipe3.AddRecipe();


            ModRecipe recipe4 = new ModRecipe(mod);
            recipe4.AddIngredient(ItemID.ManaCrystal, 1);
            recipe4.AddIngredient(ItemID.LifeCrystal, 1);
            recipe4.SetResult(this, 1);
            recipe4.AddRecipe();

            ModRecipe recipe5 = new ModRecipe(mod);
            recipe5.AddIngredient(mod.ItemType("BloodGem"), 1);
            recipe5.AddIngredient(mod.ItemType("CrystalMana"), 1);
            recipe5.SetResult(this, 1);
            recipe5.AddRecipe();
        }
    }
}