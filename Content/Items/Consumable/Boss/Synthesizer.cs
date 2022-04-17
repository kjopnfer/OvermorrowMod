using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Content.NPCs.Bosses.SandstormBoss;
using OvermorrowMod.Content.WorldGeneration;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Consumable.Boss
{
    public class Synthesizer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Synthesizer of the Sands");
            Tooltip.SetDefault("Summons Dharuud, the Sandstorm\nCan only be used within the shifting sands of an arid wasteland\n'What's the song name?'");
        }

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 34;
            Item.rare = ItemRarityID.Green;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.maxStack = 20;
            Item.noMelee = true;
            Item.consumable = false;
            Item.autoReuse = false;
        }

        public override bool CanUseItem(Player player)
        {
            // Make sure that the boss doesn't already exist and player is in correct zone
            return !NPC.AnyNPCs(ModContent.NPCType<SandstormBoss>()) && player.ZoneDesert;
        }

        public override bool? UseItem(Player player)
        {
            if (player.ZoneDesert)
            {
                // Arena spawn offset since it isn't precise grahhh
                // First vector spawns it onto the tile position, second vector shifts it back halfway
                Vector2 SpawnOffset = new Vector2(1 * 16, 2 * 16) - new Vector2(8, -8);
                Projectile.NewProjectile(null, Desert.DesertArenaCenter + SpawnOffset, Vector2.Zero, ModContent.ProjectileType<DharuudArena>(), 0, 0, Main.myPlayer, 0, 0);

                return true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Amber, 2)
                .AddRecipeGroup("IronBar", 10)
                .AddIngredient(ItemID.SandBlock, 25)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
