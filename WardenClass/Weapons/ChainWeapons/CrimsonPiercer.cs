using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Piercing;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Weapons.ChainWeapons
{
    public class CrimsonPiercer : PiercingItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sanguine Impaler");
            Tooltip.SetDefault("[c/00FF00:{ Special Ability }]\n" +
                "[c/800080:Right Click] to launch 3 chains that deal increased damage\nConsumes 1 Soul Essence");
        }

        public override void SafeSetDefaults()
        {
            item.autoReuse = false;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 14;
            item.useTime = 14;
            item.knockBack = 0f;
            item.width = 30;
            item.height = 10;
            item.damage = 6;
            item.shootSpeed = 14f;
            item.shoot = mod.ProjectileType("CrimsonPiercerProjectile");
            item.rare = ItemRarityID.Blue;
            item.UseSound = SoundID.Item71;
            item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player)
        {
            // Get the class info from the player
            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            if(player.altFunctionUse == 2 && modPlayer.soulResourceCurrent > 0)
            {
                item.useStyle = ItemUseStyleID.SwingThrow;
                item.useAnimation = 14;
                item.useTime = 14;
                item.knockBack = 0f;
                item.damage = 12;
                item.shootSpeed = 28f;
                item.shoot = mod.ProjectileType("CrimsonPiercerProjectileAlt");
                item.UseSound = SoundID.Item71;

                ConsumeSouls(1, player);
            }
            else
            {
                item.useStyle = ItemUseStyleID.SwingThrow;
                item.useTurn = true;
                item.useAnimation = 14;
                item.useTime = 14;
                item.damage = 6;
                item.shootSpeed = 14f + modPlayer.modifyShootSpeed();
                item.shoot = mod.ProjectileType("CrimsonPiercerProjectile");
            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if(type == mod.ProjectileType("CrimsonPiercerProjectileAlt"))
            {
                float numberProjectiles = 3; // This defines how many projectiles to shot
                float rotation = MathHelper.ToRadians(15);
                position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f; //this defines the distance of the projectiles form the player when the projectile spawns
                for (int i = 0; i < numberProjectiles; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .4f; // This defines the projectile roatation and speed. .4f == projectile speed
                    Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI);
                }
                return false;
            }

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CrimtaneBar, 12);
            recipe.AddIngredient(ItemID.Chain, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}