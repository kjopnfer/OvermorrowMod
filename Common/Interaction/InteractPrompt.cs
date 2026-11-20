using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod.Common.Interaction
{
    public class InteractPrompt : ModSystem
    {
        private const int FrameHeight = 24;
        private const float OuterRadius = 200f;
        private const float InnerRadius = 120f;

        public static Texture2D InteractIcon => ModContent.Request<Texture2D>(AssetDirectory.UI + "InteractIcon").Value;
        public static Texture2D WaypointIcon => ModContent.Request<Texture2D>(AssetDirectory.UI + "WaypointIcon").Value;

        private static Texture2D cursorIcon;
        private static float cursorScale;

        public static Rectangle Frame(Texture2D icon, int frame) => new Rectangle(0, frame * FrameHeight, icon.Width, FrameHeight);

        /// <summary>
        /// Shows <paramref name="icon"/> beside the mouse for this update. Call every update from <c>MouseOver</c>.
        /// </summary>
        public static void SetCursor(Texture2D icon, float scale = 1f)
        {
            Main.LocalPlayer.noThrow = 2;
            cursorIcon = icon;
            cursorScale = scale;
        }

        /// <param name="zero">The tile draw offset (<c>Main.offScreenRange</c> unless drawing to screen).</param>
        public static void DrawFloating(SpriteBatch spriteBatch, Texture2D icon, Vector2 worldAnchor, Vector2 zero, float scale = 1f)
        {
            float dist = Vector2.Distance(Main.LocalPlayer.Center, worldAnchor);
            float proximity = MathHelper.Clamp((OuterRadius - dist) / (OuterRadius - InnerRadius), 0f, 1f);
            if (proximity <= 0f) return;

            float wave = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f);
            float alpha = proximity * (0.85f + wave * 0.15f);
            Vector2 screenPos = worldAnchor + new Vector2(0f, wave * 4f) - Main.screenPosition + zero;
            Rectangle frame = Frame(icon, 0);
            spriteBatch.Draw(icon, screenPos, frame, Color.White * alpha, 0f, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
        }

        public override void PreUpdatePlayers() => cursorIcon = null;

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(layer => layer.Name == "Vanilla: Interact Item Icon");
            if (index < 0) return;

            layers.Insert(index + 1, new LegacyGameInterfaceLayer("OvermorrowMod: Interact Prompt Cursor", DrawCursor, InterfaceScaleType.UI));
        }

        private static bool DrawCursor()
        {
            if (cursorIcon == null || Main.LocalPlayer.mouseInterface) return true;

            Vector2 position = new Vector2(Main.mouseX + 10, Main.mouseY + 10);
            Main.spriteBatch.Draw(cursorIcon, position, Frame(cursorIcon, 0), Color.White, 0f, Vector2.Zero, cursorScale * Main.cursorScale, SpriteEffects.None, 0f);
            return true;
        }
    }
}
