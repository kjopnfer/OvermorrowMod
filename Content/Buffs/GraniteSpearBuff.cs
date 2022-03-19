using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs
{
    public class GraniteSpearBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Granite Empowerment");
            Description.SetDefault("Minion damage increased by 10%");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<OvermorrowModPlayer>().graniteSpearBuff = true;
        }
    }
}