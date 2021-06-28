using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs.Summon;
using OvermorrowMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Summoner.GraniteStaff
{
    public class GraniteStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Geomancer Staff");
            Tooltip.SetDefault("Summons Granite Elementals, hold down the mouse with this item while enemies are arround to control them");
        }

        public override void SetDefaults()
        {
            item.width = 56;
            item.height = 56;
            item.damage = 18;
            item.mana = 20;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.rare = ItemRarityID.Green;
            item.noMelee = true;
            item.summon = true;
            item.buffType = ModContent.BuffType<GraniteEleBuff>();
            item.shoot = ModContent.ProjectileType<GraniteSummon>();
            item.UseSound = SoundID.Item82;
            item.channel = true;
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }
    }
}
