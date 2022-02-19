using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace OvermorrowMod.Items.Consumable.Boss
{
    public class DriplordAnimDrips : ModProjectile
    {
        public override string Texture => "OvermorrowMod/NPCs/Bosses/DripplerBoss/Driplad";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Small Drips");
            Main.projFrames[projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.tileCollide = false;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.timeLeft = 1800;
            projectile.penetrate = -1;
            //projectile.alpha = 255;
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Projectile owner = Main.projectile[(int)projectile.ai[1]];

            if (projectile.Distance(owner.Center) < 25)
            {
                ((DriplordAnim)Main.projectile[(int)projectile.ai[1]].modProjectile).dripsdead++; //true;
                ((DriplordAnim)Main.projectile[(int)projectile.ai[1]].modProjectile).scale += 0.03f; //true;
                projectile.Kill();
            }
        }
    }
}
