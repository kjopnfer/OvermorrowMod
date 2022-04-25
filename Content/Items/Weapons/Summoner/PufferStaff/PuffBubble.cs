using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Summoner.PufferStaff
{
    public class PuffBubble : ModProjectile
    {
        private bool release = false;

        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.FlaironBubble;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bubble");
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.timeLeft = 75;
            Projectile.penetrate = 1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            if (!Main.player[Projectile.owner].channel)
            {
                release = true;
            }
            if (release)
            {
                Projectile.velocity.Y -= 0.5f;
            }
            if (!release)
            {
                Projectile.velocity.X *= 0.90f;
                Projectile.velocity.Y *= 0.90f;
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item, Projectile.position, 54);
        }
    }
}
