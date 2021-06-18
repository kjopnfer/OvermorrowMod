using OvermorrowMod.Items.Weapons.Hardmode.HardSummon;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.Hardmode.HardSummon
{
    public class EyeStillBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Eye of Staffthu");
            Description.SetDefault("The eye will fight for you");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
    
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<EyeStill>()] > 0)
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
