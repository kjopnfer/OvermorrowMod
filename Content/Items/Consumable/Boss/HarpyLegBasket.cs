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
            Item.width = 38;
            Item.height = 38;
            Item.rare = ItemRarityID.Green;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.maxStack = 20;
            Item.noMelee = true;
            Item.consumable = true;
            Item.autoReuse = false;
        }

        public override bool CanUseItem(Player player)
        {
            // Make sure that the boss doesn't already exist and player is in correct zone
            return !NPC.AnyNPCs(ModContent.NPCType</*StormDrake*/StormDrake>()) && player.ZoneSkyHeight;
        }

        public override bool? UseItem(Player player)
        {
            if (player.ZoneSkyHeight)
            {
                Projectile.NewProjectile(null, player.Center + Vector2.UnitY * -75, Vector2.Zero, ModContent.ProjectileType<StormDrakeAnim>(), 0, 0, Main.myPlayer, 0, 900);
                return true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HarpyLeg>(4)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}