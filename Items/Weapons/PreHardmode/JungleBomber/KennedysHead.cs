using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace OvermorrowMod.Items.Weapons.PreHardmode.JungleBomber
{

    public class KennedysHead : ModItem
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chemical Warfare");
            Tooltip.SetDefault("Shoots out exploding vines \nThe explosions do not hurt you \n'Crazy, crazy, crazy, crazy, crazy, crazy, crazy, crazy'");
        }


        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Yellow;
            item.channel = true;
            item.width = 32;
            item.melee = true;
            item.height = 32;
            item.damage = 85;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.autoReuse = true;
            item.knockBack = 0;
            item.shoot = ProjectileID.MoonlordTurretLaser;
            item.shootSpeed = 3.5f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int RandomShoot = Main.rand.Next(-1, 2);
            item.shootSpeed = 3.7f + RandomShoot;
            int numberProjectiles = 1;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(45f));
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, mod.ProjectileType("JungleBomb"), item.damage, 3, player.whoAmI);
            }
            return true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType("JungleEssence"), 17);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}