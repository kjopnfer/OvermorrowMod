using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Content.NPCs.Bosses.StormDrake;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Consumable.Boss
{
    public class HarpyLegBasket : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Basket of Harpy Legs");
            Tooltip.SetDefault("Summons The Storm Drake of Oris\nCan only be used where Harpies roam the skies\n'Hey there, we both got baskets of Harpy Legs'");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 38;
            item.rare = ItemRarityID.Green;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.maxStack = 20;
            item.noMelee = true;
            item.consumable = true;
            item.autoReuse = false;
        }

        public override bool CanUseItem(Player player)
        {
            // Make sure that the boss doesn't already exist and player is in correct zone
            return !NPC.AnyNPCs(ModContent.NPCType</*StormDrake*/StormDrake>()) && player.ZoneSkyHeight;
        }

        public override bool UseItem(Player player)
        {
            if (player.ZoneSkyHeight)
            {
                Projectile.NewProjectile(player.Center + Vector2.UnitY * -75, Vector2.Zero, ModContent.ProjectileType<StormDrakeAnim>(), 0, 0, Main.myPlayer, 0, 900);
                return true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<HarpyLeg>(), 4);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}