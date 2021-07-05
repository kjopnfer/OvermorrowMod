//using System;
//using Terraria;
////using TestMod.Testing;
//using Terraria.ModLoader;
//using Microsoft.Xna.Framework;
//using OvermorrowMod.Projectiles.Boss;

//namespace OvermorrowMod.NPCs.Bosses.StormDrake
//{
//    public class LaserWarning : Deathray
//    {
//        public LaserWarning() : base(60f, 1000f, 0f, Color.Blue, /*"Projectiles/StormDrake/LaserWarning"*/ "OvermorrowMod/NPCs/Bosses/StormDrake/LaserWarning") {}
//        public override bool CanHitPlayer(Player target) => false;
//        public override bool? CanHitNPC(NPC target) => false;
//        public override void AI()
//        {
//            float length = (TRay.Cast(projectile.Center, projectile.velocity, 1000f) - projectile.Center).Length();
//            LaserLength = length;
//            projectile.scale = MathHelper.Clamp((float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2, 0, 1) * 0.1f;
//        }
//        public override void Kill(int timeLeft)
//        {
//            Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<TestLightning>(), projectile.damage, projectile.knockBack, projectile.owner);
//        }
//    }
//}

//using System;
//using Terraria;
////using TestMod.Testing;
//using Terraria.ModLoader;
//using Microsoft.Xna.Framework;
//using Terraria.ID;

//namespace OvermorrowMod.NPCs.Bosses.StormDrake
//{
//    public class LaserWarning : Deathray
//    {
//        public LaserWarning() : base(60f, 1000f, 0f, Color.Blue, /*"Projectiles/StormDrake/LaserWarning"*/ "OvermorrowMod/NPCs/Bosses/StormDrake/LaserWarning") { }
//        /*public override bool CanHitPlayer(Player target) => false;
//        public override bool? CanHitNPC(NPC target) => false;
//        public override void AI()
//        {
//            float length = (TRay.Cast(projectile.Center, projectile.velocity, 1000f) - projectile.Center).Length();
//            LaserLength = length;
//            projectile.scale = MathHelper.Clamp((float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2, 0, 1) * 0.1f;
//            projectile.timeLeft = 60;
//            Main.NewText("ayaya");
//        }
//        public override void Kill(int timeLeft)
//        {
//            Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<TestLightning>(), projectile.damage, projectile.knockBack, projectile.owner);
//        }*/
//    }
//}

using System;
using Terraria;
//using TestMod.Testing;
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
        
        public LaserWarning()  : base(300 + wait /*+ wait*/, /*1000f*/2500 + (wait / 10), 0f, Color.Blue, "NPCs/Bosses/StormDrake/LaserWarning") { }
        public override bool CanHitPlayer(Player target) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override void AI()
        {
            /*if(projectile.scale == 0)
            {
                //Main.NewText(300 - wait);
                Main.NewText(wait);
            }*/
            wait =  killearly ? waittime * 10 : waittime;
            if (killnow == true)
            {
                projectile.active = false;
                projectile.timeLeft = 0;
            }
            float length = (TRay.Cast(projectile.Center, projectile.velocity, /*1000f*/ 2500f) - projectile.Center).Length();
            LaserLength = length;
            projectile.scale = MathHelper.Clamp((float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2, 0, 1) * 0.1f;
        }
        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<TestLightning>(), projectile.damage, projectile.knockBack, projectile.owner);
        }
    }
}