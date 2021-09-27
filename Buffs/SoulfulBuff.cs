using Terraria;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.Buffs
{
    public class SoulfulBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Soulful");
            Description.SetDefault("Increased Soul Gain rate by 4%");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
            canBeCleared = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<WardenDamagePlayer>().soulGainBonus += 4;
        }
    }
}