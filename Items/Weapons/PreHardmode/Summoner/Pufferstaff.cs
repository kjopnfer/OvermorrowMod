using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Buffs.Summon;
using OvermorrowMod.Projectiles.Summon;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Summoner
{
    public class Pufferstaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pufferfish Staff");
            Tooltip.SetDefault("Summons a Pufferfish Still to fight for you \nTo make it fire use the summon stick \nTakes a sentry slot");
        }
        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Green;
            item.width = 32;
            item.height = 32;
            item.damage = 12;
            item.UseSound = SoundID.Item82;
            item.summon = true;
            item.noMelee = true;
            item.sentry = true;
            item.useTime = 40;
            item.useAnimation = 40;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.autoReuse = false;
            item.buffType = ModContent.BuffType<PufferBuff>();
            item.knockBack = 0;
            item.shoot = ModContent.ProjectileType<PufferFish>();
            item.shootSpeed = 0f;
        }
        public override bool CanUseItem(Player player)
        {
            // Ensures no more than one spear can be thrown out, use this when using autoReuse
            return player.ownedProjectileCounts[item.shoot] < 1;
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
