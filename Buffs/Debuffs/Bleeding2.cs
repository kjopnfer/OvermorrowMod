using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Buffs.Debuffs
{
    public class Bleeding2 : ModBuff
    {
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Bleeding II");
			Description.SetDefault("Losing life");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = false;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<OvermorrowGlobalNPC>().bleedingDebuff2 = true;
		}
	}
}