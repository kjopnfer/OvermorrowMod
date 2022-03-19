using OvermorrowMod.Content.Buffs.Summon;
using OvermorrowMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.PreHardmode.Summoner
{
    public class MeatballStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meatball Staff");
            Tooltip.SetDefault("Summons Meatballs to circle around you\nMeatballs will fire at nearby enemies\nOnly takes half a minion slot");
        }

        public override void SetDefaults()
        {
            item.width = 56;
            item.height = 56;
            item.damage = 16;
            item.mana = 20;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.rare = ItemRarityID.Green;
            item.noMelee = true;
            item.summon = true;
            item.buffType = ModContent.BuffType<MeatBallBuff>();
            item.shoot = ModContent.ProjectileType<FriendlyMeatball>();
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
