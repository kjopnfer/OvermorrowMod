using OvermorrowMod.Projectiles.Accessory;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Buffs.Summon
{
    class GraniteShieldBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Granite Shield");
            Description.SetDefault("A Granite Shield will protect you");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GraniteShield>()] > 0)
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