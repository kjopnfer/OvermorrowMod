using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Artifact;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.WardenClass.Weapons.Artifacts
{
    public class EarthCrystal : Artifact
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Earthen Crystal");
            Tooltip.SetDefault("[c/00FF00:{ Artifact }]\nConsume 3 Soul Essences to summon an ancient tree\n" +
                "All players within range have their health regen increased\n" +
                "'A crystal that once grew on the World Tree that had long since disappeared'");
        }

        public override void SafeSetDefaults()
        {
            item.width = 28;
            item.height = 44;
            item.rare = ItemRarityID.Orange;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.UseSound = SoundID.Item25;
            item.consumable = false;
            item.autoReuse = false;
            item.shoot = ModContent.ProjectileType<WorldTree>();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // Not sure why this isn't running in UseItem
            ConsumeSouls(3, player);

            // Allow only one instance of the projectile
            if (player.ownedProjectileCounts[ModContent.ProjectileType<RedCloud>()] > 0 || player.ownedProjectileCounts[ModContent.ProjectileType<WorldTree>()] > 0)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI &&
                        (Main.projectile[i].type == ModContent.ProjectileType<RedCloud>() || Main.projectile[i].type == ModContent.ProjectileType<WorldTree>()))
                    {
                        Main.projectile[i].Kill();
                    }
                }
                position = Main.MouseWorld;
            }
            else
            {
                position = Main.MouseWorld;
            }

            return true;
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
            ConsumeSouls(3, player);

            return true;
        }
    }
}
