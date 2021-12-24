using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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

        public bool RunOnce = true;
        public Player Target;
        public NPC ParentNPC;
        public override string Texture => "OvermorrowMod/Textures/LaserWarning";
        public TrackingWarning() : base(230f, 3000f, 0f, new Color(88, 237, 128), "NPCs/Bosses/StormDrake/LaserWarning") { }
        public override bool CanHitPlayer(Player target) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override void AI()
        {
            if (RunOnce)
            {
                Target = Main.player[(int)projectile.ai[0]];
                projectile.ai[0] = 0;
                RunOnce = false;
            }

            laserColor = Color.Lerp(new Color(88, 237, 128), Color.White, (float)Math.Sin(projectile.ai[1] / 5));
            projectile.ai[1]++;

            float CalculateAccuracy = MathHelper.Lerp(0, 120f, Utils.Clamp(projectile.ai[1], 0, 230f) / 230f);

            int TimeDelay = Main.expertMode ? 30 : 60;
            if (projectile.ai[1] < MaxTime - TimeDelay)
            {
                projectile.velocity = projectile.DirectionTo(Target.Center + Target.velocity * CalculateAccuracy).ToRotation().ToRotationVector2();
            }
            //projectile.velocity = projectile.DirectionTo(Target.Center + Target.velocity * 120).ToRotation().ToRotationVector2();


            projectile.scale = MathHelper.Clamp((float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2, 0, 1) * 0.1f;
        }

        public override void Kill(int timeLeft)
        {
            // Pass back the final velocity to the boss
            ParentNPC.velocity = Vector2.Normalize(projectile.velocity) * 40;
            ((TreeBossP2)ParentNPC.modNPC).PortalLaunched = true;
            ((TreeBossP2)ParentNPC.modNPC).PortalRuns++;
        }
    }

    public class MeteorWarning : Deathray
    {

        public bool RunOnce = true;
        public Player Target;
        public NPC ParentNPC;
        public override string Texture => "OvermorrowMod/Textures/LaserWarning";
        public MeteorWarning() : base(230f, 6000f, 0f, new Color(88, 237, 128), "NPCs/Bosses/StormDrake/LaserWarning") { }
        public override bool CanHitPlayer(Player target) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override void AI()
        {
            if (RunOnce)
            {
                Target = Main.player[(int)projectile.ai[0]];
                projectile.ai[0] = 0;
                RunOnce = false;
            }

            laserColor = Color.Lerp(Main.DiscoColor, Color.White, (float)Math.Sin(projectile.ai[1] / 5));
            projectile.ai[1]++;

            float CalculateAccuracy = MathHelper.Lerp(0, 120f, Utils.Clamp(projectile.ai[1], 0, 230f) / 230f);

            int TimeDelay = 75;
            if (projectile.ai[1] < MaxTime - TimeDelay)
            {
                //projectile.velocity = projectile.DirectionTo(Target.Center + Target.velocity * CalculateAccuracy).ToRotation().ToRotationVector2();

                projectile.velocity = projectile.DirectionTo(Target.Center).ToRotation().ToRotationVector2();
            }

            LaserLength = TRay.CastLength(projectile.Center, projectile.velocity, 6000f);
            projectile.scale = MathHelper.Clamp((float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2, 0, 1) * 0.1f;
        }

        public override void Kill(int timeLeft)
        {
            // Pass back the final velocity to the boss
            if (((TreeBossP2)ParentNPC.modNPC).AbsorbedEnergies > 12)
            {
                ParentNPC.velocity = Vector2.Normalize(projectile.velocity) * 60;
            }
            else
            {
                ParentNPC.velocity = Vector2.Normalize(projectile.velocity) * 40;
            }
            //((TreeBossP2)ParentNPC.modNPC).PortalLaunched = true;
        }
    }

    public class ScytheWarning : Deathray
    {
        public override string Texture => "OvermorrowMod/NPCs/Bosses/TreeBoss/ScytheWarning";
        public ScytheWarning() : base(60 * 7, 10000f, 0f, Color.LightGreen, "NPCs/Bosses/TreeBoss/ScytheWarning") { }
        public override bool CanHitPlayer(Player target) => false;
        public override bool? CanHitNPC(NPC target) => false;

        // This doesn't work lol
        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindProjectiles.Add(index);
            drawCacheProjsBehindNPCs.Add(index);
        }

        public override void AI()
        {
            if (projectile.ai[1]++ > 60 * 6)
            {
                projectile.localAI[1]++;
            }

            laserColor = Color.Lerp(Color.LightGreen, Color.Transparent, Utils.Clamp(projectile.localAI[1], 0, 60f) / 60);
        }
    }
}