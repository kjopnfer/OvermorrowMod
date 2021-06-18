using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.Hardmode.BiomeWep
{
    public class RainbowBulletGun : ModItem
    {
        public string devtooltip;
        public override void SetDefaults()
        {
            item.damage = 65;
            item.ranged = true;
            item.width = 26;
            item.height = 52;
            item.useTime = 7;
            item.useAnimation = 7;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2;
            item.UseSound = SoundID.Item36;
            item.rare = ItemRarityID.Yellow;
            item.autoReuse = true;
            item.shoot = ProjectileID.MoonlordTurretLaser;
            item.shootSpeed = 14f;
            item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rainbow Mini Gun");
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {

            int numberProjectiles = 1;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(6.45f));
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, mod.ProjectileType("RainbowBullet"), item.damage, 3, player.whoAmI);
            }
            return true;
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType("HallowEssence"), 17);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
