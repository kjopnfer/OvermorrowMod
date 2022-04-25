using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.ShroomThrower
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
            Item.rare = ItemRarityID.Blue;
            Item.width = 32;
            Item.height = 32;
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.UseSound = SoundID.Item34;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.knockBack = 0.1f;
            Item.shoot = ModContent.ProjectileType<ShroomFlame>();
            Item.useAmmo = 183;
            Item.useAmmo = ItemID.Gel;
            Item.shootSpeed = 15f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GlowingMushroom, 35)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
