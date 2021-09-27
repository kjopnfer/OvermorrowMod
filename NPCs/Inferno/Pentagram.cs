
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Inferno
{
    public class Pentagram : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.LightDisc);
            projectile.width = 50;
            projectile.height = 50;
            projectile.alpha = 0;
            projectile.timeLeft = 125;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        int timer = 0;

        public override void AI()
        {
            timer++;
            projectile.rotation = projectile.rotation + 1;
            if (timer == 1)
            {
                projectile.position.Y = Main.player[projectile.owner].Center.Y - 35f;
                projectile.friendly = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.damage > 1)
            {
                target.damage += 5;
                target.life = target.life + 15;
                target.AddBuff(BuffID.Lovestruck, 100);
                CombatText.NewText(target.getRect(), Color.Green, "+15 health +5 DMG");
            }
            else
            {
                target.life = target.life + 1;
            }
        }
    }
}