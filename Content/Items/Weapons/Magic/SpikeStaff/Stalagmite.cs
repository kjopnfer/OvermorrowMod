using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.SpikeStaff
{
    public class Stalagmite : ModProjectile
    {
        int Random2 = Main.rand.Next(-15, 12);
        int Random = Main.rand.Next(1, 3);
        private float CircleArr = 1;
        private int PosCheck = 0;
        private int PosPlay = 0;
        private int OrignalDamage = 0;


        private int HasChecked = 0;

        private int NumProj = 0;
        int mrand = Main.rand.Next(-100, 101);
        int mrand2 = Main.rand.Next(-100, 101);
        private bool charge = false;
        public override string Texture => AssetDirectory.Magic + "SpikeStaff/ShotSpike";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spikes");
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
        }


        public override void AI()
        {
            Projectile.timeLeft = Projectile.timeLeft + 1;
            Projectile.rotation = (Main.MouseWorld - Projectile.Center).ToRotation();
            Projectile.tileCollide = false;

            if (Main.player[Projectile.owner].channel && !charge)
            {

                NumProj = Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<Stalagmite>()];
                PosCheck++;
                if (PosCheck == 1)
                {
                    PosPlay = NumProj;
                    OrignalDamage = Projectile.damage;
                }
                if (PosCheck == 2)
                {
                    HasChecked = PosPlay;
                }


                if (PosCheck == 2)
                {
                    CircleArr = NumProj * 20;
                }

                if (PosCheck > 2)
                {
                    Projectile.damage = OrignalDamage * NumProj;
                    double deg = (double)CircleArr; //The degrees, you can multiply Projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                    double rad = deg * (Math.PI / 180); //Convert degrees to radian

                    Projectile.position.X = 50 * (float)Math.Cos(rad) + Main.player[Projectile.owner].Center.X - Projectile.width / 2;
                    Projectile.position.Y = 50 * (float)Math.Sin(rad) + Main.player[Projectile.owner].Center.Y - Projectile.height / 2;


                    CircleArr += 1.7f;
                }
            }
            else if (!charge)
            {
                if (PosCheck > 2)
                {
                    Vector2 position = Projectile.Center;
                    Vector2 targetPosition = Main.MouseWorld + Projectile.position - Main.player[Projectile.owner].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 11;
                    SoundEngine.PlaySound(SoundID.Item8, Projectile.position);
                    charge = true;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, direction.X * speed, direction.Y * speed, ModContent.ProjectileType<ShotSpike>(), Projectile.damage, 0f, Projectile.owner, 0f);
                }
                Projectile.Kill();
            }

            if (PosPlay > 9 && PosCheck < 30)
            {
                Projectile.Kill();
            }
        }
    }
}
