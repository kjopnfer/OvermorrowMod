using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.StormDrake
{
    public class LaserWarning : Deathray
    {
        public int waittime;
        public static int wait;
        public bool killearly = false;
        public bool killnow = false;

        public override string Texture => "OvermorrowMod/Content/NPCs/Bosses/StormDrake/LaserWarning";

        public LaserWarning() : base(300 + wait, 3000f, 0f, Color.Blue, "OvermorrowMod/Content/NPCs/Bosses/StormDrake/LaserWarning") { }
        public override bool CanHitPlayer(Player target) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override void AI()
        {
            laserColor = Color.Lerp(Color.Cyan, Color.White, (float)Math.Sin(Projectile.ai[1] / 5));
            Projectile.ai[1]++;

            wait = killearly ? waittime * 10 : waittime;
            if (killnow == true)
            {
                Projectile.active = false;
                Projectile.timeLeft = 0;
            }
            /*float length = (TRay.Cast(projectile.Center, projectile.velocity, 1250f) - projectile.Center).Length();
            LaserLength = length;*/
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2, 0, 1) * 0.1f;
        }
        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<TestLightning>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }

    public class LaserWarning2 : Deathray
    {
        public int waittime;
        public static int wait;
        public bool killearly = false;
        public bool killnow = false;

        public LaserWarning2() : base(50, 3000f, 0f, Color.Blue, "OvermorrowMod/Content/NPCs/Bosses/StormDrake/LaserWarning") { }
        public override bool CanHitPlayer(Player target) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override void AI()
        {
            /*float length = (TRay.Cast(projectile.Center, projectile.velocity, /*2000f*/ /*2500f 3000f) - projectile.Center).Length();
            LaserLength = length;*/
            laserColor = Color.Lerp(Color.Cyan, Color.White, (float)Math.Sin(Projectile.ai[1] / 5));
            Projectile.ai[1]++;

            Projectile.scale = MathHelper.Clamp((float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2, 0, 1) * 0.1f;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                float distance = Vector2.Distance(Projectile.Center, Main.player[i].Center);
                if (distance <= 1600)
                {
                    //Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 20;
                }
            }
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<TestLightning>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }

    public class LaserBreathWarning : Deathray
    {
        public float RotateBy;
        private float flashTimer = 0;
        public int Direction = 1;
        public override string Texture => "OvermorrowMod/Content/NPCs/Bosses/StormDrake/LaserWarning";
        public LaserBreathWarning() : base(180, 3000f, 0f, Color.Blue, "OvermorrowMod/Content/NPCs/Bosses/StormDrake/LaserWarning") { }
        public override bool CanHitPlayer(Player target) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override void AI()
        {
            laserColor = Color.Lerp(Color.Cyan, Color.White, (float)Math.Sin(flashTimer / 5));
            flashTimer++;

            Projectile.scale = MathHelper.Clamp((float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2, 0, 1) * 0.1f;
            NPC projectileowner = Main.npc[(int)Projectile.ai[1]];
            Projectile.position = projectileowner.Center + new Vector2(187 * Direction, -50);
            Projectile.velocity = (Vector2.UnitX * Direction).RotatedBy(MathHelper.ToRadians((Direction == 1) ? 315 + RotateBy : 45 + -RotateBy));
        }
    }

    public class LightningPhaseChangeWarning : Deathray
    {
        public float RotateBy;
        public override string Texture => "OvermorrowMod/Content/NPCs/Bosses/StormDrake/LaserWarning";
        public LightningPhaseChangeWarning() : base(120, 3000f, 0f, Color.Blue, "OvermorrowMod/Content/NPCs/Bosses/StormDrake/LaserWarning") { }
        public override bool CanHitPlayer(Player target) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override void AI()
        {
            laserColor = Color.Lerp(Color.Cyan, Color.White, (float)Math.Sin(Projectile.ai[1] / 5));
            Projectile.ai[1]++;

            Projectile.scale = MathHelper.Clamp((float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2, 0, 1) * 0.1f;
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(RotateBy));
        }
        public override void Kill(int timeLeft)
        {
            int proj = Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<TestLightning5>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            ((TestLightning5)Main.projectile[proj].ModProjectile).RotateBy = RotateBy;

        }
    }
}