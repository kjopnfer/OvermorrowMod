using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Buffs
{
    public class WindBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Apollo's Favor");
            Description.SetDefault("Increases your speed & leaves a trail behind you while holding a ranged weapon");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<OvermorrowModPlayer>().windBuff = true;
        }
    }
}