using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.WardenClass;
using Terraria;

namespace OvermorrowMod.Projectiles.Artifact
{
    public class Pillar : ArtifactProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fortitude Pillar");
            Main.projFrames[projectile.type] = 19;
        }

        public override void SafeSetDefaults()
        {
            projectile.width = 82;
            projectile.height = 86;
            projectile.tileCollide = true;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 5400; // 1.5 minutes

        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y2 = num154 * projectile.frame;

            Texture2D texture = mod.GetTexture("Projectiles/Artifact/Pillar_Glow");
            Rectangle drawRectangle = new Microsoft.Xna.Framework.Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);
            spriteBatch.Draw
            (
                texture,
                new Vector2
                (
                    projectile.position.X - Main.screenPosition.X + projectile.width * 0.5f,
                    projectile.position.Y - Main.screenPosition.Y + projectile.height - drawRectangle.Height * 0.5f
                ),
                drawRectangle,
                Color.White,
                projectile.rotation,
                new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2),
                projectile.scale,
                projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0f
            );
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
    }
}
