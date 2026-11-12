using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Misc
{
    public class SpawnTelegraph : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        private float SpinDirection => Projectile.ai[0] >= 0f ? 1f : -1f;
        private int Duration => Projectile.ai[1] > 0 ? (int)Projectile.ai[1] : 70;


        private const int GrowTicks = 20;
        private const int FadeOutTicks = 8;
        private const float SpinSpeed = 0.045f;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = Duration;
        }

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.rotation += SpinSpeed * SpinDirection;
            Lighting.AddLight(Projectile.Center, 0.4f, 0.05f, 0.05f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D outerRing = ModContent.Request<Texture2D>(AssetDirectory.Textures + "magic_circle_01").Value;
            Texture2D innerRing = ModContent.Request<Texture2D>(AssetDirectory.Textures + "magic_circle_02").Value;

            float outerDiameter = 101f;
            float innerDiameter = 66f;

            int elapsed = Duration - Projectile.timeLeft;
            float growProgress = MathHelper.Clamp(elapsed / (float)GrowTicks, 0f, 1f);
            float grow = EasingUtils.EaseOutBack(growProgress);

            float alpha = growProgress;
            if (Projectile.timeLeft < FadeOutTicks)
                alpha *= Projectile.timeLeft / (float)FadeOutTicks;

            Color color = new Color(255, 60, 60) * alpha;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            float outerScale = outerDiameter / outerRing.Width * grow;
            float innerScale = innerDiameter / innerRing.Width * grow;

            Main.spriteBatch.Draw(outerRing, drawPos, null, color, Projectile.rotation, outerRing.Size() / 2f, outerScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(innerRing, drawPos, null, color, -Projectile.rotation, innerRing.Size() / 2f, innerScale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
