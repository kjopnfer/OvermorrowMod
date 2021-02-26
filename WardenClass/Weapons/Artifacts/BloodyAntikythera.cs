using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.WardenClass.Weapons.Artifacts
{
    public class BloodyAntikythera : ModItem
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloody Antikythera");
            Tooltip.SetDefault("[c/00FF00:{ Artifact }]\nConsume 3 Soul Essences to summon a miniature Blood Moon\n" +
                "All players within range have their attack and health regen increased\n" +
                "'Its gaze is the light of the Blood Moon'");
        }

        public override void SetDefaults()
        {
            item.width = 50;
            item.height = 50;
            item.rare = ItemRarityID.Orange;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.UseSound = SoundID.Item103;
            item.consumable = false;
            item.autoReuse = false;
        }

        public override bool CanUseItem(Player player)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            if (modPlayer.soulResourceCurrent >= 3)
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
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            for (int i = 0; i < 3; i++)
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
                    }
                }
            }
            return base.UseItem(player);
        }
    }
}