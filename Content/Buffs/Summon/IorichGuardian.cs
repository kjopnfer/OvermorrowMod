using OvermorrowMod.Content.Items.Weapons.Summoner.IorichWand;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs.Summon
{
    public class IorichGuardian : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Guardian of Iorich");
            Description.SetDefault("A Guardian of Iorich will fight for you");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<IorichSummon>()] > 0)
            {
                player.buffTime[buffIndex] = 1;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}