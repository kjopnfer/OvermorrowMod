using Microsoft.Xna.Framework;
using OvermorrowMod.NPCs.Bosses.TreeBoss;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Consumable.Boss
{
    public class TreeRune : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mossy Runestone");
            Tooltip.SetDefault("Summons Iorich, the Guardian\nCan only be used in the serene forests of the Overworld\n'Holding it gives you feelings of serenity'");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 28;
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
            return !NPC.AnyNPCs(ModContent.NPCType<TreeBoss>()) && !NPC.AnyNPCs(ModContent.NPCType<TreeBossP2>()) && Main.dayTime;
        }

        public override bool UseItem(Player player)
        {
            player.GetModPlayer<OvermorrowModPlayer>().TitleID = 4;
            player.GetModPlayer<OvermorrowModPlayer>().FocusBoss = true;
            player.GetModPlayer<OvermorrowModPlayer>().ShowText = true;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText("Iorich, the Guardian has awoken!", 175, 75, 255);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("Iorich, the Guardian has awoken!"), new Color(175, 75, 255));
                }

                NPC.NewNPC((int)player.position.X, (int)(player.position.Y - 50f), ModContent.NPCType<TreeBoss>(), 0, 0f, 0f, 0f, 0f, 255);
                Main.PlaySound(SoundID.Roar, player.position, 0);
                return true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PurificationPowder, 15);
            recipe.AddIngredient(ItemID.Emerald, 1);
            recipe.AddIngredient(ItemID.StoneBlock, 25);
            recipe.AddIngredient(ItemID.Acorn, 10);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
