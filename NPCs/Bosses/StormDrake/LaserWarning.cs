using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.NPCs.Bosses.StormDrake
{
    public class LaserWarning : Deathray
    {
        public int waittime;
        public static int wait;
        public bool killearly = false;
        public bool killnow = false;
        
        public LaserWarning()  : base(300 + wait, 1500f, 0f, Color.Blue, "NPCs/Bosses/StormDrake/LaserWarning") { }
        public override bool CanHitPlayer(Player target) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override void AI()
        {
            wait =  killearly ? waittime * 10 : waittime;
            if (killnow == true)
            {
                projectile.active = false;
                projectile.timeLeft = 0;
            }
            float length = (TRay.Cast(projectile.Center, projectile.velocity, 1500f) - projectile.Center).Length();
            LaserLength = length;
            projectile.scale = MathHelper.Clamp((float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2, 0, 1) * 0.1f;
        }
        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<TestLightning>(), projectile.damage, projectile.knockBack, projectile.owner);
        }
    }
}