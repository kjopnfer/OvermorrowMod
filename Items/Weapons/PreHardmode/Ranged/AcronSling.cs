using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using OvermorrowMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Ranged
{
    public class AcronSling : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acorn Slingshot");
            Tooltip.SetDefault("Uses acorns as ammo");
        }

        public override void SetDefaults()
        {
            item.damage = 15;
            item.ranged = true;
            item.width = 40;
            item.height = 20;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = 10000;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shootSpeed = 10f;
            item.shoot = ModContent.ProjectileType<AcornProj>();
            item.useAmmo = ItemID.Acorn;
            item.crit = 6;
        }

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Wood, 45);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
