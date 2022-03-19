using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Content.NPCs.Bosses.DripplerBoss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Consumable.Boss
{
    public class CursedFlesh : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cursed Flesh");
            Tooltip.SetDefault("Summons Dripplord, The Bloody Assimilator\nCan only be used under the light of a foul moon\n'Make us whole...'");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 36;
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
            return !NPC.AnyNPCs(ModContent.NPCType<DripplerBoss>()) && Main.bloodMoon;
        }

        public override bool UseItem(Player player)
        {
            //player.GetModPlayer<OvermorrowModPlayer>().TitleID = 3;
            //player.GetModPlayer<OvermorrowModPlayer>().FocusBoss = true;
            //player.GetModPlayer<OvermorrowModPlayer>().ShowText = true;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                //if (Main.netMode == NetmodeID.SinglePlayer)
                //{
                //    Main.NewText("Dripplord, the Bloody Assimilator has awoken!", 175, 75, 255);
                //}
                //else if (Main.netMode == NetmodeID.Server)
                //{
                //    NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("Dripplord, the Bloody Assimilator has awoken!"), new Color(175, 75, 255));
                //}

                //NPC.NewNPC((int)player.position.X, (int)(player.position.Y - 450f), ModContent.NPCType<DripplerBoss>(), 0, 0f, 0f, 0f, 0f, 255);
                //Main.PlaySound(SoundID.Roar, player.position, 0);
                Projectile.NewProjectile(player.Center + (Vector2.UnitY * -300), Vector2.Zero, ModContent.ProjectileType<DriplordAnim>(), 0, 0, Main.myPlayer, 0, 0);
                return true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MutatedFlesh>(), 9);
            recipe.AddIngredient(ItemID.VilePowder, 15);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MutatedFlesh>(), 9);
            recipe.AddIngredient(ItemID.ViciousPowder, 15);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}