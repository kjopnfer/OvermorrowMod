using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core.Effects.Slash;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Test
{
    public class TestSlashProjectile : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        private SlashRenderer slashRenderer;
        private bool initialized = false;

        public override void AI()
        {
            if (!initialized)
            {
                InitializeLayeredSlash();
                initialized = true;
            }
        }

        private void InitializeLayeredSlash()
        {
            Vector2 center = Projectile.Center;
            float radiusX = 80f;
            float radiusY = 80f;
            float ellipseRotation = Projectile.rotation + MathHelper.PiOver4;
            float startAngle = -MathHelper.PiOver2;
            float endAngle = MathHelper.PiOver2 * 2;

            SlashPath path = new SlashPath(center, radiusX, radiusY, ellipseRotation, startAngle, endAngle);

            slashRenderer = new SlashRenderer(path, baseWidth: 35f, segments: 40);
            SetupSlashLayers();
        }

        private void SetupSlashLayers()
        {
            Texture2D baseTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "SwordTrails/Blob").Value;
            slashRenderer.AddLayer(new SlashLayer(
                baseTexture,
                new Color(50, 100, 255),
                widthScale: 1f,
                opacity: 1f,
                BlendState.AlphaBlend
            ));

            Texture2D highlightTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "SwordTrails/Dissolved").Value;
            slashRenderer.AddLayer(new SlashLayer(
                highlightTexture,
                new Color(255, 255, 200),
                widthScale: 1f,
                opacity: 0.8f,
                BlendState.Additive
            ));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (initialized && slashRenderer != null)
            {
                slashRenderer.Draw(Main.spriteBatch);

                // debug visualization
                // SlashDebugRenderer.DrawPath(Main.spriteBatch, slashRenderer.Path, segments: 30);
            }

            return false;
        }
    }
}