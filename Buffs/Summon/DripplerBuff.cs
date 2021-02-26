using OvermorrowMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Buffs.Summon
{
    public class DripplerBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Looming Drippler");
            Description.SetDefault("A Looming Drippler will protect you");
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