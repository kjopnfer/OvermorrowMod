using OvermorrowMod.Projectiles.Piercing;
using OvermorrowMod.WardenClass.Weapons.ChainWeapons;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Weapons.ChainWeapons
{
    public class BonePiercer : PiercingItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bone Spike");
            Tooltip.SetDefault("[c/00FF00:{ Special Ability }]\n" +
                            "[c/800080:Right Click] to launch a chain that explodes into bones on hit\nConsumes 1 Soul Essence");
        }

        public override void SafeSetDefaults()
        {
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 14;
            item.useTime = 14;
            item.knockBack = 0f;
            item.width = 30;
            item.height = 10;
            item.damage = 9;
            item.shootSpeed = 14f;
            item.shoot = mod.ProjectileType("BonePiercerProjectile");
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item1;
            item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player)
        {
            // Get the class info from the player
            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            if (player.altFunctionUse == 2 && modPlayer.soulResourceCurrent > 0)
            {
                item.useStyle = ItemUseStyleID.SwingThrow;
                item.useAnimation = 14;
                item.useTime = 14;
                item.knockBack = 0f;
                item.damage = 11;
                item.shootSpeed = 28f;
                item.shoot = mod.ProjectileType("BonePiercerProjectileAlt");
                //item.UseSound = SoundID.Item71;

                // Get the instance of the first projectile in the list
                int removeProjectile = modPlayer.soulList[0];

                // Remove the projectile from the list
                modPlayer.soulList.RemoveAt(0);
                modPlayer.soulResourceCurrent--;

                // Call the projectile's method to kill itself
                for (int i = 0; i < Main.maxProjectiles; i++) // Loop through the projectile array
                {
                    // Check that the projectile is the same as the removed projectile and it is active
                    if (Main.projectile[i] == Main.projectile[removeProjectile] && Main.projectile[i].active)
                    {
                        // Kill the projectile
                        Main.projectile[i].Kill();
                    }
                }
            }
            else
            {
                item.autoReuse = true;
                item.useStyle = ItemUseStyleID.SwingThrow;
                item.useTurn = true;
                item.useAnimation = 14;
                item.useTime = 14;
                item.damage = 10;
                item.shootSpeed = 14f;
                item.shoot = mod.ProjectileType("BonePiercerProjectile");
            }

            return base.CanUseItem(player);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Bone, 125);
            recipe.AddIngredient(ItemID.Cobweb, 40);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}