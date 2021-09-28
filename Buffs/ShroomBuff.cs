using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Buffs
{
    public class ShroomBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Fungal Growth");
            Description.SetDefault("Mushrooms grow from your back, firing towards your cursor");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
            canBeCleared = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<OvermorrowModPlayer>().shroomBuff = true;
        }
    }
}