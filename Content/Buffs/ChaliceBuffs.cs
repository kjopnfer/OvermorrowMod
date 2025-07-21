using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs
{
    public class ChaliceMana : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.AddManaRegenPerSecond(16);
        }
    }

    public class ChaliceHealth : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.AddHealthRegenPerSecond(16);
        }
    }
}