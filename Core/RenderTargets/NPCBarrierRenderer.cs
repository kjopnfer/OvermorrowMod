using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.RenderTargets
{
    /// <summary>
    /// Adapted from https://github.com/Trivaxy/JadeFables
    /// </summary>
    class NPCBarrierRenderer : ILoadable
    {
        public static RenderTarget2D barrierRenderTarget;

        public static Vector2 oldScreenPos;

        public void Load(Mod mod)
        {
            if (Main.dedServ)
                return;

            Terraria.On_Main.CheckMonoliths += SetBarrierTargets;
            Terraria.On_Main.DrawNPCs += DrawTarget;
        }

        public void Unload()
        {
            barrierRenderTarget = null;
        }

        private void DrawTarget(Terraria.On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            orig(self, behindTiles);

            var validNPCs = Main.npc.Where(npc => npc.active && npc.GetGlobalNPC<BarrierNPC>().HasBarrier);
            if (Main.gameMenu || validNPCs.Count() == 0)
                return;

            GraphicsDevice gD = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (Main.dedServ || spriteBatch == null || barrierRenderTarget == null || gD == null)
                return;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Effect barrier = OvermorrowModFile.Instance.BarrierShader.Value;
            barrier.Parameters["Time"].SetValue(Main.GameUpdateCount / 60f);
            barrier.Parameters["NoiseSeed"].SetValue((float)(Math.Sin(Main.GameUpdateCount / 60f) * 0.5f + 0.5f));
            barrier.Parameters["Ratio"].SetValue(0.8f);
            barrier.Parameters["TintColor"].SetValue(new Vector4(0.3f, 0.6f, 1.0f, 1f));
            barrier.Parameters["Alpha"].SetValue(0.8f);

            Main.graphics.GraphicsDevice.Textures[1] = OvermorrowModFile.Instance.BarrierNoiseTexture.Value;

            barrier.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Draw(barrierRenderTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

            spriteBatch.End();
            spriteBatch.Begin(default, default, Main.DefaultSamplerState, default, default, default, Main.GameViewMatrix.TransformationMatrix);
        }

        private void SetBarrierTargets(Terraria.On_Main.orig_CheckMonoliths orig)
        {
            orig();

            var validNPCs = Main.npc.Where(npc => npc.active && npc.GetGlobalNPC<BarrierNPC>().HasBarrier);
            if (Main.gameMenu || validNPCs.Count() == 0)
                return;

            var graphics = Main.graphics.GraphicsDevice;

            int RTwidth = Main.screenWidth;
            int RTheight = Main.screenHeight;
            if (barrierRenderTarget is null || barrierRenderTarget.Size() != new Vector2(RTwidth, RTheight))
                barrierRenderTarget = new RenderTarget2D(graphics, RTwidth, RTheight, default, default, default, default, RenderTargetUsage.PreserveContents);

            graphics.SetRenderTarget(barrierRenderTarget);

            graphics.Clear(Color.Transparent);
            Main.spriteBatch.Begin(default, BlendState.Additive, Main.DefaultSamplerState, default, default, default);

            DrawTargets(validNPCs.ToList());

            Main.spriteBatch.End();
            graphics.SetRenderTarget(null);
        }

        private void DrawTargets(List<NPC> validNPCs)
        {
            foreach (NPC npc in validNPCs)
            {
                bool shouldDraw = true;

                if (npc.ModNPC is OvermorrowNPC overmorrowNPC)
                {
                    // This is different because OvermorrowNPC has draw hooks that shouldn't be captured.
                    shouldDraw = overmorrowNPC.DrawOvermorrowNPC(Main.spriteBatch, Main.screenPosition, npc.GetAlpha(Color.White));
                    overmorrowNPC.PostDraw(Main.spriteBatch, Main.screenPosition, npc.GetAlpha(Color.White));
                }
                else if (npc.ModNPC is ModNPC modNPC)
                {
                    shouldDraw = modNPC.PreDraw(Main.spriteBatch, Main.screenPosition, npc.GetAlpha(Color.White));
                    modNPC.PostDraw(Main.spriteBatch, Main.screenPosition, npc.GetAlpha(Color.White));
                }

                if (shouldDraw)
                {
                    Main.instance.DrawNPC(npc.whoAmI, false);
                }
            }
        }
    }
}