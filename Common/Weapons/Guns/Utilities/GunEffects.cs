using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Particles;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Weapons.Guns
{
    public static class GunEffects
    {
        // This shit doesnt fucking work because gore is stupid. These are actually in the HeldGun.Helpers file.
        /*public static void SpawnBulletCasing(Projectile projectile, Player player, Vector2 position,
            Vector2 offset = default, float scale = 0.75f, bool sticky = true)
        {
            Vector2 velocity = new Vector2(player.direction * -0.03f, 0.01f);
            int gore = Gore.NewGore(null, position + offset, velocity, Mod.Find<ModGore>("BulletCasing").Type, scale);

            Main.gore[gore].sticky = sticky;
        }*/

        /// <summary>
        /// Drops multiple bullet casings at once, used for reload effects
        /// </summary>
        /*public static void DropMultipleCasings(Projectile projectile, Player player, int count)
        {
            for (int i = 0; i < count; i++)
            {
                SpawnBulletCasing(projectile, player, projectile.Center);
            }
        }*/

        /// <summary>
        /// Draws a standard muzzle flash for guns
        /// </summary>
        public static void DrawMuzzleFlash(SpriteBatch spriteBatch, Projectile projectile, Player player,
            Vector2 muzzleOffset, float scale = 0.05f, Color? primaryColor = null, Color? secondaryColor = null,
            string texture = "muzzle_05")
        {
            Vector2 directionOffset = player.direction == -1 ? new Vector2(0, -10) : Vector2.Zero;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState,
                DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D muzzleFlash = ModContent.Request<Texture2D>(AssetDirectory.Textures + texture).Value;
            Vector2 offset = projectile.Center + directionOffset + muzzleOffset.RotatedBy(projectile.rotation);
            var rotationSpriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color primary = primaryColor ?? Color.Red * 0.85f;
            Color secondary = secondaryColor ?? Color.Orange * 0.6f;

            spriteBatch.Draw(muzzleFlash, offset - Main.screenPosition, null, primary,
                projectile.rotation + MathHelper.PiOver2, muzzleFlash.Size() / 2f, scale, rotationSpriteEffects, 1);
            spriteBatch.Draw(muzzleFlash, offset - Main.screenPosition, null, secondary,
                projectile.rotation + MathHelper.PiOver2, muzzleFlash.Size() / 2f, scale, rotationSpriteEffects, 1);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState,
                DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }

        /// <summary>
        /// Creates smoke particles for gunfire effects
        /// </summary>
        public static void CreateSmoke(Vector2 position, Vector2 velocity, int count = 8,
            float spreadAngle = 40f, float velocityScale = 0.2f, Color? color = null)
        {
            var smokeTextures = new Texture2D[] {
                ModContent.Request<Texture2D>("OvermorrowMod/Assets/Textures/smoke_01", AssetRequestMode.ImmediateLoad).Value,
                ModContent.Request<Texture2D>("OvermorrowMod/Assets/Textures/smoke_02", AssetRequestMode.ImmediateLoad).Value,
                ModContent.Request<Texture2D>("OvermorrowMod/Assets/Textures/smoke_03", AssetRequestMode.ImmediateLoad).Value,
                ModContent.Request<Texture2D>("OvermorrowMod/Assets/Textures/smoke_04", AssetRequestMode.ImmediateLoad).Value,
                ModContent.Request<Texture2D>("OvermorrowMod/Assets/Textures/smoke_05", AssetRequestMode.ImmediateLoad).Value,
                ModContent.Request<Texture2D>("OvermorrowMod/Assets/Textures/smoke_06", AssetRequestMode.ImmediateLoad).Value,
                ModContent.Request<Texture2D>("OvermorrowMod/Assets/Textures/smoke_07", AssetRequestMode.ImmediateLoad).Value
            };

            Color smokeColor = color ?? Color.DarkGray;
            for (int i = 0; i < count; i++)
            {
                Vector2 particleVelocity = (velocity * Main.rand.NextFloat(0.025f, velocityScale))
                    .RotatedByRandom(MathHelper.ToRadians(spreadAngle));

                var smoke = new Gas(smokeTextures, 30f, scaleOverride: Main.rand.NextFloat(0.2f, 0.5f)) {
                    gasBehavior = GasBehavior.Grow,
                    driftsUpward = true,
                    rotatesOverTime = true,
                    scaleRate = 0.005f,
                    customAlpha = 0.5f
                };

                ParticleManager.CreateParticleDirect(smoke, position, particleVelocity, smokeColor, 1f, scale: 0.025f, 0f);
            }
        }
    }
}