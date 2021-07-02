using System;
using Microsoft.Xna.Framework;
using OvermorrowMod.Items.Materials;
using OvermorrowMod.NPCs.Bosses.Apollus;
using OvermorrowMod.NPCs.Bosses.GraniteMini;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Consumable.Boss
{
    public class MBBS : ModItem
    {
        public override string Texture => "OvermorrowMod/Items/Consumable/Boss/CursedFlesh";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Super Biome Bros Summoner");
            Tooltip.SetDefault(/*"Summons the Mini Boss Biome Brothers, the conquerers of Marble and Granite'"*/"Summons the Super Biome Bros, The conquerers of Marble and Granite'");
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
            return !NPC.AnyNPCs(ModContent.NPCType<ApollusBoss>()) && !NPC.AnyNPCs(ModContent.NPCType<AngryStone>());
        }

        public override bool UseItem(Player player)
        {
            //player.GetModPlayer<OvermorrowModPlayer>().TitleID = 5;
            //player.GetModPlayer<OvermorrowModPlayer>().FocusBoss = false;
            //player.GetModPlayer<OvermorrowModPlayer>().ShowText = true;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                //if (Main.netMode == NetmodeID.SinglePlayer)
                //{
                //    Main.NewText(/*"Gra-knight and Apollus have awoken!"*//*"The Super Biome Bros have awoken!"*/"The Super Stoner Bros have awoken!", 175, 75, 255);
                //}
                //else if (Main.netMode == NetmodeID.Server)
                //{
                //    NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("The Super Stoner Bros have awoken!"), new Color(175, 75, 255));
                //}

                //NPC.NewNPC((int)player.position.X + 250, (int)(player.position.Y - 450f), ModContent.NPCType<ApollusBoss>(), 0, -1f, 0f, 0f, 0f, 255);
                //NPC.NewNPC((int)player.position.X - 250, (int)(player.position.Y - 450f), ModContent.NPCType<AngryStone>(), 0, -3f, 0f, 0f, 0f, 255);
                Projectile.NewProjectile(new Vector2((int)player.position.X + 250, (int)(player.position.Y - 250f)), Vector2.Zero, ModContent.ProjectileType<SSBAnim>(), 0, 0, Main.myPlayer, 0, 900);
                Projectile.NewProjectile(new Vector2((int)player.position.X - 250, (int)(player.position.Y - 250f)), Vector2.Zero, ModContent.ProjectileType<SSBAnim>(), 0, 1, Main.myPlayer, 0, 900);
                Main.PlaySound(SoundID.Roar, player.position, 0);
                return true;
            }
            return false;
        }
    }
}
