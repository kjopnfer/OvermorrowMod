using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee
{
    public class WaterSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lacusite Falchion");
            Tooltip.SetDefault("Launches a short ranged water slash");
        }

        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.damage = 20;
            item.useTime = 30;
            item.useAnimation = 30;
            item.width = 56;
            item.height = 54;
            item.shoot = ModContent.ProjectileType<WaterSlash>();
            item.shootSpeed = 8f;
            item.knockBack = 2f;
            item.melee = true;
            item.autoReuse = true;
            item.value = Item.sellPrice(gold: 1);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<WaterBar>(), 9);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}