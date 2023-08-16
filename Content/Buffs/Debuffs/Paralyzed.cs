using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs.Debuffs
{
    public class Paralyzed : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Paralyzed");
            // Description.SetDefault("You can't move!");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.velocity.X = 0;
        }
    }
}