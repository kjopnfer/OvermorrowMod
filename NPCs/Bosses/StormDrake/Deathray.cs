//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using Terraria.Graphics.Shaders;
//using Terraria;
//using Terraria.Enums;
//using Terraria.ModLoader;
//using Terraria.ID;

//namespace OvermorrowMod.NPCs.Bosses.StormDrake
//{
//    public abstract class Deathray : ModProjectile
//    {
//        ///<param name="MaxTime"> Duration of the laser beam <param/>
//        ///<param name="LaserLenght"> Lenght of the laser <param/>
//        ///<param name="rotation"> Amount the laser rotates per tick <param/> 
//        ///<param name="laserColor"> Self explanatory <param/> 
//        protected Deathray(float MaxTime, float LaserLength, float rotation, Color laserColor, string texture)
//        {
//            this.MaxTime = MaxTime;
//            this.laserColor = laserColor;
//            this.LaserLength = LaserLength;
//            this.projectile.ai[0] = rotation;
//            this.TexturePath = texture;
//        }
//        public float RotationalSpeed
//        {
//            get => projectile.ai[0];
//            set => projectile.ai[0] = value;
//        }
//        public override void SetDefaults()
//        {
//            projectile.width = 10;
//            projectile.height = 10;
//            projectile.penetrate = -1;
//            projectile.tileCollide = false;
//        }
//        public string TexturePath;
//        public Color laserColor = Color.White; // just color
//        public Texture2D LaserBeginTexture => Main.projectileTexture[projectile.type];
//        public Texture2D LaserMiddleTexture => mod.GetTexture(TexturePath + "1");
//        public Texture2D LaserEndTexture => mod.GetTexture(TexturePath + "2");
//        public float LaserLength = 1000; // laser lenght
//        public float timer { get { return projectile.localAI[0]; } set { projectile.localAI[0] = value; } }
//        public float MaxTime = 0; // time the laser lasts
//        public void DrawLaser(SpriteBatch spriteBatch, Color beamColor, float scale)
//        {
//            spriteBatch.Draw(LaserBeginTexture, projectile.Center - Main.screenPosition, null, Color.White * projectile.Opacity, projectile.rotation, new Vector2(LaserBeginTexture.Width * scale, LaserBeginTexture.Height) / 2, new Vector2(scale, 1f), SpriteEffects.None, 0f);
//            float middleLaserLength = LaserLength;
//            middleLaserLength -= (LaserBeginTexture.Height / 2 + LaserEndTexture.Height);
//            Vector2 centerOnLaser = projectile.Center;
//            centerOnLaser += projectile.velocity * LaserBeginTexture.Height / 2f;

//            if (middleLaserLength > 0f)
//            {
//                float laserOffset = LaserMiddleTexture.Height;
//                float incrementalBodyLength = 0f;
//                while (incrementalBodyLength + 1f < middleLaserLength)
//                {
//                    spriteBatch.Draw(LaserMiddleTexture, centerOnLaser - Main.screenPosition, null, Color.White * projectile.Opacity, projectile.rotation, new Vector2(LaserMiddleTexture.Width * scale, LaserMiddleTexture.Height) / 2f, new Vector2(scale, 1f), SpriteEffects.None, 0f);
//                    incrementalBodyLength += laserOffset;
//                    centerOnLaser += projectile.velocity * laserOffset;
//                }
//            }

//            if (Math.Abs(LaserLength - 2000) < 30f)
//            {
//                Vector2 laserEndCenter = centerOnLaser - Main.screenPosition;
//                spriteBatch.Draw(LaserEndTexture, laserEndCenter, null, Color.White * projectile.Opacity, projectile.rotation, new Vector2(LaserEndTexture.Width * scale, LaserEndTexture.Height) / 2f, new Vector2(scale, 1f), SpriteEffects.None, 0f);
//            }
//        }
//        public override void PostAI()
//        {
//            timer++;
//            if (timer >= MaxTime)
//            {
//                projectile.Kill();
//            }

//            projectile.velocity = projectile.velocity.SafeNormalize(-Vector2.UnitY);

//            /*projectile.scale = (float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2;
//            if (projectile.scale > 1) projectile.scale = 1;*/

//            float newDirection = projectile.velocity.ToRotation() + MathHelper.ToRadians(RotationalSpeed);
//            projectile.rotation = newDirection - MathHelper.PiOver2;
//            projectile.velocity = newDirection.ToRotationVector2();

//            /*float idealLaserLength = 2000;
//            LaserLength = MathHelper.Lerp(LaserLength, idealLaserLength, 0.9f);*/

//            CastLights();
//        }
//        public override void CutTiles()
//        {
//            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
//            Terraria.Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * LaserLength, projectile.Size.Length() * projectile.scale, DelegateMethods.CutTiles);
//        }
//        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
//        {
//            DrawLaser(spriteBatch, laserColor, projectile.scale);
//            return false;
//        }
//        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
//        {
//            float _ = 0f;
//            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * LaserLength, projectile.Size.Length() * projectile.scale, ref _);
//        }
//        private void CastLights()
//        {
//            DelegateMethods.v3_1 = laserColor.ToVector3();
//            Terraria.Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * LaserLength, projectile.width * projectile.scale, new Terraria.Utils.PerLinePoint(DelegateMethods.CastLight));
//        }
//        public override bool ShouldUpdatePosition() => false;
//    }
//}


//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using Terraria.Graphics.Shaders;
//using Terraria;
//using Terraria.Enums;
//using Terraria.ModLoader;
//using Terraria.ID;

//namespace OvermorrowMod.NPCs.Bosses.StormDrake
//{
//    public abstract class Deathray : ModProjectile
//    {
//        ///<param name="MaxTime"> Duration of the laser beam <param/>
//        ///<param name="LaserLenght"> Lenght of the laser <param/>
//        ///<param name="rotation"> Amount the laser rotates per tick <param/> 
//        ///<param name="laserColor"> Self explanatory <param/> 
//        protected Deathray(float MaxTime, float LaserLength, float rotation, Color laserColor, string texture)
//        {
//            this.MaxTime = MaxTime;
//            this.laserColor = laserColor;
//            this.LaserLength = LaserLength;
//            this.projectile.ai[0] = rotation;
//            this.TexturePath = texture;
//        }
//        public float RotationalSpeed
//        {
//            get => projectile.ai[0];
//            set => projectile.ai[0] = value;
//        }
//        public override void SetDefaults()
//		{
//			projectile.width = 10;
//			projectile.height = 10;
//			projectile.penetrate = -1;
//			projectile.tileCollide = false;
//		}
//        public string TexturePath;
//        public Color laserColor = Color.White; // just color
//        public Texture2D LaserBeginTexture => Main.projectileTexture[projectile.type];
//        public Texture2D LaserMiddleTexture => mod.GetTexture(TexturePath + "1");
//        public Texture2D LaserEndTexture => mod.GetTexture(TexturePath + "2");
//        public float LaserLength = 1000; // laser lenght
//        public float timer{ get {return projectile.localAI[0];} set {projectile.localAI[0] = value;}}
//        public float MaxTime = 0; // time the laser lasts
//        public void DrawLaser(SpriteBatch spriteBatch, Color beamColor, float scale)
//        {
//            spriteBatch.Draw(LaserBeginTexture, projectile.Center - Main.screenPosition, null, Color.White * projectile.Opacity, projectile.rotation, new Vector2(LaserBeginTexture.Width * scale, LaserBeginTexture.Height) / 2, new Vector2(scale, 1f), SpriteEffects.None, 0f);
//            float middleLaserLength = LaserLength;
//            middleLaserLength -= (LaserBeginTexture.Height / 2 + LaserEndTexture.Height);
//            Vector2 centerOnLaser = projectile.Center;
//            centerOnLaser += projectile.velocity * LaserBeginTexture.Height / 2f;

//            if (middleLaserLength > 0f)
//            {
//                float laserOffset = LaserMiddleTexture.Height;
//                float incrementalBodyLength = 0f;
//                while (incrementalBodyLength + 1f < middleLaserLength)
//                {
//                    spriteBatch.Draw(LaserMiddleTexture, centerOnLaser - Main.screenPosition, null, Color.White * projectile.Opacity, projectile.rotation, new Vector2(LaserMiddleTexture.Width * scale, LaserMiddleTexture.Height) / 2f, new Vector2(scale, 1f), SpriteEffects.None, 0f);
//                    incrementalBodyLength += laserOffset;
//                    centerOnLaser += projectile.velocity * laserOffset;
//                }
//            }

//            if (Math.Abs(LaserLength - 2000) < 30f)
//            {
//                Vector2 laserEndCenter = centerOnLaser - Main.screenPosition;
//                spriteBatch.Draw(LaserEndTexture, laserEndCenter, null, Color.White * projectile.Opacity, projectile.rotation, new Vector2(LaserEndTexture.Width * scale, LaserEndTexture.Height) / 2f, new Vector2(scale, 1f), SpriteEffects.None, 0f);
//            }
//        }
//        public override void PostAI()
//        {
//            timer++;
//            if (timer >= MaxTime)
//            {
//                projectile.Kill();
//            }

//            projectile.velocity = projectile.velocity.SafeNormalize(-Vector2.UnitY);

//            /*projectile.scale = (float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2;
//            if (projectile.scale > 1) projectile.scale = 1;*/  

//            float newDirection = projectile.velocity.ToRotation() + MathHelper.ToRadians(RotationalSpeed);
//            projectile.rotation = newDirection - MathHelper.PiOver2; 
//            projectile.velocity = newDirection.ToRotationVector2();

//            /*float idealLaserLength = 2000;
//            LaserLength = MathHelper.Lerp(LaserLength, idealLaserLength, 0.9f);*/  

//            CastLights();
//        }
//        public override void CutTiles()
//        {
//            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
//            Terraria.Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * LaserLength, projectile.Size.Length() * projectile.scale, DelegateMethods.CutTiles);
//        }
//        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
//        {
//            DrawLaser(spriteBatch, laserColor, projectile.scale);
//            return false;
//        }
//        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
//        {
//            float _ = 0f;
//            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * LaserLength, projectile.Size.Length() * projectile.scale, ref _);
//        }
//        private void CastLights()
//        {
//            DelegateMethods.v3_1 = laserColor.ToVector3();
//            Terraria.Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * LaserLength, projectile.width * projectile.scale, new Terraria.Utils.PerLinePoint(DelegateMethods.CastLight));
//        }
//        public override bool ShouldUpdatePosition() => false;
//    }
//}

//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using Terraria.Graphics.Shaders;
//using Terraria;
//using Terraria.Enums;
//using Terraria.ModLoader;
//using Terraria.ID;

//namespace OvermorrowMod.NPCs.Bosses//.StormDrake
//{
//    public abstract class Deathray : ModProjectile
//    {
//        ///<param name="MaxTime"> Duration of the laser beam <param/>
//        ///<param name="LaserLenght"> Lenght of the laser <param/>
//        ///<param name="rotation"> Amount the laser rotates per tick <param/> 
//        ///<param name="laserColor"> Self explanatory <param/> 
//        protected Deathray(float MaxTime, float LaserLength, float rotation, Color laserColor, string texture)
//        {
//            this.MaxTime = MaxTime;
//            this.laserColor = laserColor;
//            this.LaserLength = LaserLength;
//            this.projectile.ai[0] = rotation;
//            this.TexturePath = texture;
//        }
//        /*public float RotationalSpeed
//        {
//            get => projectile.ai[0];
//            set => projectile.ai[0] = value;
//        }*/
//        public override void SetDefaults()
//        {
//            projectile.width = 10;
//            projectile.height = 10;
//            projectile.penetrate = -1;
//            projectile.tileCollide = false;
//        }
//        public string TexturePath;
//        public Color laserColor = Color.White; // just color
//        public Texture2D LaserBeginTexture => mod.GetTexture(Texture);//Main.projectileTexture[projectile.type];
//        public Texture2D LaserMiddleTexture => mod.GetTexture(TexturePath + "1");
//        public Texture2D LaserEndTexture => mod.GetTexture(TexturePath + "2");
//        public float LaserLength = 1000; // laser lenght
//        public float timer; // { get { return projectile.localAI[0]; } set { projectile.localAI[0] = value; } }
//        public float MaxTime = 0; // time the laser lasts
//        public void DrawLaser(SpriteBatch spriteBatch, Color beamColor, float scale)
//        {
//            //spriteBatch.Draw(LaserBeginTexture, projectile.Center - Main.screenPosition, null, Color.White * projectile.Opacity, projectile.rotation, new Vector2(LaserBeginTexture.Width * scale, LaserBeginTexture.Height) / 2, new Vector2(scale, 1f), SpriteEffects.None, 0f);
//            float middleLaserLength = LaserLength;
//            middleLaserLength -= (LaserBeginTexture.Height / 2 + LaserEndTexture.Height);
//            Vector2 centerOnLaser = projectile.Center;
//            centerOnLaser += projectile.velocity * LaserBeginTexture.Height / 2f;

//            if (middleLaserLength > 0f)
//            {
//                float laserOffset = LaserMiddleTexture.Height;
//                float incrementalBodyLength = 0f;
//                while (incrementalBodyLength + 1f < middleLaserLength)
//                {
//                    spriteBatch.Draw(LaserMiddleTexture, centerOnLaser - Main.screenPosition, null, Color.White * projectile.Opacity, projectile.rotation, new Vector2(LaserMiddleTexture.Width * scale, LaserMiddleTexture.Height) / 2f, new Vector2(scale, 1f), SpriteEffects.None, 0f);
//                    incrementalBodyLength += laserOffset;
//                    centerOnLaser += projectile.velocity * laserOffset;
//                }
//            }

//            /*if (Math.Abs(LaserLength - 2000) < 30f)
//            {
//                Vector2 laserEndCenter = centerOnLaser - Main.screenPosition;
//                spriteBatch.Draw(LaserEndTexture, laserEndCenter, null, Color.White * projectile.Opacity, projectile.rotation, new Vector2(LaserEndTexture.Width * scale, LaserEndTexture.Height) / 2f, new Vector2(scale, 1f), SpriteEffects.None, 0f);
//            }*/
//        }
//        public override void PostAI()
//        {
//            timer++;
//            if (timer >= MaxTime)
//            {
//                projectile.Kill();
//            }

//            projectile.velocity = projectile.velocity.SafeNormalize(-Vector2.UnitY);

//            /*projectile.scale = (float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2;
//            if (projectile.scale > 1) projectile.scale = 1;*/

//            /*float newDirection = projectile.velocity.ToRotation() + MathHelper.ToRadians(RotationalSpeed);
//            projectile.rotation = newDirection - MathHelper.PiOver2;
//            projectile.velocity = newDirection.ToRotationVector2();*/

//            /*float idealLaserLength = 2000;
//            LaserLength = MathHelper.Lerp(LaserLength, idealLaserLength, 0.9f);*/

//            //CastLights();
//        }
//        /*public override void CutTiles()
//        {
//            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
//            Terraria.Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * LaserLength, projectile.Size.Length() * projectile.scale, DelegateMethods.CutTiles);
//        }*/
//        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
//        {
//            DrawLaser(spriteBatch, laserColor, projectile.scale);
//            return false;
//        }
//        /*public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
//        {
//            float _ = 0f;
//            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * LaserLength, projectile.Size.Length() * projectile.scale, ref _);
//        }
//        private void CastLights()
//        {
//            DelegateMethods.v3_1 = laserColor.ToVector3();
//            Terraria.Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * LaserLength, projectile.width * projectile.scale, new Terraria.Utils.PerLinePoint(DelegateMethods.CastLight));
//        }
//        public override bool ShouldUpdatePosition() => false;*/
//    }
//}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Graphics.Shaders;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ID;

namespace OvermorrowMod.NPCs.Bosses
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
            this.projectile.ai[0] = rotation;
            this.TexturePath = texture;
        }
        public float RotationalSpeed
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
        }
        public string TexturePath;
        public Color laserColor = Color.White; // just color
        public Texture2D LaserBeginTexture => mod.GetTexture(TexturePath);//Main.projectileTexture[projectile.type];
        public Texture2D LaserMiddleTexture => mod.GetTexture(TexturePath + "1");
        public Texture2D LaserEndTexture => mod.GetTexture(TexturePath + "2");
        public float LaserLength = 1000; // laser lenght
        public float timer { get {return projectile.localAI[0];} set {projectile.localAI[0] = value;}}
        public float MaxTime = 0; // time the laser lasts
        public void DrawLaser(SpriteBatch spriteBatch, Color beamColor, float scale)
        {
            spriteBatch.Draw(LaserBeginTexture, projectile.Center - Main.screenPosition, null, Color.White * projectile.Opacity, projectile.rotation, new Vector2(LaserBeginTexture.Width * scale, LaserBeginTexture.Height) / 2, new Vector2(scale, 1f), SpriteEffects.None, 0f);
            float middleLaserLength = LaserLength;
            middleLaserLength -= (LaserBeginTexture.Height / 2 + LaserEndTexture.Height);
            Vector2 centerOnLaser = projectile.Center;
            centerOnLaser += projectile.velocity * LaserBeginTexture.Height / 2f;

            if (middleLaserLength > 0f)
            {
                float laserOffset = LaserMiddleTexture.Height;
                float incrementalBodyLength = 0f;
                while (incrementalBodyLength + 1f < middleLaserLength)
                {
                    spriteBatch.Draw(LaserMiddleTexture, centerOnLaser - Main.screenPosition, null, Color.White * projectile.Opacity, projectile.rotation, new Vector2(LaserMiddleTexture.Width * scale, LaserMiddleTexture.Height) / 2f, new Vector2(scale, 1f), SpriteEffects.None, 0f);
                    incrementalBodyLength += laserOffset;
                    centerOnLaser += projectile.velocity * laserOffset;
                }
            }

            if (Math.Abs(LaserLength - 2000) < 30f)
            {
                Vector2 laserEndCenter = centerOnLaser - Main.screenPosition;
                spriteBatch.Draw(LaserEndTexture, laserEndCenter, null, Color.White * projectile.Opacity, projectile.rotation, new Vector2(LaserEndTexture.Width * scale, LaserEndTexture.Height) / 2f, new Vector2(scale, 1f), SpriteEffects.None, 0f);
            }
        }
        public override void PostAI()
        {
            timer++;
            if (timer >= MaxTime)
            {
                projectile.Kill();
            }

            projectile.velocity = projectile.velocity.SafeNormalize(-Vector2.UnitY);

            /*projectile.scale = (float)Math.Sin(timer / MaxTime * MathHelper.Pi) * 2;
            if (projectile.scale > 1) projectile.scale = 1;*/

            float newDirection = projectile.velocity.ToRotation(); //+ MathHelper.ToRadians(RotationalSpeed);
            projectile.rotation = newDirection - MathHelper.PiOver2;
            projectile.velocity = newDirection.ToRotationVector2();

            /*float idealLaserLength = 2000;
            LaserLength = MathHelper.Lerp(LaserLength, idealLaserLength, 0.9f);*/

            CastLights();
        }
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Terraria.Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * LaserLength, projectile.Size.Length() * projectile.scale, DelegateMethods.CutTiles);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            DrawLaser(spriteBatch, laserColor, projectile.scale);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * LaserLength, projectile.Size.Length() * projectile.scale, ref _);
        }
        private void CastLights()
        {
            DelegateMethods.v3_1 = laserColor.ToVector3();
            Terraria.Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * LaserLength, projectile.width * projectile.scale, new Terraria.Utils.PerLinePoint(DelegateMethods.CastLight));
        }
        public override bool ShouldUpdatePosition() => false;
    }
}