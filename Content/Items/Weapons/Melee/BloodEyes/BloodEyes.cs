using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using Terraria;
using Terraria.DataStructures;
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
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.damage = 14;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 0.9f;
            Item.rare = ItemRarityID.Orange;
            Item.crit = 4;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item19;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BloodyEye>();
            Item.shootSpeed = 15f;
            Item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numberProjectiles = 1;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed2 = velocity.RotatedBy(MathHelper.ToRadians(-10f));
                Vector2 perturbedSpeed1 = velocity.RotatedBy(MathHelper.ToRadians(10f));
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed1.X, perturbedSpeed1.Y, ModContent.ProjectileType<BloodyEye>(), Item.damage, 3, player.whoAmI);
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed2.X, perturbedSpeed2.Y, ModContent.ProjectileType<BloodyEye>(), Item.damage, 3, player.whoAmI);
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
            CreateRecipe()
                .AddIngredient(ItemID.CrimtaneBar, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}

