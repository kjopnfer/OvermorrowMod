using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Buffs
{
    public class PillarBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Osmanthian Fortitude");
            Description.SetDefault("Your defense is being increased\nYou are immune to knockback\n'Mwaaha, the Osmanthus Wine has always been celebrated for it's excellence'");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 6;
            player.noKnockback = true;
        }
    }
}