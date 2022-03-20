using OvermorrowMod.Common;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs
{
    public class ShieldBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Guardian's Protection");
            Description.SetDefault("Reduces damage taken by enemies (up to 50)");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<OvermorrowModPlayer>().iorichGuardianShield = true;
        }
    }
}