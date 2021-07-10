using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using OvermorrowMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using OvermorrowMod.Items.Materials;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Ranged
{
    public class ShurikenCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shuriken Cannon");
            Tooltip.SetDefault("Uses shurikens as ammo");
        }
        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Blue;
            item.width = 32;
            item.height = 32;
            item.damage = 5;
            item.ranged = true;
            item.noMelee = true;
            item.useTime = 20;
            item.useAnimation = 20;
            item.UseSound = SoundID.Item61;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.knockBack = 0.1f;
            item.shoot = ProjectileID.MoonlordTurretLaser;
            item.shootSpeed = 2f;
            item.useAmmo = 42;
        }



        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(22, 20);
            recipe.AddIngredient(42, 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();

            ModRecipe recipe2 = new ModRecipe(mod);
            recipe2.AddIngredient(704, 20);
            recipe2.AddIngredient(42, 3);
            recipe2.AddTile(TileID.Anvils);
            recipe2.SetResult(this);
            recipe2.AddRecipe();

            
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
