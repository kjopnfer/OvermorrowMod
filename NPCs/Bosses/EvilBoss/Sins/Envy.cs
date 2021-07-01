using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.EvilBoss.Sins
{
    public class Envy : ModProjectile
    {
        private int SavedDMG = 0;
        private int timer = 0;
        private bool ComingBack = false;
        private int flametimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sin of Envy");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 42;
            projectile.timeLeft = 140;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
        }
        public override void AI()
        {
            Projectile parentProjectile = Main.projectile[(int)projectile.ai[0]];
            timer++;

			projectile.rotation += 0.36f; 

            if(projectile.timeLeft < 40)
            {
                projectile.timeLeft = 10;
                ComingBack = true;
            }


            if(ComingBack)
            {
                flametimer++;
                float BetweenKill = Vector2.Distance(parentProjectile.Center, projectile.Center);
                projectile.tileCollide = false;
                Vector2 position = projectile.Center;
                Vector2 targetPosition = parentProjectile.Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                projectile.velocity = direction * 18;
                if(BetweenKill < 22)
                {
				    projectile.Kill();    
                }
            }
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Texture2D texture = mod.GetTexture("NPCs/Bosses/EvilBoss/Sins/Envy");

                Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
                for (int k = 0; k < projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                    Color color = projectile.GetAlpha(Color.White) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                    spriteBatch.Draw(texture, drawPos, new Rectangle?(), color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
                }
            return true;
        }
    }
}
