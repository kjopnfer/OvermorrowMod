using OvermorrowMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs.Summon
{
    public class SkullBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Skull");
            Description.SetDefault("The skull will shoot for you while using a summon stick");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SkeletronSumm>()] > 0)
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
