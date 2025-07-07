using OvermorrowMod.Common;
using OvermorrowMod.Content.Items.Archives.Accessories;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs
{
    public class BlackEchoes : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {

        }
    }
}