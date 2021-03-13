using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs.Summon;
using OvermorrowMod.Projectiles.Summon;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Summoner
{
    public class IorichWand : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Renewal Wand of Iorich");
            Tooltip.SetDefault("Summons a miniature Ent to impale your enemies");
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 42;
            item.damage = 25;
            item.mana = 20;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.rare = ItemRarityID.Orange;
            item.noMelee = true;
            item.summon = true;
            item.sentry = true;
            item.shoot = ModContent.ProjectileType<MiniTreeBoss>();
            item.UseSound = SoundID.Item82;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.
            position = Main.MouseWorld;
            return true;
        }
    }
}