using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Primitives
{
    public interface IPortalDrawable
    {
        void DrawOnLayer(SpriteBatch spriteBatch);
    }
    public class PortalLayer : ModSystem
    {
        /// <summary>
        /// Used to add any sort of drawable object (except projectiles) to the layer
        /// Add the sprite to this on SetDefaults or any initialization
        /// Remove the sprite on Kill
        /// </summary>
        public List<IPortalDrawable> sprites;
        RenderTarget2D target;
        // Please set this to something i have not set it yet
        Texture2D portalTexture;
        public override void Load()
        {
            portalTexture = Mod.Assets.Request<Texture2D>("", AssetRequestMode.ImmediateLoad).Value;
            sprites = new List<IPortalDrawable>();
            Main.QueueMainThreadAction(() =>
            {
                target = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            });
            On.Terraria.Main.DrawProjectiles += DrawTarget;
            Main.OnPreDraw += DrawToTarget;
        }
        public override void Unload()
        {
            Main.OnPreDraw -= DrawToTarget;
            On.Terraria.Main.DrawProjectiles -= DrawTarget;
            target = null;
            sprites = null;
            portalTexture = null;
        }

        private void DrawToTarget(GameTime obj)
        {
            // so the target doesnt move with the player
            Main.screenPosition += Main.LocalPlayer.velocity;

            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice device = spriteBatch.GraphicsDevice;
            RenderTargetBinding[] bindings = device.GetRenderTargets();

            device.SetRenderTarget(target);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, null, null, Main.UIScaleMatrix);
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.ModProjectile is IPortalDrawable)
                {
                    (p.ModProjectile as IPortalDrawable).DrawOnLayer(spriteBatch);
                }
            }
            foreach(var sprite in sprites)
            {
                sprite.DrawOnLayer(spriteBatch);
            }
            spriteBatch.End();

            device.SetRenderTargets(bindings);

            Main.screenPosition -= Main.LocalPlayer.velocity;
        }

        private void DrawTarget(On.Terraria.Main.orig_DrawProjectiles orig, Main self)
        {
            // check if isnt equal null because QueueMainThreadAction isnt always instant
            if (target != null)
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Effect effect = OvermorrowModFile.Instance.Portal.Value;
                effect.Parameters["screenPosition"].SetValue(Main.screenPosition);
                effect.Parameters["screenSize"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                effect.Parameters["portalTexture"].SetValue(portalTexture);
                effect.CurrentTechnique.Passes[0].Apply();
                spriteBatch.Draw(target, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
                spriteBatch.End();
            }
            orig(self);
        }
    }
}
