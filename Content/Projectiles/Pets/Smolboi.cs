using OvermorrowMod.Common;
using OvermorrowMod.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Projectiles.Pets
{
    public class Smolboi : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Smolboi");
            Main.projFrames[Projectile.type] = 3;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Penguin);
            AIType = ProjectileID.Penguin;
            Projectile.width = 22;
            Projectile.height = 46;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            player.penguin = false; // Relic from aiType
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            OvermorrowModPlayer modPlayer = player.GetModPlayer<OvermorrowModPlayer>();
            if (player.dead)
            {
                modPlayer.smolBoi = false;
            }

            if (modPlayer.smolBoi)
            {
                Projectile.timeLeft = 2;
            }
        }
    }
}