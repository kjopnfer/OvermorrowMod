using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Detours;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Content.NPCs.Carts;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public static class ModDetours
    {
        public static void Load()
        {
            #region Hide UI
            Terraria.On_Main.DrawInterface_36_Cursor += DrawInterface_36_Cursor;

            //On.Terraria.Main.DrawNPCChatButtons += DrawNPCChatButtons;
            /*On.Terraria.GameContent.UI.ResourceSets.PlayerResourceSetsManager.Draw += Draw;
            On.Terraria.Main.DrawHealthBar += DrawHealthBar;
            On.Terraria.Main.DrawInterface_Resources_Buffs += DrawInterface_Resources_Buffs;
            On.Terraria.Main.DrawInterface_16_MapOrMinimap += DrawInterface_16_MapOrMinimap;
            On.Terraria.Main.DrawInterface_25_ResourceBars += DrawInterface_25_ResourceBars;
            On.Terraria.Main.DrawInterface_30_Hotbar += DrawInterface_30_Hotbar;*/
            #endregion

            #region Trails
            PrimitiveDrawing.trails = new List<Trail>();

            Terraria.On_NPC.NewNPC += PrimitiveDrawing.CreateNPCTrail;
            Terraria.On_Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float += PrimitiveDrawing.CreateProjectileTrail;

            Terraria.On_Main.DrawNPCs += PrimitiveDrawing.DrawNPCTrails;
            Terraria.On_Main.DrawProjectiles += PrimitiveDrawing.DrawProjectileTrails;

            Terraria.On_NPC.NPCLoot += PrimitiveDrawing.NPCLoot;
            Terraria.On_Projectile.Kill += PrimitiveDrawing.Kill;
            #endregion

            Terraria.On_Player.Update_NPCCollision += CustomCollision.Player_UpdateNPCCollision;
            Terraria.On_Player.SlopingCollision += CustomCollision.Player_PlatformCollision;
            Terraria.On_Main.DrawInterface += ParticleDrawing.DrawParticles;
            Terraria.On_Main.DrawDust += DrawOverlay;
            Terraria.On_Main.GUIChatDrawInner += DialogueOverrides.GUIChatDrawInner;
            Terraria.On_Player.SetTalkNPC += SetTalkNPC;

            Terraria.Graphics.Effects.On_FilterManager.EndCapture += FilterManager_EndCapture;
            Main.OnResolutionChanged += Main_OnResolutionChanged;
            OvermorrowModFile.Instance.CreateRender();

            TileOverlay.ResizeTarget();

            Terraria.On_Main.DrawProjectiles += TileOverlay.Main_DrawProjectiles;
            On_Main.CacheNPCDraws += Detours.BackgroundObjects.Main_DrawBackgroundObjects;
            //On.Terraria.Main.DrawBackground += Detours.BackgroundObjects.Main_DrawBackgroundObjects;
            Main.OnPreDraw += TileOverlay.Main_OnPreDraw;
            Terraria.On_Main.Update += TileOverlay.Main_Update;
        }

        public static void Unload()
        {
            #region Hide UI
            Terraria.On_Main.DrawInterface_36_Cursor -= DrawInterface_36_Cursor;

            //On.Terraria.Main.DrawNPCChatButtons -= DrawNPCChatButtons;
            /*On.Terraria.GameContent.UI.ResourceSets.PlayerResourceSetsManager.Draw -= Draw;

            On.Terraria.Main.DrawHealthBar -= DrawHealthBar;
            On.Terraria.Main.DrawInterface_Resources_Buffs -= DrawInterface_Resources_Buffs;
            On.Terraria.Main.DrawInterface_16_MapOrMinimap -= DrawInterface_16_MapOrMinimap;
            On.Terraria.Main.DrawInterface_25_ResourceBars -= DrawInterface_25_ResourceBars;
            On.Terraria.Main.DrawInterface_30_Hotbar -= DrawInterface_30_Hotbar;*/
            #endregion

            #region Trails
            Terraria.On_Projectile.Kill -= PrimitiveDrawing.Kill;
            Terraria.On_NPC.NPCLoot -= PrimitiveDrawing.NPCLoot;

            Terraria.On_Main.DrawProjectiles -= PrimitiveDrawing.DrawProjectileTrails;
            Terraria.On_Main.DrawNPCs -= PrimitiveDrawing.DrawNPCTrails;

            Terraria.On_Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float -= PrimitiveDrawing.CreateProjectileTrail;
            Terraria.On_NPC.NewNPC -= PrimitiveDrawing.CreateNPCTrail;

            PrimitiveDrawing.trails = null;
            #endregion

            Terraria.On_Player.Update_NPCCollision -= CustomCollision.Player_UpdateNPCCollision;
            Terraria.On_Player.SlopingCollision -= CustomCollision.Player_PlatformCollision;
            Terraria.On_Main.DrawInterface -= ParticleDrawing.DrawParticles;
            Terraria.On_Main.DrawDust -= DrawOverlay;
            Terraria.On_Main.GUIChatDrawInner -= DialogueOverrides.GUIChatDrawInner;
            Terraria.On_Player.SetTalkNPC -= SetTalkNPC;

            Terraria.Graphics.Effects.On_FilterManager.EndCapture -= FilterManager_EndCapture;
            Main.OnResolutionChanged -= Main_OnResolutionChanged;

            Terraria.On_Main.DrawProjectiles -= TileOverlay.Main_DrawProjectiles;
            Terraria.On_Main.CacheNPCDraws -= Detours.BackgroundObjects.Main_DrawBackgroundObjects;
            Terraria.On_Main.DrawProjectiles -= TileOverlay.Main_DrawProjectiles;
            //On.Terraria.Main.DrawBackground -= Detours.BackgroundObjects.Main_DrawBackgroundObjects;
            Main.OnPreDraw -= TileOverlay.Main_OnPreDraw;
            Terraria.On_Main.Update -= TileOverlay.Main_Update;

            TileOverlay.projTarget = null;
        }

        private static void DrawInterface_36_Cursor(Terraria.On_Main.orig_DrawInterface_36_Cursor orig)
        {
            if (OvermorrowModSystem.Instance.ScreenColor.IsVisible())
            {
                return;
            }

            orig();
        }

        private static void SetTalkNPC(Terraria.On_Player.orig_SetTalkNPC orig, Player self, int npcIndex, bool fromNet)
        {
            if (npcIndex == ModContent.NPCType<Cart>())
            {
                self.currentShoppingSettings.HappinessReport = "";
            }

            orig.Invoke(self, npcIndex, fromNet);
        }

        private static void Draw(Terraria.GameContent.UI.ResourceSets.On_PlayerResourceSetsManager.orig_Draw orig, Terraria.GameContent.UI.ResourceSets.PlayerResourceSetsManager self)
        {
            foreach (NPC npc in Main.npc) if (npc.type == NPCID.EyeofCthulhu && npc.active) return;

            orig(self);
        }

        private static void DrawHealthBar(Terraria.On_Main.orig_DrawHealthBar orig, Main self, float X, float Y, int Health, int MaxHealth, float alpha, float scale, bool noFlip)
        {
            foreach (NPC npc in Main.npc) if (npc.type == NPCID.EyeofCthulhu && npc.active) return;

            orig(self, X, Y, Health, MaxHealth, alpha, scale, noFlip);
        }

        private static void DrawInterface_Resources_Buffs(Terraria.On_Main.orig_DrawInterface_Resources_Buffs orig, Main self)
        {
            foreach (NPC npc in Main.npc) if (npc.type == NPCID.EyeofCthulhu && npc.active) return;

            orig(self);
        }

        private static void DrawInterface_16_MapOrMinimap(Terraria.On_Main.orig_DrawInterface_16_MapOrMinimap orig, Main self)
        {
            foreach (NPC npc in Main.npc) if (npc.type == NPCID.EyeofCthulhu && npc.active) return;

            orig(self);
        }

        private static void DrawInterface_25_ResourceBars(Terraria.On_Main.orig_DrawInterface_25_ResourceBars orig, Main self)
        {
            foreach (NPC npc in Main.npc) if (npc.type == NPCID.EyeofCthulhu && npc.active) return;

            orig(self);
        }

        private static void DrawInterface_30_Hotbar(Terraria.On_Main.orig_DrawInterface_30_Hotbar orig, Main self)
        {
            foreach (NPC npc in Main.npc) if (npc.type == NPCID.EyeofCthulhu && npc.active) return;

            orig(self);
        }

        private static void Main_OnResolutionChanged(Vector2 obj)
        {
            OvermorrowModFile.Instance.CreateRender();
        }

        private static void FilterManager_EndCapture(Terraria.Graphics.Effects.On_FilterManager.orig_EndCapture orig, Terraria.Graphics.Effects.FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {
            // Swap Render Target
            // Set New Render Target
            // Swap Back to screenTarget

            GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;

            //gd.SetRenderTarget(Main.screenTargetSwap);
            //gd.Clear(Color.Transparent);
            //sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            //sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            //sb.End();

            //gd.SetRenderTarget(OvermorrowModFile.Instance.Render);
            //gd.Clear(Color.Transparent);
            //sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //CosmicFlame.DrawAll(sb);
            //sb.End();

            graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            graphicsDevice.SetRenderTarget(OvermorrowModFile.Instance.Render);
            graphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //sb.Draw(TextureAssets.MagicPixel.Value, new Vector2(800, 500), new Rectangle(0, 0, 50, 50), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            //DarkVortex.DrawAll(spriteBatch);
            spriteBatch.End();

            /*graphicsDevice.SetRenderTarget(Main.screenTarget);
            graphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            //graphicsDevice.Textures[1] = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Cosmic").Value;
            graphicsDevice.Textures[1] = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/Test").Value;
            OvermorrowModFile.BigTentacle.CurrentTechnique.Passes[0].Apply();
            OvermorrowModFile.BigTentacle.Parameters["m"].SetValue(0.62f);
            OvermorrowModFile.BigTentacle.Parameters["n"].SetValue(0.01f);
            spriteBatch.Draw(OvermorrowModFile.Instance.Render, Vector2.Zero, Color.White);
            spriteBatch.End();*/

            orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
        }

        // Test for blurred overlay
        private static void DrawOverlay(Terraria.On_Main.orig_DrawDust orig, Main self)
        {
            var effect = OvermorrowModFile.Instance.RadialBlur.Value;
            var texture = OvermorrowModFile.Instance.BlurTestTexture.Value;

            var data = Primitives.PrimitiveHelper.GetRectangleStrip(new Rectangle(Main.spawnTileX * 16 - 200, Main.spawnTileY * 16 - 200, 400, 400));

            effect.SafeSetParameter("WVP", Primitives.PrimitiveHelper.GetMatrix());
            effect.SafeSetParameter("img0", texture);
            effect.SafeSetParameter("blurOrigin", new Vector2(Main.screenWidth / 2, Main.screenHeight / 2));
            // Relevant parameters. Radius is radius in pixels of the blur effect, no blur beyond that.
            effect.SafeSetParameter("blurRadius", 100);
            // Intensity is the "hardness" parameter of the blur. Distance to origin is converted to a number between 0 and 1, then
            // this number is applied as an exponent. Higher is harder.
            effect.SafeSetParameter("blurIntensity", 4);
            effect.CurrentTechnique.Passes["Blur"].Apply();

            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;
            device.Textures[0] = texture;

            //device.DrawUserPrimitives(PrimitiveType.TriangleStrip, data, 0, 4);

            orig(self);
        }
    }
}
