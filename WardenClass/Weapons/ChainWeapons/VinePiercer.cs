using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Piercing;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Weapons.ChainWeapons
{
    public class VinePiercer : PiercingItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thorns of the Jungle");
            Tooltip.SetDefault("Launches chains that have a chance to poison\n[c/00FF00:{ Special Ability }]\n" +
                            "[c/800080:Right Click] to launch a chain that releases toxic gas on hit\nConsumes 1 Soul Essence");
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
            item.damage = 10;
            item.shootSpeed = 14f;
            item.shoot = mod.ProjectileType("VinePiercerProjectile");
            item.rare = ItemRarityID.Blue;
            item.UseSound = new LegacySoundStyle(SoundID.Grass, 0); // Grass
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
                item.shoot = mod.ProjectileType("VinePiercerProjectileAlt");
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
                item.shoot = mod.ProjectileType("VinePiercerProjectile");
            }

            return base.CanUseItem(player);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Stinger, 18);
            recipe.AddIngredient(ItemID.JungleSpores, 6);
            recipe.AddIngredient(ItemID.Vine, 12);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}