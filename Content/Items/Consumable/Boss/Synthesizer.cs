using Microsoft.Xna.Framework;
using OvermorrowMod.Content.NPCs.Bosses.SandstormBoss;
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
            item.width = 48;
            item.height = 34;
            item.rare = ItemRarityID.Green;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.maxStack = 20;
            item.noMelee = true;
            item.consumable = false;
            item.autoReuse = false;
        }

        public override bool CanUseItem(Player player)
        {
            // Make sure that the boss doesn't already exist and player is in correct zone
            return !NPC.AnyNPCs(ModContent.NPCType<SandstormBoss>()) && player.ZoneDesert;
        }

        public override bool UseItem(Player player)
        {
            if (player.ZoneDesert)
            {
                Vector2 SpawnPosition = new Vector2(player.Center.X, player.Bottom.Y) - Vector2.UnitY * 224;
                Projectile.NewProjectile(SpawnPosition, Vector2.Zero, ModContent.ProjectileType<DharuudArena>(), 0, 0, Main.myPlayer, 0, 0);
                return true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Amber, 2);
            recipe.AddRecipeGroup("IronBar", 10);
            recipe.AddIngredient(ItemID.SandBlock, 25);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
