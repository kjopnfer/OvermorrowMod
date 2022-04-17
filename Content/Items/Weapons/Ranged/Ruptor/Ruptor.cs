using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Ruptor
{
    public class Ruptor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Boomruptor Cannon");
            Tooltip.SetDefault("Fires a bomb that sticks into enemies");
        }

        public override void SetDefaults()
        {
            Item.damage = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 31;
            Item.useAnimation = 31;
            Item.autoReuse = true;
            Item.shootSpeed = 17f;
            Item.knockBack = 0;
            Item.DamageType = DamageClass.Ranged;
            Item.UseSound = SoundID.Item61;
            Item.shoot = ModContent.ProjectileType<AmoungUsExplosive>();
            Item.scale = 0.86f;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bone, 30)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
    }
}
