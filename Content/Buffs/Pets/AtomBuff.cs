using OvermorrowMod.Common.Players;
using OvermorrowMod.Content.Projectiles.Pets;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs.Pets
{
    public class AtomBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Atomic Heart");
            // Description.SetDefault("'Atomguy? Atomgirl? She's cute either way!'");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Main.lightPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.GetModPlayer<OvermorrowModPlayer>().atomBuff = true;
            bool petProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<Atom>()] <= 0;
            if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, ModContent.ProjectileType<Atom>(), 0, 0f, player.whoAmI, 0f, 0f);
            }
        }
    }
}