using OvermorrowMod.Common;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs
{
    public class Steal : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Steal!");
            Description.SetDefault("You've taken something from the enemy, better put it to good use...");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<OvermorrowModPlayer>().StoleArtifact = true;
        }
    }
}