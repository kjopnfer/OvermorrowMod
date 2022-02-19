using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Armor
{
    class Withdraw : ModBuff
    {
        public override void SetDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = true;
            DisplayName.SetDefault("Withdraw");
            Description.SetDefault("30% less damage, You feel more painful");
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.allDamage -= 0.20f;
        }
    }
}
