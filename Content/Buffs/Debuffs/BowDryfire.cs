using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs.Debuffs
{
    public class BowDryfire : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dryfire Cooldown");
            Description.SetDefault("You were thinking too fast you forgot to load your bow, and ended up dryfiring it.");
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}