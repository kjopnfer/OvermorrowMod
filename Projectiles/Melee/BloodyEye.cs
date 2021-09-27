using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Melee
{
    public class BloodyEye : ModProjectile
    {
        private int SavedDMG = 0;
        private int timer = 0;
        private bool ComingBack = false;
        private int flametimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eater Boomerang");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.timeLeft = 100;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            timer++;
            if(timer == 1)
            {
                SavedDMG = projectile.damage;
            }

			projectile.rotation += 0.36f; 

            if(projectile.timeLeft < 65)
            {
                projectile.timeLeft = 10;
                ComingBack = true;
            }

            if(projectile.timeLeft > 98)
            {
                projectile.tileCollide = false;
            }
            else if(!ComingBack)
            {
                projectile.tileCollide = true;
            }

            if(ComingBack)
            {
                flametimer++;
                float BetweenKill = Vector2.Distance(Main.player[projectile.owner].Center, projectile.Center);
                projectile.tileCollide = false;
                Vector2 position = projectile.Center;
                Vector2 targetPosition = Main.player[projectile.owner].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                projectile.velocity = direction * 18;
                if(BetweenKill < 22)
                {
				    projectile.Kill();    
                }
            }
        }



        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Vector2 eee = projectile.Center;
            Main.PlaySound(SoundID.Item54, (int)eee.X, (int)eee.Y);
            {
                ComingBack = true;
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Texture2D texture = mod.GetTexture("Projectiles/Melee/BloodyEye");

                Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
                for (int k = 0; k < projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                    Color color = projectile.GetAlpha(Color.White) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                    spriteBatch.Draw(texture, drawPos, new Rectangle?(), color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
                }
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Vector2 eeee = projectile.Center;
            Main.PlaySound(SoundID.Item54, (int)eeee.X, (int)eeee.Y);
            ComingBack = true;
        }
    }
}
