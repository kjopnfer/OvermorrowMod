using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Buffs
{
	public class SlimeBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Gelatin Empowerment");
			Description.SetDefault("Increases jump height, jumping spawns gel spikes from the player");
			Main.buffNoSave[Type] = true;
			Main.debuff[Type] = false;
			canBeCleared = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<OvermorrowModPlayer>().slimeBuff = true;
		}
	}
}