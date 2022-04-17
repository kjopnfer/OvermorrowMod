using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Catfish
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
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item41;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 19;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.width = 52;
            Item.height = 34;
            Item.shoot = ModContent.ProjectileType<WaterBullet>();
            Item.shootSpeed = 15;
            Item.knockBack = 5f;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(gold: 1);
            Item.useAmmo = AmmoID.Bullet;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y - 5)) * 45f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            if (type == ProjectileID.Bullet)
            {
                type = ModContent.ProjectileType<WaterBullet>();
            }
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<WaterBar>(), 7).AddTile(TileID.Anvils).Register();
        }
    }
}