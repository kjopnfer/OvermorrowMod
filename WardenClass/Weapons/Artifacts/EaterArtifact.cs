using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Weapons.Artifacts
{
    public class EaterArtifact : Artifact
    {
        private int consumedSouls = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Maw of the Eater");
            Tooltip.SetDefault("[c/00FF00:{ Artifact }]\nUse to consume all your Soul Essences, \n" +
                "Each Soul Essence consumed heals for 10 life each" +
                "\n'Like a big dream catcher that eats your face when you sleep'");
        }

        public override void SafeSetDefaults()
        {
            item.width = 42;
            item.height = 42;
            item.rare = ItemRarityID.Green;
            item.useAnimation = 60;
            item.useTime = 60;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.UseSound = SoundID.Item103;
            item.consumable = false;
            item.autoReuse = false;
        }

        public override bool CanUseItem(Player player)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            if(modPlayer.soulResourceCurrent > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool UseItem(Player player)
        {
            //var modPlayer = WardenDamagePlayer.ModPlayer(player);
            //ConsumeSouls(modPlayer.soulResourceCurrent, player);

            // Doing it manually is less janky, I guess
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            int soulCount = modPlayer.soulResourceCurrent;
            for (int i = 0; i < soulCount; i++)
            {
                // Get the instance of the first projectile in the list
                int removeProjectile = modPlayer.soulList[0];

                // Remove the projectile from the list
                modPlayer.soulList.RemoveAt(0);
                modPlayer.soulResourceCurrent--;

                // Call the projectile's method to kill itself
                for (int j = 0; j < Main.maxProjectiles; j++) // Loop through the projectile array
                {
                    // Check that the projectile is the same as the removed projectile and it is active
                    if (Main.projectile[j] == Main.projectile[removeProjectile] && Main.projectile[j].active)
                    {
                        // Kill the projectile
                        Main.projectile[j].Kill();
                        //player.statLife += 10;
                        //player.HealEffect(10);
                        consumedSouls++;
                    }
                }
            }
            player.statLife += 10 * consumedSouls;
            player.HealEffect(10 * consumedSouls);
            consumedSouls = 0;

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DemoniteBar, 12);
            recipe.AddIngredient(ItemID.ShadowScale, 10);
            recipe.AddIngredient(ItemID.WormTooth, 6);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}