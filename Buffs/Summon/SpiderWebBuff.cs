using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Buffs.Summon;
using OvermorrowMod.Projectiles.Accessory;
using OvermorrowMod.Projectiles.Summon;

namespace OvermorrowMod.Buffs.Summon
{
    class SpiderWebBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Granite Shield");
            Description.SetDefault("Granite Shield");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }


        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<StormWhelp>()] > 0)
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