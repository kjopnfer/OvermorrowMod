/*using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs.Summon
{
    public class IorichGuardian : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Guardian of Iorich");
            Description.SetDefault("A Guardian of Iorich will fight for you");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
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
}*/