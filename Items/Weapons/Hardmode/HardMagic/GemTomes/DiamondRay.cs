
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.Hardmode.HardMagic.GemTomes
{
    public class DiamondRay : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Diamond Ray");
            Tooltip.SetDefault("Shoot a laser beam that can eliminate anything...");
        }

        public override void SetDefaults()
        {
            item.damage = 30;
            item.noMelee = true;
            item.magic = true;
            item.channel = true; //Channel so that you can hold the weapon [Important]
            item.mana = 2;
            item.rare = ItemRarityID.Pink;
            item.width = 28;
            item.height = 30;
            item.useTime = 20;
            item.UseSound = SoundID.Item13;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.shootSpeed = 14f;
            item.useAnimation = 20;
            item.shoot = ModContent.ProjectileType<ExampleLaser>();
            item.value = Item.sellPrice(silver: 3);
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(1527, 1);
            recipe.AddIngredient(548, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
