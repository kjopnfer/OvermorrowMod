using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Artifact;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Weapons.Artifacts
{
    public class DemonMonocle : Artifact
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Baleful Spine");
            Tooltip.SetDefault("[c/00FF00:{ Artifact }]\nConsume 2 Soul Essences to summon 6 Crimeras\n" +
                "Crimeras will home in on nearby enemies");
        }

        public override void SafeSetDefaults()
        {
            item.width = 26;
            item.height = 46;
            item.rare = ItemRarityID.Green;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.UseSound = SoundID.Item103;
            item.consumable = false;
            item.autoReuse = false;
            item.damage = 21;
        }

        public override bool CanUseItem(Player player)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            if (modPlayer.soulResourceCurrent >= 2)
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
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                ConsumeSouls(2, player);
            }

            int projectiles = 6;
            if (Main.netMode != NetmodeID.MultiplayerClient && Main.myPlayer == player.whoAmI)
            {
                for (int i = 0; i < projectiles; i++)
                {
                    Projectile.NewProjectile(player.Center, new Vector2(4).RotatedBy(MathHelper.ToRadians((360 / projectiles) * i + i)), ModContent.ProjectileType<DemonEye>(), item.damage, 2, player.whoAmI);
                }
            }

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CrimtaneBar, 12);
            recipe.AddIngredient(ItemID.TissueSample, 10);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}