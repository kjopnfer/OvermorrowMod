using System;
using Terraria;
//using TestMod.Testing;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.NPCs.Bosses.StormDrake
{
    public class LaserBreathWarning : Deathray
    {
        //private int myprojectile;
        public int direction = 1;
        public override string Texture => "OvermorrowMod/NPCs/Bosses/StormDrake/LaserWarning";
        public LaserBreathWarning() : base(180 /*+ 360*/, 1000f, 0f, Color.Blue, "NPCs/Bosses/StormDrake/LaserWarning") { }
        public override bool CanHitPlayer(Player target) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override void AI()
        {
            //if (++projectile.ai[1] >= 180)
            //{
            //    if (projectile.ai[1] == 180)
            //    {
            //        /*int myprojectile*/
            //        myprojectile = Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<TestLightning2>(), projectile.damage, projectile.knockBack, projectile.owner, 0, projectile.ai[1]);
            //    }
            //    projectile.alpha = 255;
            //    projectile.scale = 0;
            //    ((TestLightning2)Main.projectile[myprojectile].modProjectile).direction = direction;
            //}
            //else
            //{
                float length = (TRay.Cast(projectile.Center, projectile.velocity, /*1000f*/ 2500f) - projectile.Center).Length();
                LaserLength = length;
                projectile.scale = MathHelper.Clamp((float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2, 0, 1) * 0.5f;//0.25f;
                NPC projectileowner = Main.npc[(int)projectile.ai[1]];
                projectile.position = projectileowner.Center + new Vector2(175 * direction, -49);
                projectile.velocity = Vector2.UnitX * direction;
            //}
        }
        public override void Kill(int timeLeft)
        {
            //int myprojectile = Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<TestLightning2>(), projectile.damage, projectile.knockBack, projectile.owner, 0, projectile.ai[1]);
            //((TestLightning2)Main.projectile[myprojectile].modProjectile).direction = direction;
            //((StormDrake2)Main.npc[(int)projectile.ai[1]].modNPC).LaserWarningDead = true;
            //((StormDrake2)Main.npc[(int)projectile.ai[1]].modNPC).MyProjectileStore = myprojectile;
        }
    }
}