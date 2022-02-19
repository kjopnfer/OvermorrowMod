/*using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.Hardmode.HardMagic.GemTomes
{
    public class RainbowWave : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rainbow Wave");
            Tooltip.SetDefault("Shoots an array of colourful waves in 8 directions");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.AnimatesAsSoul[item.type] = true;
            ItemID.Sets.ItemIconPulse[item.type] = false;
            ItemID.Sets.ItemNoGravity[item.type] = false;
        }

        public override void SetDefaults()
        {
            item.damage = 87;
            item.noMelee = true;
            item.magic = true;
            item.channel = true; //Channel so that you can hold the weapon [Important]
            item.mana = 5;
            item.rare = ItemRarityID.Pink;
            item.width = 28;
            item.height = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.shootSpeed = 0f;
            item.useAnimation = 60;
            item.useTime = 20;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.shoot = mod.ProjectileType("Splitter");
            item.value = Item.sellPrice(silver: 3);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe1 = new ModRecipe(mod);
            recipe1.AddIngredient(mod.ItemType("DiamondRay"), 1);
            recipe1.AddIngredient(mod.ItemType("EmeraldSplash"), 1);
            recipe1.AddIngredient(mod.ItemType("RubyStorm"), 1);
            recipe1.AddIngredient(mod.ItemType("SapphireSlugger"), 1);
            recipe1.AddIngredient(mod.ItemType("TopazBubbler"), 1);
            recipe1.AddIngredient(mod.ItemType("AmethystShooter"), 1);
            recipe1.AddTile(TileID.Anvils);
            recipe1.SetResult(this);
            recipe1.AddRecipe();
        }
    }
}*/