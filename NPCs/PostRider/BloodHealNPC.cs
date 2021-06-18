
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.PostRider
{
    public class BloodHealNPC : ModProjectile
    {

        int timer = 0;

        public override void SetDefaults()
        {
            projectile.CloneDefaults(307);
            projectile.width = 6;
            projectile.height = 13;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300; //The amount of time the projectile is alive for
        }
        public override void AI()
        {

            projectile.alpha = 255;
            timer++;
            if (timer == 2)
            {
                Vector2 value1 = new Vector2(0f, 0f);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X / 7, projectile.velocity.Y / 7, mod.ProjectileType("BloodTrail"), 0, 1f, projectile.owner, 0f);
                timer = 0;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
                target.life = target.life + 150;
                target.AddBuff(BuffID.Lovestruck, 60);
                CombatText.NewText(target.getRect(), Color.Green, "150");
        }
    }
}
