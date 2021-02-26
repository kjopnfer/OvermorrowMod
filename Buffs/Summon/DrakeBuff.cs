using OvermorrowMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Buffs.Summon
{
    public class DrakeBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Storm Whelp");
            Description.SetDefault("The Storm Whelp will fight for you");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<StormWhelp>()] > 0)
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