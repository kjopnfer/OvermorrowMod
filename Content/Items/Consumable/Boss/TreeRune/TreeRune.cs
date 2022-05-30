using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Content.NPCs.Bosses.TreeBoss;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Consumable.Boss.TreeRune
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
            Item.width = 26;
            Item.height = 28;
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
            return !NPC.AnyNPCs(ModContent.NPCType<TreeBoss>()) && !NPC.AnyNPCs(ModContent.NPCType<TreeBossP2>()) && Main.dayTime;
        }

        public override bool? UseItem(Player player)
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
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Iorich, the Guardian has awoken!"), new Color(175, 75, 255));
                }

                NPC.NewNPC(null, (int)player.position.X, (int)(player.position.Y - 50f), ModContent.NPCType<TreeBoss>(), 0, 0f, 0f, 0f, 0f, 255);
                SoundEngine.PlaySound(SoundID.Roar, player.position);
                return true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PurificationPowder, 15)
                .AddIngredient(ItemID.Emerald, 1)
                .AddIngredient(ItemID.StoneBlock, 25)
                .AddIngredient(ItemID.Acorn, 10)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
