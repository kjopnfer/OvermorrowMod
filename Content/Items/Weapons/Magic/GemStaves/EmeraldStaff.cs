using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Items.Materials;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.GemStaves
{
    public class EmeraldStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greater Emerald Staff");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Orange;
            Item.mana = 9;
            Item.UseSound = SoundID.Item43;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 16;
            Item.useTurn = false;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.width = 48;
            Item.height = 48;
            Item.shoot = ModContent.ProjectileType<EmeraldProj>();
            Item.shootSpeed = 7f;
            Item.knockBack = 5f;
            Item.DamageType = DamageClass.Magic;
            Item.value = Item.sellPrice(gold: 1, silver: 75);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numberProjectiles = 1;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed1 = velocity.RotatedBy(MathHelper.ToRadians(21f));
                Vector2 perturbedSpeed2 = velocity.RotatedBy(MathHelper.ToRadians(-21f));
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed1.X, perturbedSpeed1.Y, ModContent.ProjectileType<EmeraldProj>(), Item.damage, 3, player.whoAmI);
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed2.X, perturbedSpeed2.Y, ModContent.ProjectileType<EmeraldProj>(), Item.damage, 3, player.whoAmI);
            }
            return true;
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.EmeraldStaff)
                .AddIngredient<ManaBar>(4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}