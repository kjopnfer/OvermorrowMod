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
            item.consumable = true;
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
                Projectile.NewProjectile(player.Center + Vector2.UnitY * -5, Vector2.Zero, ModContent.ProjectileType<DharuudAnim>(), 0, 0, Main.myPlayer, 0, 0);
                //player.GetModPlayer<OvermorrowModPlayer>().TitleID = 1;
                //player.GetModPlayer<OvermorrowModPlayer>().FocusBoss = true;
                //player.GetModPlayer<OvermorrowModPlayer>().ShowText = true;

                //NPC.NewNPC((int)player.position.X, (int)(player.position.Y + 650f), ModContent.NPCType<SandstormBoss>(), 0, 0f, 0f, 0f, 0f, 255);
                //Main.PlaySound(SoundID.Roar, player.position, 0);
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
