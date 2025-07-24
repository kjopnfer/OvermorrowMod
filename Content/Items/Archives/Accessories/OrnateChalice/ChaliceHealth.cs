using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Particles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class ChaliceHealth : ModProjectile, IOutlineEntity
    {
        public override string Texture => AssetDirectory.ArchiveProjectiles + Name;

        public bool ShouldDrawOutline => true;
        public Color OutlineColor => Color.DarkRed;
        public bool UseFillColor => true;
        public Texture2D FillTexture => null;
        public Color? FillColor => Color.Black;
        public Action<SpriteBatch, GraphicsDevice, int, int> SharedGroupDrawFunction => null;
        public Action<SpriteBatch, GraphicsDevice, Entity> IndividualEntityDrawFunction => null;

        public override void SetStaticDefaults()
        {
            Projectile.width = Projectile.height = 28;
            Projectile.timeLeft = ModUtils.SecondsToTicks(5);
            Projectile.tileCollide = true;
        }

        private float baseScale;
        public override void OnSpawn(IEntitySource source)
        {
            //baseScale = Main.rand.NextFloat(f, 2f);
            baseScale = Main.rand.NextFloat(0.2f, 0.6f);
            Projectile.scale = baseScale;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.025f;
            Projectile.rotation -= 0.02f;

            var outlineParticle = new OutlineParticle(AssetDirectory.ArchiveProjectiles + Name, 16, 16)
            {
                ShouldDrawOutline = true,
                OutlineColor = Color.Purple,
                FillColor = Color.Black,
                MaxLifetime = ModUtils.SecondsToTicks(1)
            };

            ParticleManager.CreateParticleDirect(outlineParticle, Projectile.Center, -Vector2.Normalize(Projectile.velocity), Color.White, 1f, Main.rand.NextFloat(0.2f, 0.5f));


            if (Projectile.Hitbox.Intersects(Main.LocalPlayer.Hitbox))
                Projectile.Kill();
            //float pulsateSpeed = 0.08f;
            //float pulsateAmount = 0.3f;

            //float pulsate = 1f + (float)Math.Sin(Projectile.timeLeft * pulsateSpeed) * pulsateAmount;
            //Projectile.scale = baseScale * pulsate;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_06").Value;
            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texture.Size() / 2f, 0.75f, SpriteEffects.None, 0);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveProjectiles + Name).Value;
            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        private void DrawSharedBackground(SpriteBatch spriteBatch, GraphicsDevice gD, int screenWidth, int screenHeight)
        {
            // Draw the continuous tiled background once for the entire group
            Texture2D backgroundTexture = ModContent.Request<Texture2D>(AssetDirectory.MapBackgrounds + "GrandArchives").Value;

            int tilesX = (screenWidth / backgroundTexture.Width) + 2;
            int tilesY = (screenHeight / backgroundTexture.Height) + 2;

            // Parallax factor - lower values = slower background movement
            float parallaxFactor = 0.5f; // Background moves at half camera speed

            Vector2 offset = new Vector2(
                (Main.screenPosition.X * parallaxFactor) % backgroundTexture.Width,
                (Main.screenPosition.Y * parallaxFactor) % backgroundTexture.Height
            );

            for (int x = -1; x < tilesX; x++)
            {
                for (int y = -1; y < tilesY; y++)
                {
                    Vector2 position = new Vector2(x * backgroundTexture.Width, y * backgroundTexture.Height) - offset;
                    spriteBatch.Draw(backgroundTexture, position, Color.White);
                }
            }
        }

        private void DrawEntityRat(SpriteBatch spriteBatch, GraphicsDevice gD, Entity entity)
        {
            // Draw the animated rat specific to this entity
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "ArchiveRat").Value;

            int yFrame = (int)((Main.GameUpdateCount / 6) % 9);
            var frame = new Rectangle(0, yFrame * (texture.Height / 9), texture.Width / 10, texture.Height / 9);

            // Entity-specific positioning with drift
            float time = Main.GameUpdateCount * 0.02f + entity.whoAmI * 0.5f;
            Vector2 driftOffset = new Vector2(
                (float)Math.Sin(time) * 120f,
                (float)Math.Cos(time * 0.7f) * 120f
            );

            // Add a small parallax offset to the drift itself
            float parallaxAmount = 20f; // Small offset amount
            Vector2 parallaxDrift = new Vector2(
                (float)Math.Sin(time * 0.3f) * parallaxAmount,
                (float)Math.Cos(time * 0.2f) * parallaxAmount
            );

            Vector2 ratPosition = entity.Center - Main.screenPosition + driftOffset + parallaxDrift;

            spriteBatch.Draw(texture, ratPosition, frame, Color.White, 0f, frame.Size() / 2, 1f, SpriteEffects.None, 0);
        }
    }
}