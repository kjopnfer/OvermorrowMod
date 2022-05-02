using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Summon;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Summoner.PufferStaff
{
    public class PufferFish : ModProjectile
    {
        int righttimer = 0;
        int lefttimer = 0;
        int Random2 = Main.rand.Next(-15, 12);
        int Random = Main.rand.Next(1, 3);
        public override bool? CanDamage() => false;

        private int timer = 0;
        private int PosCheck = 0;
        int mrand = Main.rand.Next(-100, 101);
        int mrand2 = Main.rand.Next(-100, 101);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("PufferFish");
            Main.projFrames[base.Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.sentry = true;
            Projectile.width = 44;
            Projectile.height = 48;
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
                player.ClearBuff(ModContent.BuffType<PufferBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<PufferBuff>()))
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
                if (timer == 10)
                {
                    int Random = Main.rand.Next(-15, 16);
                    Vector2 position = Projectile.Center;
                    Vector2 targetPosition = Main.MouseWorld;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    Vector2 newpoint2 = new Vector2(direction.X, direction.Y).RotatedByRandom(MathHelper.ToRadians(7f));
                    float speed = 15.5f + (Random * 0.10f);
                    SoundEngine.PlaySound(SoundID.Item, Projectile.position, 85);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, newpoint2.X * speed, newpoint2.Y * speed, ModContent.ProjectileType<PuffBubble>(), Projectile.damage, 1f, Projectile.owner, 0f);
                    timer = 0;
                }
            }

            if (Main.MouseWorld.X > Projectile.Center.X)
            {
                righttimer = 0;
                lefttimer++;
                if (lefttimer == 1)
                {
                    Projectile.frame = 2;
                }
                if (++Projectile.frameCounter >= 4)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame > 1)
                    {
                        Projectile.frame = 0;
                    }
                }
            }
            else
            {
                lefttimer = 0;
                righttimer++;
                if (righttimer == 1)
                {
                    Projectile.frame = 0;
                }
                if (++Projectile.frameCounter >= 4)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame > 3)
                    {
                        Projectile.frame = 2;
                    }
                }
            }
        }
    }
}

