using OvermorrowMod.Content.Items.Weapons.Summoner.GraniteStaff;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs.Summon
{
    public class GraniteEleBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Granite Elemental");
            Description.SetDefault("The Granite Elemental will fight for you");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GraniteSummon>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}