using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Summon;
using OvermorrowMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Summoner
{
    public class DrakeStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Whelp Staff");
            Tooltip.SetDefault("Summons a Storm Whelp to fight for you\nRequires at least 2 minion slots");
            ItemID.Sets.GamepadWholeScreenUseRange[item.type] = true; // This lets the player target anywhere on the whole screen while using a controller.
            ItemID.Sets.LockOnIgnoresCollision[item.type] = true;
            ItemID.Sets.StaffMinionSlotsRequired[item.type] = 2;
        }

        public override void SetDefaults()
        {
            item.width = 50;
            item.height = 50;
            item.damage = 23;
            item.mana = 10;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.rare = ItemRarityID.Orange;
            item.noMelee = true;
            item.summon = true;
            item.buffType = ModContent.BuffType<DrakeBuff>();
            item.shoot = ModContent.ProjectileType<StormWhelp>();
            item.UseSound = SoundID.Item82;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            player.AddBuff(item.buffType, 2);

            // Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.
            position = Main.MouseWorld;
            return true;
        }
    }
}