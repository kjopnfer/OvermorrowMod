using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.MushroomBook
{
    public class MushroomBook : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mushroom Tome");
            Tooltip.SetDefault("Spews Mushrooms");
        }

        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Blue;
            Item.mana = 2;
            Item.UseSound = SoundID.Item20;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 20;
            Item.useTurn = false;
            Item.useAnimation = 14;
            Item.useTime = 14;
            Item.width = 30;
            Item.height = 36;
            Item.shoot = ProjectileID.MoonlordTurretLaser;
            Item.shootSpeed = 2f;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Magic;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GlowingMushroom, 35)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
        {
            int numberProjectiles = 3;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Projectile.NewProjectile(source, player.Center.X + Main.rand.Next(-75, 76), player.Center.Y + Main.rand.Next(-75, 76), 0, 0, ProjectileID.TruffleSpore, Item.damage, 3, player.whoAmI);
            }
            return true;
        }
    }
}