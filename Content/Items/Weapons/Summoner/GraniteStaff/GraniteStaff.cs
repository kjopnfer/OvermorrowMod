using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Summoner.GraniteStaff
{
    public class GraniteStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Geomancer Staff");
            Tooltip.SetDefault("Summons Granite Elementals, hold down the mouse with this item while enemies are around to control them");
        }

        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 56;
            Item.damage = 18;
            Item.mana = 20;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Green;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<GraniteEleBuff>();
            Item.shoot = ModContent.ProjectileType<GraniteSummon>();
            Item.UseSound = SoundID.Item82;
            Item.channel = true;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600, true);
            }
        }
    }
}
