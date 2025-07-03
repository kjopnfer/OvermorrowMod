using OvermorrowMod.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs
{
    public class ShadowBrand : ModBuff
    {
        public static readonly int TagDamage = 5;

        public override string Texture => AssetDirectory.Buffs + Name;

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }
}