using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.TreeBoss
{
    public class TreeWarning : Deathray
    {
        public int WaitTime;
        public static int Wait;
        public bool KillEarly = false;
        public bool KillNow = false;
        public override string Texture => "OvermorrowMod/Textures/LaserWarning";
        public TreeWarning() : base(60, 3000f, 0f, Main.DiscoColor, "NPCs/Bosses/StormDrake/LaserWarning") { }
        public override bool CanHitPlayer(Player target) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override void AI()
        {
            laserColor = Color.Lerp(Main.DiscoColor, Color.White, (float)Math.Sin(projectile.ai[1] / 5));
            projectile.ai[1]++;

            Wait = KillEarly ? WaitTime * 10 : WaitTime;
            if (KillNow == true)
            {
                projectile.active = false;
                projectile.timeLeft = 0;
            }
  
            projectile.scale = MathHelper.Clamp((float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2, 0, 1) * 0.1f;
            LaserLength = TRay.CastLength(projectile.Center, projectile.velocity, 3000f);
        }
    }
}