using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.SpiderBoss
{
    public class SpiderRay2 : ModProjectile
    {
        private const string ChainTexturePath = "Juvenation/NPCs/SpiderBoss/SpiderRayWeb";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spider Ray");
        }
        private int timer = 1000;
        Vector2 SpiderPos = new Vector2(0f, 0f);
        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 32;
            projectile.timeLeft = 300;
            projectile.light = 3f;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            timer++;
            NPC npc = Main.npc[(int)projectile.ai[0]];
            SpiderPos = npc.Center;
            if(timer == 500)
            {
                projectile.timeLeft = 0;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            Vector2 endPoint;
            endPoint.X = SpiderPos.X;
            endPoint.Y = SpiderPos.Y;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, endPoint, 4f, ref point);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            projectile.alpha = 0;
            projectile.velocity = Vector2.Zero;
            Vector2 endPoint;
            endPoint.X = SpiderPos.X;
            endPoint.Y = SpiderPos.Y;
            Texture2D chainTexture = ModContent.GetTexture(ChainTexturePath);

            Vector2 unit = endPoint - projectile.Center; // changing all endpoints it just how you change it, dont change other stuff it wont go well
            float length = unit.Length();
            unit.Normalize();
            for (float k = 0; k <= length; k += 4f)
            {
                Vector2 drawPos = projectile.Center + unit * k - Main.screenPosition;
                Color alpha = Color.White;
                spriteBatch.Draw(chainTexture, drawPos, null, alpha, projectile.rotation, new Vector2(2, 2), 1f, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}
