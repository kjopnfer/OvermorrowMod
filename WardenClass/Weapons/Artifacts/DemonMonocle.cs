using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Artifact;
using OvermorrowMod.Projectiles.Boss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.WardenClass.Weapons.Artifacts
{
    public class DemonMonocle : Artifact
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demonic Monocle");
            Tooltip.SetDefault("[c/00FF00:{ Artifact }]\nConsume 2 Soul Essences to summon 6 Demon Eyes\n" +
                "Demon eyes will home in on nearby enemies\n" +
                "'Why do Demons wear monocles? I don't know either'");
        }

        public override void SafeSetDefaults()
        {
            item.width = 32;
            item.height = 30;
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
            recipe.AddIngredient(ItemID.LeadBar, 12);
            recipe.AddIngredient(ItemID.Lens, 6);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.IronBar, 12);
            recipe.AddIngredient(ItemID.Lens, 6);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}