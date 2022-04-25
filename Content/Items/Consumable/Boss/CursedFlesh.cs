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
            Item.width = 28;
            Item.height = 36;
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
            return !NPC.AnyNPCs(ModContent.NPCType<DripplerBoss>()) && Main.bloodMoon;
        }

        public override bool? UseItem(Player player)
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
                Projectile.NewProjectile(null, player.Center + (Vector2.UnitY * -300), Vector2.Zero, ModContent.ProjectileType<DriplordAnim>(), 0, 0, Main.myPlayer, 0, 0);
                return true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MutatedFlesh>(9)
                .AddIngredient(ItemID.VilePowder, 15)
                .AddTile(TileID.DemonAltar)
                .Register();

            CreateRecipe()
                .AddIngredient<MutatedFlesh>(9)
                .AddIngredient(ItemID.ViciousPowder, 15)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}