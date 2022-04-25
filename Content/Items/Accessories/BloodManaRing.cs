using OvermorrowMod.Common;
using OvermorrowMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories
{
    public class BloodManaRing : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Magic weapons use health instead of mana \n20% Increased Magic Damage \nYou can go into negitive health while using this Item, getting hit on negitive health means death \nIncreased Regeneration \nDamage types of other classes are reduced");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.accessory = true;
            Item.consumable = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 1);
        }

        public override void UpdateAccessory(Terraria.Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += 0.70f;
            player.lifeRegen += 8;
            player.GetModPlayer<OvermorrowModPlayer>().Bloodmana = true;
            player.GetDamage(DamageClass.Generic) -= 0.50f;
            player.statManaMax = 200;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BandofStarpower, 1)
                .AddIngredient(ItemID.BandofRegeneration, 1)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.PanicNecklace, 1)
                .AddIngredient(ItemID.BandofRegeneration, 1)
                .Register();

            CreateRecipe()
                .AddIngredient<SoulFire>()
                .AddIngredient<MutatedFlesh>()
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.ManaCrystal)
                .AddIngredient(ItemID.LifeCrystal)
                .Register();

            CreateRecipe()
                .AddIngredient<BloodGem>()
                .AddIngredient<CrystalMana>()
                .Register();
        }
    }
}