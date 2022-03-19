using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.BloodEyes
{
    public class BloodyEye : ModProjectile
    {
        private bool ComingBack = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloody Eyes");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
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
            projectile.rotation += .55f;

            if (projectile.timeLeft < 65)
            {
                projectile.timeLeft = 10;
                ComingBack = true;
            }

            if (projectile.timeLeft > 98)
            {
                projectile.tileCollide = false;
            }
            else if (!ComingBack)
            {
                projectile.tileCollide = true;
            }

            if (ComingBack)
            {
                float BetweenKill = Vector2.Distance(Main.player[projectile.owner].Center, projectile.Center);
                projectile.tileCollide = false;
                Vector2 position = projectile.Center;
                Vector2 targetPosition = Main.player[projectile.owner].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                projectile.velocity = direction * 18;
                if (BetweenKill < 22)
                {
                    projectile.Kill();
                }
            }
        }



        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            ComingBack = true;
            
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture(AssetDirectory.Melee + "BloodEyes/BloodyEye_Trail");

            Color color = Color.Red;
            int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y2 = num154 * projectile.frame;
            Rectangle drawRectangle = new Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);

            Vector2 origin2 = drawRectangle.Size() / 2f;
            var off = new Vector2(projectile.width / 2f, projectile.height / 2f);


            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color2 = color;
                color2 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];

                float scale = projectile.scale * (ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Main.spriteBatch.Draw(texture, projectile.oldPos[i] - Main.screenPosition + off, new Microsoft.Xna.Framework.Rectangle?(drawRectangle), color2, num165, origin2, scale, SpriteEffects.None, 0f);
            }
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            ComingBack = true;
        }
    }
}
