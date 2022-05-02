using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Summon;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Summoner.SkeletonStaff
{
    public class SkeletronSumm : ModProjectile
    {
        int Random2 = Main.rand.Next(-15, 12);
        int Random = Main.rand.Next(1, 3);
        public override bool? CanDamage() => false;

        private int timer = 0;
        private int PosCheck = 0;
        int mrand = Main.rand.Next(-100, 101);
        int mrand2 = Main.rand.Next(-100, 101);


        private bool up = true;
        private bool down = false;
        int Flamerot = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("PufferFish");
            Main.projFrames[base.Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.sentry = true;
            Projectile.width = 52;
            Projectile.height = 60;
            Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 80000;
        }

        public override void AI()
        {


            Player player = Main.player[Projectile.owner];
            player.UpdateMaxTurrets();
            #region Active check
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<SkullBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<SkullBuff>()))
            {
                Projectile.timeLeft = 2;
            }
            #endregion


            PosCheck++;

            Projectile.position.X = Main.player[Projectile.owner].Center.X - 22;
            Projectile.position.Y = Main.player[Projectile.owner].Center.Y - 100;
            Projectile.rotation = (Projectile.Center - Main.MouseWorld).ToRotation();


            Projectile.rotation = (Main.MouseWorld - Projectile.Center).ToRotation();

            Projectile.spriteDirection = -1;

            if (Main.player[Projectile.owner].channel)
            {
                timer++;
                if (timer == 5)
                {
                    Vector2 position = Projectile.Center;
                    Vector2 targetPosition = Main.MouseWorld;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    Vector2 newpoint2 = new Vector2(direction.X, direction.Y).RotatedBy(MathHelper.ToRadians(Flamerot));
                    float speed = 15.5f;
                    SoundEngine.PlaySound(SoundID.Item, Projectile.position, 34);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, 10).RotatedBy(MathHelper.ToRadians(Projectile.rotation)), newpoint2 * speed, ModContent.ProjectileType<SpritFlame>(), Projectile.damage, 1f, Projectile.owner, 0f);

                    if (up)
                    {
                        Flamerot += 3;
                    }
                    if (down)
                    {
                        Flamerot -= 3;
                    }

                    if (Flamerot > 20)
                    {
                        up = false;
                        down = true;
                    }
                    if (Flamerot < -20)
                    {
                        up = true;
                        down = false;
                    }
                    timer = 0;

                }
            }

            if (Main.MouseWorld.X > Projectile.Center.X && !Main.player[Projectile.owner].channel)
            {
                Projectile.frame = 1;
            }
            if (Main.MouseWorld.X > Projectile.Center.X && Main.player[Projectile.owner].channel)
            {
                Projectile.frame = 0;
            }

            if (Main.MouseWorld.X < Projectile.Center.X && !Main.player[Projectile.owner].channel)
            {
                Projectile.frame = 2;
            }
            if (Main.MouseWorld.X < Projectile.Center.X && Main.player[Projectile.owner].channel)
            {
                Projectile.frame = 3;
            }
        }
    }
}

