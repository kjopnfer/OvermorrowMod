using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Pets
{
    public class Smolboi : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Smolboi");
            Main.projFrames[projectile.type] = 3;
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Penguin);
            aiType = ProjectileID.Penguin;
            projectile.width = 22;
            projectile.height = 46;
        }

        public override bool PreAI()
        {
            Player player = Main.player[projectile.owner];
            player.penguin = false; // Relic from aiType
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            OvermorrowModPlayer modPlayer = player.GetModPlayer<OvermorrowModPlayer>();
            if (player.dead)
            {
                modPlayer.smolBoi = false;
            }

            if (modPlayer.smolBoi)
            {
                projectile.timeLeft = 2;
            }
        }
    }
}