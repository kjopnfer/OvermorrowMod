using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core.Interfaces;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.RenderTargets
{
    class OutlineRenderer : ILoadable
    {
        public static RenderTarget2D outlineRenderTarget;
        public static RenderTarget2D processedRenderTarget;

        public float Priority => 1.1f;

        public void Load(Mod mod)
        {
            if (Main.dedServ)
                return;

            Terraria.On_Main.DrawNPCs += DrawOutlines;
            Terraria.On_Main.CheckMonoliths += BuildOutlineTargets;
        }

        public void Unload()
        {
            outlineRenderTarget?.Dispose();
            processedRenderTarget?.Dispose();
            outlineRenderTarget = null;
            processedRenderTarget = null;
        }

        private void DrawOutlines(Terraria.On_Main.orig_DrawNPCs orig, Main self, bool behindTiles = false)
        {
            orig(self, behindTiles);

            if (!behindTiles && processedRenderTarget != null)
            {
                DrawFinalOutlines();
            }
        }

        private void DrawFinalOutlines()
        {
            if (Main.dedServ || Main.spriteBatch == null || processedRenderTarget == null)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone,
                null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(processedRenderTarget, Vector2.Zero, Color.White);

            spriteBatch.End();
            spriteBatch.Begin(default, default, Main.DefaultSamplerState, default, default, default, Main.GameViewMatrix.TransformationMatrix);
        }

        private void BuildOutlineTargets(Terraria.On_Main.orig_CheckMonoliths orig)
        {
            orig();

            if (Main.gameMenu || Main.spriteBatch == null || Main.graphics.GraphicsDevice == null)
                return;

            var gD = Main.graphics.GraphicsDevice;

            int RTwidth = Main.screenWidth;
            int RTheight = Main.screenHeight;

            if (outlineRenderTarget is null || outlineRenderTarget.Size() != new Vector2(RTwidth, RTheight))
            {
                outlineRenderTarget?.Dispose();
                outlineRenderTarget = new RenderTarget2D(gD, RTwidth, RTheight, default, default, default, default, RenderTargetUsage.PreserveContents);
            }

            if (processedRenderTarget is null || processedRenderTarget.Size() != new Vector2(RTwidth, RTheight))
            {
                processedRenderTarget?.Dispose();
                processedRenderTarget = new RenderTarget2D(gD, RTwidth, RTheight, default, default, default, default, RenderTargetUsage.PreserveContents);
            }

            var validProjectiles = Main.projectile.Where(proj => proj.active && ShouldDrawOutline(proj)).ToList();
            var validNPCs = Main.npc.Where(npc => npc.active && ShouldDrawOutline(npc)).ToList();

            gD.SetRenderTarget(outlineRenderTarget);
            gD.Clear(Color.Transparent);

            if (validProjectiles.Count > 0 || validNPCs.Count > 0)
            {
                Main.spriteBatch.Begin(default, BlendState.AlphaBlend, Main.DefaultSamplerState, default, default, default);

                foreach (Projectile proj in validProjectiles)
                {
                    DrawProjectileToTarget(proj);
                }

                foreach (NPC npc in validNPCs)
                {
                    DrawNPCToTarget(npc);
                }

                Main.spriteBatch.End();

                gD.SetRenderTarget(processedRenderTarget);
                gD.Clear(Color.Transparent);

                Effect outline = OvermorrowModFile.Instance.Outline.Value;
                if (outline != null)
                {
                    var firstEntity = validProjectiles.FirstOrDefault() as Entity ?? validNPCs.FirstOrDefault() as Entity;
                    var outlineInterface = GetOutlineInterface(firstEntity);

                    outline.Parameters["PixelSize"].SetValue(new Vector2(1f / RTwidth, 1f / RTheight));
                    outline.Parameters["OutlineColor"].SetValue((outlineInterface?.OutlineColor ?? Color.White).ToVector4());
                    outline.Parameters["FillColor"].SetValue((outlineInterface?.FillColor ?? Color.White).ToVector4());
                    outline.Parameters["UseFillColor"].SetValue(outlineInterface?.UseFillColor ?? true);

                    Main.spriteBatch.Begin(default, default, Main.DefaultSamplerState, default, RasterizerState.CullNone, outline);
                    Main.spriteBatch.Draw(outlineRenderTarget, Vector2.Zero, Color.White);
                    Main.spriteBatch.End();
                }
            }
            else
            {
                // If no entities, clear the processed target too
                gD.SetRenderTarget(processedRenderTarget);
                gD.Clear(Color.Transparent);
            }

            gD.SetRenderTarget(null);
        }

        private void DrawProjectileToTarget(Projectile proj)
        {
            if (proj.ModProjectile != null)
            {
                Texture2D texture = ModContent.Request<Texture2D>(proj.ModProjectile.Texture).Value;

                Vector2 position = proj.Center - Main.screenPosition;
                Vector2 origin = texture.Size() / 2f;

                Main.spriteBatch.Draw(texture, position, null, Color.White, proj.rotation, origin, proj.scale, SpriteEffects.None, 0f);
            }
        }

        private void DrawNPCToTarget(NPC npc)
        {
            if (npc.type > 0 && npc.type < Terraria.ID.NPCID.Count)
            {
                Texture2D texture = Terraria.GameContent.TextureAssets.Npc[npc.type].Value;

                Vector2 position = npc.Center - Main.screenPosition;
                Vector2 origin = texture.Size() / 2f;

                Main.spriteBatch.Draw(texture, position, null, Color.White, npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);
            }
        }

        private bool ShouldDrawOutline(Entity entity)
        {
            return entity switch
            {
                NPC npc when npc.ModNPC is IOutlineEntity outline => outline.ShouldDrawOutline,
                Projectile proj when proj.ModProjectile is IOutlineEntity outline => outline.ShouldDrawOutline,
                _ => false
            };
        }

        private IOutlineEntity GetOutlineInterface(Entity entity)
        {
            return entity switch
            {
                NPC npc => npc.ModNPC as IOutlineEntity,
                Projectile proj => proj.ModProjectile as IOutlineEntity,
                _ => null
            };
        }
    }
}