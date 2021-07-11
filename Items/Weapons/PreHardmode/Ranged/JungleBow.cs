using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Projectiles.Ranged;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Ranged
{
    public class JungleBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vine Bow");
            Tooltip.SetDefault("Shoots 3-6 poison stingers");
        }
        public override void SetDefaults()
        {
            item.damage = 12;
            item.ranged = true;
            item.width = 40;
            item.height = 25;
            item.useTime = 36;
            item.useAnimation = 36;
            item.UseSound = SoundID.Item5;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 0f;
            item.value = 10000;
            item.rare = ItemRarityID.Green;
            item.shoot = ModContent.ProjectileType<PoisonArrow>();
            item.autoReuse = false;
            item.shootSpeed = 8.2f;
            item.useAmmo = AmmoID.Arrow;
        }

        public override bool Shoot(Terraria.Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = ModContent.ProjectileType<PoisonArrow>();
            int numberProjectiles = 3 + Main.rand.Next(1, 4);
            for (int i = 0; i < numberProjectiles; i++)
            {
                Projectile.NewProjectile(position.X, position.Y, speedX + Main.rand.Next(-2, 3), speedY + Main.rand.Next(-2, 3), ModContent.ProjectileType<PoisonArrow>(), item.damage, 3, player.whoAmI);
            }
            return true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe1 = new ModRecipe(mod);
            recipe1.AddIngredient(210, 1);
            recipe1.AddIngredient(620, 14);
            recipe1.AddTile(TileID.Anvils);
            recipe1.SetResult(this);
            recipe1.AddRecipe();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
