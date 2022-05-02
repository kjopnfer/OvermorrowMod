using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Summoner.TankRemote
{
    public class Tankrocket : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tank Rocket");
        }

        public override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.width = 16;
            Projectile.height = 12;
            Projectile.penetrate = 1;
            Projectile.aiStyle = 34;
            Projectile.timeLeft = 120; //The amount of time the Projectile is alive for
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 1;
        }

        public override void Kill(int timeLeft)
        {
            int explode = Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y, 0, 0, ProjectileID.RocketSnowmanI, Projectile.damage, 3f, Projectile.owner, 0f);
            Main.projectile[explode].timeLeft = 0;
            Main.projectile[explode].friendly = true;
            Main.projectile[explode].hostile = false;
        }
    }
}
