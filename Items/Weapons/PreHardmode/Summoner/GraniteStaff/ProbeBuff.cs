
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Items.Weapons.PreHardmode.Summoner.GraniteStaff.Probe;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Summoner.GraniteStaff
{
    public class ProbeBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Probe");
            Description.SetDefault("The Probe will fight for you");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
    
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ProbePROJ>()] > 0)
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
