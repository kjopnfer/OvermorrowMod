using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace OvermorrowMod.Content.Items.Consumable.Boss
{
    public class DriplordAnimDrips : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Content/NPCs/Bosses/DripplerBoss/Driplad";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Small Drips");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.timeLeft = 1800;
            Projectile.penetrate = -1;
            //Projectile.alpha = 255;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Projectile owner = Main.projectile[(int)Projectile.ai[1]];

            if (Projectile.Distance(owner.Center) < 25)
            {
                ((DriplordAnim)Main.projectile[(int)Projectile.ai[1]].ModProjectile).dripsdead += 1; //true;
                ((DriplordAnim)Main.projectile[(int)Projectile.ai[1]].ModProjectile).scale += 0.03f; //true;
                Projectile.Kill();
            }
        }
    }
}
