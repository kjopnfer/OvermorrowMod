using OvermorrowMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs.Summon
{
    public class MeatBallBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Meatball");
            Description.SetDefault("The Meatball will fight for you");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FriendlyMeatball>()] > 0)
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