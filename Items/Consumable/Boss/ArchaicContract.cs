﻿using System;
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
    public class ArchaicContract : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Archaic Contract");
            Tooltip.SetDefault("Summons the Spirits of Marble and Granite\n'The words 'simp' seem to be inscribed within the scroll'");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
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
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(new Vector2((int)player.position.X + 250, (int)(player.position.Y - 250f)), Vector2.Zero, ModContent.ProjectileType<SSBAnim>(), 0, 0, Main.myPlayer, 0, 900);
                Projectile.NewProjectile(new Vector2((int)player.position.X - 250, (int)(player.position.Y - 250f)), Vector2.Zero, ModContent.ProjectileType<SSBAnim>(), 0, 1, Main.myPlayer, 0, 900);
                Main.PlaySound(SoundID.Roar, player.position, 0);
                return true;
            }
            return false;
        }
    }
}