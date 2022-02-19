using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Ranged;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Ranged
{
    public class ShroomThrower : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shroomthrower");
            Tooltip.SetDefault("Uses gel as ammo \nHas a chance to inflict fungal infection");
        }
        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Blue;
            item.width = 32;
            item.height = 32;
            item.damage = 12;
            item.ranged = true;
            item.noMelee = true;
            item.useTime = 10;
            item.useAnimation = 10;
            item.UseSound = SoundID.Item34;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.knockBack = 0.1f;
            item.shoot = ModContent.ProjectileType<ShroomFlame>();
            item.useAmmo = 183;
            item.useAmmo = ItemID.Gel;
            item.shootSpeed = 15f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.GlowingMushroom, 35);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
