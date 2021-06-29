using OvermorrowMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;


namespace OvermorrowMod.Items.Weapons.PreHardmode.Melee
{

    public class StarSlasher : ModItem
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star Slasher");
            Tooltip.SetDefault("Calls swords from above you that do half damage");
        }
        public override void SetDefaults()
        {
            item.damage = 27;
            item.noMelee = false;
            item.melee = true;
            item.rare = ItemRarityID.Orange;
            item.knockBack = 3;
            item.useTime = 18;
            item.useAnimation = 18;
            item.autoReuse = true;
            item.UseSound = SoundID.Item20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.shootSpeed = 15f;
            item.shoot = ProjectileID.MoonlordTurretLaser;
            item.value = Item.sellPrice(silver: 50);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {

            int numberProjectiles = 1;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(0.5f));
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<StarSlasherProj>(), item.damage / 2, 3, player.whoAmI);
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe1 = new ModRecipe(mod);
            recipe1.AddIngredient(824, 25);
            recipe1.AddIngredient(65, 1);
            recipe1.AddTile(TileID.Anvils);
            recipe1.SetResult(this, 1);
            recipe1.AddRecipe();
        }
    }
}
