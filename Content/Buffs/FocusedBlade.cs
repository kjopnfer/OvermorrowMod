using OvermorrowMod.Common;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs
{
    public class FocusedBlade : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }

        // Actual effects are handled in the Accessory
        public override void Update(Player player, ref int buffIndex)
        {
        }
    }
}