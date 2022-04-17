using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace OvermorrowMod.Content.Items.Weapons.Magic.IorichStaff
{
    public class NatureSpike : ModProjectile, ITrailEntity
    {
        public Color TrailColor(float progress) => new Color(66, 245, 167);
        public float TrailSize(float progress) => 20;
        public bool TrailActive() => true;
        public Type TrailType() => typeof(SpikeTrail);

        private bool RunOnce = true;
        private float Radius;
        public NPC RotationCenter;
        public Vector2 OldPosition;
        public float StartingRotation;
        public float RandomOffset;
        public bool Converge = false;
        public bool Ready = false;

        public Color ProjectileColor = new Color(54, 255, 64);
        public override string Texture => AssetDirectory.Empty;
        public override bool? CanDamage() => Converge;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit Blades");
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            if (RunOnce)
            {
                StartingRotation = Projectile.ai[0];
                Radius = Projectile.ai[1];

                Projectile.ai[0] = 0;
                Projectile.ai[1] = 0;

                RunOnce = false;
            }

            #region Dust Code
            if (Projectile.ai[1] == 0)
            {
                SoundEngine.PlaySound(SoundID.Item25, Projectile.Center);

                Vector2 vector23 = Projectile.Center + Vector2.One * -20f;
                int num137 = 40;
                int num138 = num137;
                for (int num139 = 0; num139 < 4; num139++)
                {
                    int num140 = Dust.NewDust(vector23, num137, num138, 89, 0f, 0f, 100, default(Color), 0.25f);
                    Main.dust[num140].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
                }

                for (int num141 = 0; num141 < 10; num141++)
                {
                    int num142 = Dust.NewDust(vector23, num137, num138, 107, 0f, 0f, 200, default(Color), 0.7f);
                    Main.dust[num142].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
                    Main.dust[num142].noGravity = true;
                    Main.dust[num142].noLight = true;
                    Dust dust = Main.dust[num142];
                    dust.velocity *= 3f;
                    dust = Main.dust[num142];
                    dust.velocity += Projectile.DirectionTo(Main.dust[num142].position) * (2f + Main.rand.NextFloat() * 4f);
                    num142 = Dust.NewDust(vector23, num137, num138, 107, 0f, 0f, 100, default(Color), 0.25f);
                    Main.dust[num142].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
                    dust = Main.dust[num142];
                    dust.velocity *= 2f;
                    Main.dust[num142].noGravity = true;
                    Main.dust[num142].fadeIn = 1f;
                    Main.dust[num142].color = Color.Crimson * 0.5f;
                    Main.dust[num142].noLight = true;
                    dust = Main.dust[num142];
                    dust.velocity += Projectile.DirectionTo(Main.dust[num142].position) * 8f;
                }

                for (int num143 = 0; num143 < 10; num143++)
                {
                    int num144 = Dust.NewDust(vector23, num137, num138, 107, 0f, 0f, 0, default(Color), 0.7f);
                    Main.dust[num144].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy(Projectile.velocity.ToRotation()) * num137 / 2f;
                    Main.dust[num144].noGravity = true;
                    Main.dust[num144].noLight = true;
                    Dust dust = Main.dust[num144];
                    dust.velocity *= 3f;
                    dust = Main.dust[num144];
                    dust.velocity += Projectile.DirectionTo(Main.dust[num144].position) * 2f;
                }

                for (int num145 = 0; num145 < 50; num145++)
                {
                    int num146 = Dust.NewDust(vector23, num137, num138, 89, 0f, 0f, 0, default(Color), 0.25f);
                    Main.dust[num146].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy(Projectile.velocity.ToRotation()) * num137 / 2f;
                    Main.dust[num146].noGravity = true;
                    Dust dust = Main.dust[num146];
                    dust.velocity *= 3f;
                    dust = Main.dust[num146];
                    dust.velocity += Projectile.DirectionTo(Main.dust[num146].position) * 3f;
                }
            }
            #endregion

            if (!RotationCenter.active)
            {
                Projectile.Kill();
            }

            if (Projectile.timeLeft > 80)
            {
                Projectile.rotation = Projectile.DirectionTo(RotationCenter.Center).ToRotation();

                double RotationCounter = MathHelper.Lerp(StartingRotation, StartingRotation + (MathHelper.TwoPi + MathHelper.ToRadians(RandomOffset)), Utils.Clamp(Projectile.ai[0]++, 0, 100f) / (100f));
                Projectile.Center = RotationCenter.Center + new Vector2(Radius, 0).RotatedBy(RotationCounter);
                OldPosition = RotationCenter.Center;
            }
            else
            {
                Ready = true;
                if (!Converge)
                {
                    Projectile.rotation = Projectile.DirectionTo(RotationCenter.Center).ToRotation();

                    double RotationCounter = MathHelper.Lerp(StartingRotation, StartingRotation + (MathHelper.TwoPi + MathHelper.ToRadians(RandomOffset)), Utils.Clamp(Projectile.ai[0]++, 0, 100f) / (100f));
                    Projectile.Center = RotationCenter.Center + new Vector2(Radius, 0).RotatedBy(RotationCounter);
                    OldPosition = RotationCenter.Center;

                    Projectile.timeLeft = 80;
                }
                else
                {
                    Projectile.extraUpdates = 10;

                    if (Projectile.timeLeft == 55)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_PhantomPhoenixShot, Projectile.Center);
                        Projectile.velocity = Vector2.Normalize(OldPosition - Projectile.Center) * 12;
                    }
                }
            }

            Projectile.ai[1]++;
            Projectile.localAI[0]++;
        }

        public override bool? CanCutTiles() => false;

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_644").Value;
            Rectangle rect = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), ProjectileColor * 0.5f, Projectile.rotation + MathHelper.PiOver2, drawOrigin, new Vector2(0.4f, 2f), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), ProjectileColor * 0.5f, Projectile.rotation, drawOrigin, new Vector2(0.4f, 1f), SpriteEffects.None, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D SoulTexture = ModContent.Request<Texture2D>("Terraria/Images/Extra_89").Value;

            Main.spriteBatch.Draw(SoulTexture, Projectile.Center - Main.screenPosition, null, ProjectileColor, Projectile.rotation + MathHelper.PiOver2, SoulTexture.Size() / 2, new Vector2(0.5f, 1), SpriteEffects.None, 0f);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 RandomVelocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(5, 15);
                Dust.NewDust(Projectile.Center, 2, 2, DustID.TerraBlade, RandomVelocity.X, RandomVelocity.Y);
            }
        }
    }
}