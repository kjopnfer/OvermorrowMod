using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.BloodEyes
{
    public class BloodEyes : ModItem
    {
        public override string Texture => AssetDirectory.Melee + "BloodEyes/BloodyEye";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloody Eyes");
            Tooltip.SetDefault("Three Boomerangs that shoot out together");
        }

        public override void SetDefaults()
        {
            item.melee = true;
            item.noMelee = true;
            item.damage = 14;
            item.useTime = 26;
            item.useAnimation = 26;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 0.9f;
            item.rare = ItemRarityID.Orange;
            item.crit = 4;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item19;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<BloodyEye>();
            item.shootSpeed = 15f;
            item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {

            int numberProjectiles = 1;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed2 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(-10f));
                Vector2 perturbedSpeed1 = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(10f));
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed1.X, perturbedSpeed1.Y, ModContent.ProjectileType<BloodyEye>(), item.damage, 3, player.whoAmI);
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed2.X, perturbedSpeed2.Y, ModContent.ProjectileType<BloodyEye>(), item.damage, 3, player.whoAmI);
            }
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            {
                return player.ownedProjectileCounts[ModContent.ProjectileType<BloodyEye>()] < 1;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe1 = new ModRecipe(mod);
            recipe1.AddIngredient(ItemID.CrimtaneBar, 8);
            recipe1.AddTile(TileID.Anvils);
            recipe1.SetResult(this);
            recipe1.AddRecipe();
        }
    }
}

