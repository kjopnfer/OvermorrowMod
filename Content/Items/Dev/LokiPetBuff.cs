using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Dev
{
    public class LokiPetBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Loki");
            Description.SetDefault("he");

            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;

            bool petProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<LokiPet>()] <= 0;
            if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.GetProjectileSource_Buff(buffIndex), player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, ModContent.ProjectileType<LokiPet>(), 0, 0f, player.whoAmI, 0f, 0f);
            }
        }
    }
}
