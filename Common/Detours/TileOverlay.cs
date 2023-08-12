using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core.Interfaces;
using Terraria;
using Terraria.Graphics.Effects;

namespace OvermorrowMod.Common.Detours
{
    // Code adapted from the Starlight River repository
    public class TileOverlay
    {
        public static RenderTarget2D projTarget;
        public static RenderTarget2D tileTarget;

        private Vector2 oldScreenPos;
        private Vector2 offsetOverTime;

        public static void ResizeTarget()
        {
            projTarget?.Dispose();
            tileTarget?.Dispose();

            Main.QueueMainThreadAction(() =>
            {
                projTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                tileTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            });
        }

        public static void Main_Update(Terraria.On_Main.orig_Update orig, Main self, GameTime gameTime)
        {
            if (OvermorrowModFile.Instance != null) OvermorrowModFile.Instance.CheckScreenSize();

            orig(self, gameTime);
        }

        public static void Main_OnPreDraw(GameTime obj)
        {
            if (Main.spriteBatch == null) return;

            if (Main.gameMenu || Main.instance.tileTarget == null || Main.instance.tileTarget.IsDisposed) return;

            GraphicsDevice gD = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;

            RenderTargetBinding[] bindings = gD.GetRenderTargets();

            Vector2 translation = new Vector2(Main.LocalPlayer.velocity.X, Main.LocalPlayer.velocity.Y);

            Vector2 input = new Vector2(Main.leftWorld + 656f, Main.topWorld + 656f) - Main.GameViewMatrix.Translation;

            float xInput = Main.rightWorld - (float)Main.screenWidth / Main.GameViewMatrix.Zoom.X - 672f;
            float yInput = Main.bottomWorld - (float)Main.screenHeight / Main.GameViewMatrix.Zoom.Y - 672f;
            Vector2 input2 = new Vector2(xInput, yInput) - Main.GameViewMatrix.Translation;

            if (Main.screenPosition.X <= input.X || Main.screenPosition.X >= input2.X)
            {
                translation.X = 0;
            }

            if (Main.screenPosition.Y <= input.Y || Main.screenPosition.Y >= input2.Y)
            {
                translation.Y = 0;
            }

            gD.SetRenderTarget(projTarget);
            gD.Clear(Color.Transparent);

            spriteBatch.Begin(
                SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default,
                RasterizerState.CullNone, null,
                Matrix.CreateTranslation(new Vector3(-translation.X, -translation.Y, 0)));

            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.ModProjectile is ITileOverlay iface)
                {
                    iface.DrawOverTiles(spriteBatch);
                }
            }

            spriteBatch.End();

            gD.SetRenderTarget(tileTarget); 
            gD.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null);
            spriteBatch.Draw(Main.instance.tileTarget, Main.sceneTilePos - Main.screenPosition - translation, Color.White);
            spriteBatch.Draw(Main.instance.tile2Target, Main.sceneTile2Pos - Main.screenPosition - translation, Color.White);
            spriteBatch.End();

            gD.SetRenderTargets(bindings);

            //oldScreenPos = Main.screenPosition;
        }

        public static void Main_DrawProjectiles(Terraria.On_Main.orig_DrawProjectiles orig, Main self)
        {
            orig(self);

            if (tileTarget == null || projTarget == null) return;

            Effect effect = OvermorrowModFile.Instance.TileOverlay.Value;
            if (effect is null) return;

            effect.Parameters["TileTarget"].SetValue(tileTarget);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            effect.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(projTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

            Main.spriteBatch.End();
        }
    }
}