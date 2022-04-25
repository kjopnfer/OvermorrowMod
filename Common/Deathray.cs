using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public abstract class Deathray : ModProjectile
    {
        ///<param name="MaxTime"> Duration of the laser beam <param/>
        ///<param name="LaserLenght"> Lenght of the laser <param/>
        ///<param name="rotation"> Amount the laser rotates per tick <param/> 
        ///<param name="laserColor"> Self explanatory <param/>
        protected Deathray(float MaxTime, float LaserLength, float rotation, Color laserColor, string texture)
        {
            this.MaxTime = MaxTime;
            this.laserColor = laserColor;
            this.LaserLength = LaserLength;
            this.Projectile.ai[0] = rotation;
            this.TexturePath = texture;
        }
        public float RotationalSpeed
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        public string TexturePath;
        public Color laserColor = Color.White; // just color
        public Texture2D LaserBeginTexture => ModContent.Request<Texture2D>(TexturePath).Value;//Main.projectileTexture[Projectile.type];
        public Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>(TexturePath + "1").Value;
        public Texture2D LaserEndTexture => ModContent.Request<Texture2D>(TexturePath + "2").Value;
        public float LaserLength = 1000; // laser lenght
        public float timer { get { return Projectile.localAI[0]; } set { Projectile.localAI[0] = value; } }
        public float MaxTime = 0; // time the laser lasts
        public void DrawLaser(Color beamColor, float scale)
        {
            Main.EntitySpriteDraw(LaserBeginTexture, Projectile.Center - Main.screenPosition, null, laserColor * Projectile.Opacity, Projectile.rotation, new Vector2(LaserBeginTexture.Width * scale, LaserBeginTexture.Height) / 2, new Vector2(scale, 1f), SpriteEffects.None, 0);
            float middleLaserLength = LaserLength;
            middleLaserLength -= (LaserBeginTexture.Height / 2 + LaserEndTexture.Height);
            Vector2 centerOnLaser = Projectile.Center;
            centerOnLaser += Projectile.velocity * LaserBeginTexture.Height / 2f;

            if (middleLaserLength > 0f)
            {
                float laserOffset = LaserMiddleTexture.Height;
                float incrementalBodyLength = 0f;
                while (incrementalBodyLength + 1f < middleLaserLength)
                {
                    Main.EntitySpriteDraw(LaserMiddleTexture, centerOnLaser - Main.screenPosition, null, laserColor * Projectile.Opacity, Projectile.rotation, new Vector2(LaserMiddleTexture.Width * scale, LaserMiddleTexture.Height) / 2f, new Vector2(scale, 1f), SpriteEffects.None, 0);
                    incrementalBodyLength += laserOffset;
                    centerOnLaser += Projectile.velocity * laserOffset;
                }
            }

            if (Math.Abs(LaserLength - 2000) < 30f)
            {
                Vector2 laserEndCenter = centerOnLaser - Main.screenPosition;
                Main.EntitySpriteDraw(LaserEndTexture, laserEndCenter, null, laserColor * Projectile.Opacity, Projectile.rotation, new Vector2(LaserEndTexture.Width * scale, LaserEndTexture.Height) / 2f, new Vector2(scale, 1f), SpriteEffects.None, 0);
            }
        }
        public override void PostAI()
        {
            timer++;
            if (timer >= MaxTime)
            {
                Projectile.Kill();
            }

            Projectile.velocity = Projectile.velocity.SafeNormalize(-Vector2.UnitY);

            /*Projectile.scale = (float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2;
            if (Projectile.scale > 1) Projectile.scale = 1;*/

            float newDirection = Projectile.velocity.ToRotation() + MathHelper.ToRadians(RotationalSpeed);
            Projectile.rotation = newDirection - MathHelper.PiOver2;
            Projectile.velocity = newDirection.ToRotationVector2();

            /*float idealLaserLength = 2000;
            LaserLength = MathHelper.Lerp(LaserLength, idealLaserLength, 0.9f);*/

            CastLights();
        }
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Terraria.Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, Projectile.Size.Length() * Projectile.scale, DelegateMethods.CutTiles);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, Projectile.Size.Length() * Projectile.scale, ref _);
        }
        private void CastLights()
        {
            DelegateMethods.v3_1 = laserColor.ToVector3();
            Terraria.Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, Projectile.width * Projectile.scale, new Utils.TileActionAttempt(DelegateMethods.CastLight));
        }
        public override bool ShouldUpdatePosition() => false;
    }
}