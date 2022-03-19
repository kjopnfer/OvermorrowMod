using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.PreHardmode.Ranged
{
    public class Catfish : ModItem
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Fish-Eye");
            Tooltip.SetDefault("Transforms bullets into high velocity water bullets");
        }

        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item41;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 19;
            item.useAnimation = 25;
            item.useTime = 25;
            item.width = 52;
            item.height = 34;
            item.shoot = ModContent.ProjectileType<WaterBullet>();
            item.shootSpeed = 15;
            item.knockBack = 5f;
            item.ranged = true;
            item.value = Item.sellPrice(gold: 1);
            item.useAmmo = AmmoID.Bullet;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY - 5)) * 45f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            if (type == ProjectileID.Bullet)
            {
                type = ModContent.ProjectileType<WaterBullet>();
            }
            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<WaterBar>(), 7);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}