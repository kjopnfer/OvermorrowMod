using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using On.Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Rendering
{
    public class RenderingSystem : ModSystem
    {
        public static Dictionary<Type, Renderer> renderers;
        private HashSet<Renderer> disabledRenderers;

        private string GetRendererName(Renderer renderer)
        {
            return renderer.GetType().Name;
        }

        public static Renderer GetRenderer<T>() where T : Renderer
        {
            return renderers[typeof(T)];
        }

        public override void Load()
        {
            renderers = new Dictionary<Type, Renderer>();
            disabledRenderers = new HashSet<Renderer>();
            foreach (Type type in Mod.Code.GetTypes())
                if (type != typeof(Renderer) && !type.IsAbstract && type.IsSubclassOf(typeof(Renderer)))
                    renderers.Add(type, (Renderer)Activator.CreateInstance(type));

            foreach (Renderer renderer in renderers.Values.Where(renderer => renderer.UsesTargets))
                Terraria.Main.QueueMainThreadAction(() => renderer.PrepareTargets(Terraria.Main.graphics.GraphicsDevice));
            Main.DoDraw += DrawTargets;
            Terraria.Main.OnResolutionChanged += RecreateTargets;
            Main.DrawBlack += PreTiles;
            Main.DrawNPCs += PreNPCs;
            Main.DrawProjectiles += PreProjectiles;
            Main.DrawInterface += PreInterface;
        }

        private void PreInterface(Main.orig_DrawInterface orig, Terraria.Main self, GameTime gametime)
        {
            foreach (Renderer renderer in renderers.Values.Where(renderer =>
                         renderer.DrawLayer == DrawLayer.PreInterface && !disabledRenderers.Contains(renderer)))
                try
                {
                    renderer.DrawToScreen();
                }
                catch (Exception e)
                {
                    disabledRenderers.Add(renderer);
                    Terraria.Main.NewText("Error while drawing target in: " + GetRendererName(renderer));
                    OvermorrowModFile.Instance.Logger.Error(e.Message);
                    OvermorrowModFile.Instance.Logger.Error(e.StackTrace);
                }

            orig(self, gametime);
        }


        private void PreProjectiles(Main.orig_DrawProjectiles orig, Terraria.Main self)
        {
            foreach (Renderer renderer in renderers.Values.Where(renderer =>
                         renderer.DrawLayer == DrawLayer.PreProjectiles && !disabledRenderers.Contains(renderer)))
                try
                {
                    renderer.DrawToScreen();
                }
                catch (Exception e)
                {
                    disabledRenderers.Add(renderer);
                    Terraria.Main.NewText("Error while drawing target in: " + GetRendererName(renderer));
                    OvermorrowModFile.Instance.Logger.Error(e.Message);
                    OvermorrowModFile.Instance.Logger.Error(e.StackTrace);
                }

            orig(self);
        }

        private void PreNPCs(Main.orig_DrawNPCs orig, Terraria.Main self, bool behindtiles)
        {
            BeginData data = BeginData.GetData(Terraria.Main.spriteBatch);
            Terraria.Main.spriteBatch.End();
            foreach (Renderer renderer in renderers.Values.Where(renderer =>
                         renderer.DrawLayer == DrawLayer.PreNPCs && !disabledRenderers.Contains(renderer)))
                try
                {
                    renderer.DrawToScreen();
                }
                catch (Exception e)
                {
                    disabledRenderers.Add(renderer);
                    Terraria.Main.NewText("Error while drawing target in: " + GetRendererName(renderer));
                    OvermorrowModFile.Instance.Logger.Error(e.Message);
                    OvermorrowModFile.Instance.Logger.Error(e.StackTrace);
                }

            data.Apply(Terraria.Main.spriteBatch);
            orig(self, behindtiles);
        }

        private void PreTiles(Main.orig_DrawBlack orig, Terraria.Main self, bool force)
        {
            BeginData data = BeginData.GetData(Terraria.Main.spriteBatch);
            Terraria.Main.spriteBatch.End();
            foreach (Renderer renderer in renderers.Values.Where(renderer =>
                         renderer.DrawLayer == DrawLayer.PreTiles && !disabledRenderers.Contains(renderer)))
                try
                {
                    renderer.DrawToScreen();
                }
                catch (Exception e)
                {
                    disabledRenderers.Add(renderer);
                    Terraria.Main.NewText("Error while drawing target in: " + GetRendererName(renderer));
                    OvermorrowModFile.Instance.Logger.Error(e.Message);
                    OvermorrowModFile.Instance.Logger.Error(e.StackTrace);
                }

            data.Apply(Terraria.Main.spriteBatch);
            orig(self, force);
        }

        private void RecreateTargets(Vector2 obj)
        {
            foreach (Renderer renderer in renderers.Values.Where(renderer => renderer.UsesTargets))
                Terraria.Main.QueueMainThreadAction(() => renderer.PrepareTargets(Terraria.Main.graphics.GraphicsDevice));
        }

        private void DrawTargets(Main.orig_DoDraw orig, Terraria.Main self, GameTime gametime)
        {
            if (!Terraria.Main.gamePaused)
                Terraria.Main.screenPosition += Terraria.Main.LocalPlayer.velocity;
            foreach (Renderer renderer in renderers.Values.Where(renderer =>
                         renderer.UsesTargets && !disabledRenderers.Contains(renderer)))
                try
                {
                    renderer.DrawToTarget(Terraria.Main.graphics.GraphicsDevice, Terraria.Main.spriteBatch);
                }
                catch (Exception e)
                {
                    disabledRenderers.Add(renderer);
                    Terraria.Main.NewText("Error while drawing data to target in: " + GetRendererName(renderer));
                    Terraria.Main.NewText("Check client.log for more information.");
                    OvermorrowModFile.Instance.Logger.Error(e.Message);
                    OvermorrowModFile.Instance.Logger.Error(e.StackTrace);
                }

            if (!Terraria.Main.gamePaused)
                Terraria.Main.screenPosition -= Terraria.Main.LocalPlayer.velocity;
            orig(self, gametime);
        }

        public override void Unload()
        {
            Main.DoDraw -= DrawTargets;
            renderers = null;
        }
    }
}