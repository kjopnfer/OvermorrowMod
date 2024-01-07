using Terraria;
using Terraria.ModLoader;
using System;

namespace OvermorrowMod.Content.Buffs
{
    public class GlimsporeStomperBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<Items.Misc.StrangeSpore.GlimsporeStomper>(), player);
            player.buffTime[buffIndex] = 10;
        }
    }
}