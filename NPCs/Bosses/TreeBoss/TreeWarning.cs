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

    public class TrackingWarning : Deathray
    {

        public bool RunOnce;
        public Player Target;
        public override string Texture => "OvermorrowMod/Textures/LaserWarning";
        public TrackingWarning() : base(230f, 3000f, 0f, Main.DiscoColor, "NPCs/Bosses/StormDrake/LaserWarning") { }
        public override bool CanHitPlayer(Player target) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override void AI()
        {
            if (RunOnce)
            {
                Target = Main.player[(int)projectile.ai[0]];
                projectile.ai[0] = 0;
            }

            laserColor = Color.Lerp(Main.DiscoColor, Color.White, (float)Math.Sin(projectile.ai[1] / 5));
            projectile.ai[1]++;

            //Player target = Main.player[(int)projectile.ai[0]];

            //projectile.rotation = projectile.DirectionTo(Main.player[parentNPC.target].Center + Main.player[parentNPC.target].velocity * 15f).ToRotation() + MathHelper.ToRadians(135f);
            /*if (Target.active)
            {
                projectile.ai[0] = projectile.DirectionTo(Target.Center).ToRotation();
            }*/

            //rotation =
            projectile.velocity = projectile.DirectionTo(Target.Center).ToRotation().ToRotationVector2();

            projectile.scale = MathHelper.Clamp((float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2, 0, 1) * 0.1f;
        }
    }
}