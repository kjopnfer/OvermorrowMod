using OvermorrowMod.Content.Items.Weapons.Summoner.PufferStaff;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs.Summon
{
    public class PufferBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Pufferfish");
            Description.SetDefault("The pufferfish will shoot for you while using a summon stick");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<PufferFish>()] > 0)
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
