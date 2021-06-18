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
            Tooltip.SetDefault("Summons Granite Elementals to dive-bomb your enemies");
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
            item.buffType = mod.BuffType("ProbeBuff");
            item.shoot = mod.ProjectileType("ProbePROJ");
            item.UseSound = SoundID.Item82;
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
