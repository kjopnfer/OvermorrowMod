using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Buffs.Summon;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Projectiles.Magic
{
    public class CircleMagic : ModProjectile
    {

        int colorcooldown = 0;
        readonly int frame = 1;
        Vector2 Rot;
        int Random2 = Main.rand.Next(-15, 12);
        int Random = Main.rand.Next(1, 3);
        private readonly int timer2 = 0;
        private int timer = 0;
        private float CircleArr = 1;
        private int PosCheck = 0;
        private int PosPlay = 0;
        private int PosTimer = 0;
        private int Pos = 0;
        private int movement = 0;


        private int OrignalDamage = 0;


        private int HasChecked = 0;

        private int NumProj = 0;
        private int movement2 = 0;
        float NPCtargetX = 0;
        float NPCtargetY = 0;
        int mrand = Main.rand.Next(-100, 101);
        int mrand2 = Main.rand.Next(-100, 101);

        private int CenterXPly = 5;

        private bool charge = false;


        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 18;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.netImportant = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 2;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Friendly Meatball");
        }

        public override void AI()
        {
            projectile.timeLeft = projectile.timeLeft + 1;
            float distanceFromTarget = 300f;
            Vector2 targetCenter = projectile.position;
            bool foundTarget = false;
            projectile.rotation = (Main.MouseWorld - projectile.Center).ToRotation();
            projectile.tileCollide = false;

            if(Main.player[projectile.owner].channel && !charge)
            {

            NumProj = Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<CircleMagic>()];
            PosCheck++;
            if(PosCheck == 1)
            {
                PosPlay = NumProj;
                OrignalDamage = projectile.damage;
            }
            if(PosCheck == 2)
            {
                HasChecked = PosPlay;
            }


            if(PosCheck == 2)
            {
                CircleArr = NumProj * 20;
            }

            if(PosCheck > 2)
            {
                projectile.damage = OrignalDamage * NumProj;
                double deg = (double)CircleArr; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                double rad = deg * (Math.PI / 180); //Convert degrees to radian
                    
                projectile.position.X = 50 * (float)Math.Cos(rad) + Main.player[projectile.owner].Center.X - projectile.width / 2;
                projectile.position.Y = 50 * (float)Math.Sin(rad) + Main.player[projectile.owner].Center.Y - projectile.height / 2;


                CircleArr += 1.7f;
            }
            }
            else if(!charge)
            {
                if(PosCheck > 2)
                {
                Vector2 position = projectile.Center;
                Vector2 targetPosition = Main.MouseWorld + projectile.position - Main.player[projectile.owner].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                float speed = 11;
                Main.PlaySound(2, projectile.position, 8);
                charge = true;
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, direction.X * speed, direction.Y * speed, ModContent.ProjectileType<ShotSpike>(), projectile.damage, 0f, projectile.owner, 0f);
                }
                projectile.Kill();
            }

            if(PosPlay > 9 && PosCheck < 30)
            {
                projectile.Kill();
            }
        }
    }
}
