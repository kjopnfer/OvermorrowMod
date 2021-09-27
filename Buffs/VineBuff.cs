using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Buffs
{
    public class VineBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Stingvine Tendrils");
            Description.SetDefault("Stingvines grow from your back, attacking nearby enemies");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
            canBeCleared = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<OvermorrowModPlayer>().vineBuff = true;
        }
    }
}