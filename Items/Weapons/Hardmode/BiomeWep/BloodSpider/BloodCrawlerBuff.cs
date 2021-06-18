using OvermorrowMod.Items.Weapons.Hardmode.BiomeWep.BloodSpider;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.Hardmode.BiomeWep.BloodSpider
{
    public class BloodCrawlerBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Blood Crawler");
            Description.SetDefault("The Blood Crawler will fight for you");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
    
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<BloodSumm>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
