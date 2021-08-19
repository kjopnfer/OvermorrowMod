using OvermorrowMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Ranged
{
    public class SkullGun : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Converts Musket Balls and Silver Bullets into Fire Bullets\n'Snooze dart sold separately'");
        }
        public override void SetDefaults()
        {
            item.damage = 17;
            item.ranged = true;
            item.width = 40;
            item.height = 25;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4;
            item.value = 10000;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item40;
            item.shoot = ProjectileType<FlameCone>();
            item.autoReuse = true;
            item.shootSpeed = 5.5f;
            item.scale = 0.7f;
            item.useAmmo = AmmoID.Bullet;
        }
        public override bool Shoot(Terraria.Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (type == ProjectileID.Bullet)
            {
                type = ProjectileType<FlameCone>();
            }
            return true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe1 = new ModRecipe(mod);
            recipe1.AddIngredient(164, 1);
            recipe1.AddIngredient(175, 20);
            recipe1.AddIngredient(154, 15);
            recipe1.AddTile(TileID.Anvils);
            recipe1.SetResult(this);
            recipe1.AddRecipe();


            ModRecipe recipe2 = new ModRecipe(mod);
            recipe2.AddIngredient(219, 1);
            recipe2.AddIngredient(175, 20);
            recipe2.AddIngredient(154, 15);
            recipe2.AddTile(TileID.Anvils);
            recipe2.SetResult(this);
            recipe2.AddRecipe();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
