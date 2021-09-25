using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.NPCs.Bosses.StormDrake
{
    public class LaserWarning2 : Deathray
    {
        public int waittime;
        public static int wait;
        public bool killearly = false;
        public bool killnow = false;

        public LaserWarning2() : base(50, 3000f, 0f, Color.Blue, "NPCs/Bosses/StormDrake/LaserWarning") { }
        public override bool CanHitPlayer(Player target) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override void AI()
        {
            /*float length = (TRay.Cast(projectile.Center, projectile.velocity, /*2000f*/ /*2500f 3000f) - projectile.Center).Length();
            LaserLength = length;*/
            laserColor = Color.Lerp(Color.Cyan, Color.White, (float)Math.Sin(projectile.ai[1] / 5));
            projectile.ai[1]++;

            projectile.scale = MathHelper.Clamp((float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2, 0, 1) * 0.1f;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                float distance = Vector2.Distance(projectile.Center, Main.player[i].Center);
                if (distance <= 1600)
                {
                    Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 20;
                }
            }
            Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<TestLightning>(), projectile.damage, projectile.knockBack, projectile.owner);
        }
    }
}
