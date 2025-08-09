using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core.Effects;
using OvermorrowMod.Core.Effects.Slash;
using System.Collections.Generic;
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
            Projectile.timeLeft = 300; // 5 seconds
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        private SlashPath slashPath;
        private bool pathInitialized = false;
        private List<VertexPositionColorTexture> slashVertices;

        public override void AI()
        {
            if (!pathInitialized)
            {
                // Initialize the slash path on first frame
                InitializeSlashPath();
                pathInitialized = true;
            }
        }

        private void InitializeSlashPath()
        {
            Vector2 center = Projectile.Center;
            float radiusX = 80f;
            float radiusY = 40f;
            float ellipseRotation = Projectile.rotation + MathHelper.PiOver4;
            float startAngle = -MathHelper.PiOver2;
            float endAngle = MathHelper.PiOver2 * 2;

            slashPath = new SlashPath(center, radiusX, radiusY, ellipseRotation, startAngle, endAngle);

            // Generate mesh vertices
            float slashWidth = 40f;
            int segments = 30;
            Color slashColor = Color.White;

            slashVertices = SlashMeshGenerator.GenerateSlashMesh(slashPath, slashWidth, segments, slashColor, SpriteEffects.FlipVertically);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (pathInitialized && slashVertices != null)
            {
                //Texture2D whiteTexture = TextureAssets.MagicPixel.Value;
                Texture2D whiteTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "SwordTrails/Dissolved").Value;
                PrimitiveRenderer.Draw(Main.spriteBatch, slashVertices, whiteTexture);

                //SlashDebugRenderer.DrawPath(Main.spriteBatch, slashPath, segments: 30);
            }

            return false;
        }
    }
}