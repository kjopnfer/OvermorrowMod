using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
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
            item.autoReuse = true;
            item.rare = ItemRarityID.Blue;
            item.mana = 2;
            item.UseSound = SoundID.Item20;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 20;
            item.useTurn = false;
            item.useAnimation = 14;
            item.useTime = 14;
            item.width = 28;
            item.height = 30;
            item.shoot = ProjectileID.MoonlordTurretLaser;
            item.shootSpeed = 2f;
            item.knockBack = 3f;
            item.magic = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(183, 35);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
        
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {

            int numberProjectiles = 3;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Projectile.NewProjectile(player.Center.X + Main.rand.Next(-75, 76), player.Center.Y + Main.rand.Next(-75, 76), 0, 0, 590, item.damage, 3, player.whoAmI);
            }
            return true;
        }
    }
}