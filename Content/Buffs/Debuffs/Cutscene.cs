using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs.Debuffs
{
    public class Cutscene : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Awestruck");
            Description.SetDefault("Watch closely!");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.controlLeft = false;
            player.controlRight = false;
            player.controlJump = false;
            player.controlDown = false;
            player.controlUseItem = false;
            player.controlUseTile = false;
            player.controlHook = false;
            player.controlMount = false;
            player.velocity = player.oldVelocity;
            player.position = player.oldPosition;
        }
    }
}