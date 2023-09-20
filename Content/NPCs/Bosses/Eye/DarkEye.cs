/*using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    public class DarkEye : ModProjectile
    {
        //private int maxTime = 300;
        private int maxTime = 180;

        public override bool CanHitPlayer(Player target) => Projectile.timeLeft < maxTime - 51 && Projectile.timeLeft > 51;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 136;
            Projectile.height = 120;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = maxTime;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            if (Projectile.timeLeft > maxTime - 51)
                Projectile.alpha -= 5;
            else if (Projectile.timeLeft <= 51)
                Projectile.alpha += 5;

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // 51 ticks after the projectile fades in, which runs for 1 second
            if (Projectile.timeLeft > maxTime - 51 - 60 && Projectile.timeLeft < maxTime - 51)
            {
                if (!Main.gamePaused) Projectile.ai[0]++;

                int amount = 5;
                float progress = Utils.Clamp(Projectile.ai[0], 0, 60) / 60f;

                for (int i = 0; i < amount; i++)
                {
                    float scaleAmount = i / (float)amount;
                    float scale = 1f + progress * scaleAmount;

                    int width = TextureAssets.Projectile[Projectile.type].Value.Width;
                    int height = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
                    Rectangle drawRectangle = new Rectangle(0, height * Projectile.frame, width, height);

                    Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, drawRectangle, Color.Purple * (1f - progress), Projectile.rotation, drawRectangle.Size() / 2, scale * Projectile.scale, SpriteEffects.None, 0);
                }
            }

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            int width = TextureAssets.Projectile[Projectile.type].Value.Width;
            int height = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
            Rectangle drawRectangle = new Rectangle(0, height * Projectile.frame, width, height);

            float progress = MathHelper.Lerp(1, 0, Projectile.alpha / 255f);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, drawRectangle, Color.White * progress, Projectile.rotation, drawRectangle.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        }
    }
}*/