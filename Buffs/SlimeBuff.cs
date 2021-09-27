using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Buffs
{
    public class SlimeBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Gelatin Physique");
            Description.SetDefault("Every first jump spawns projectiles and restores 2 health\n" +
                "Increased jump height");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
            canBeCleared = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<OvermorrowModPlayer>().slimeBuff = true;
        }
    }
}