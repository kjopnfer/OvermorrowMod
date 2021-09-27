using OvermorrowMod.Projectiles.Pets;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Buffs.Pets
{
    public class SmolboiBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Lil' Rune Merchant");
            Description.SetDefault("Here to bring you happiness");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.GetModPlayer<OvermorrowModPlayer>().smolBoi = true;
            bool petProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<Smolboi>()] <= 0;
            if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, ModContent.ProjectileType<Smolboi>(), 0, 0f, player.whoAmI, 0f, 0f);
            }
        }
    }
}