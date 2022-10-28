using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OvermorrowMod.Core;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod.Common.Cutscenes
{
    public class ScreenColor : UIState
    {
        public static bool visible = false;

        private float drawTimer;
        private float holdTimer;
        private float fadeTimer;

        private float drawTime;
        private float holdTime;
        private float fadeTime;

        private bool lockMouse;
        private Vector2 storedMousePosition;

        public bool IsVisible() => visible;

        public void SetDarkness(float drawTime, float holdTime, float fadeTime, bool lockMouse = false, float opacity = 1)
        {
            this.drawTime = drawTime;
            this.holdTime = holdTime;
            this.fadeTime = fadeTime;

            this.fadeTimer = fadeTime;
            visible = true;

            this.lockMouse = lockMouse;
            if (this.lockMouse)
            {
                storedMousePosition = new Vector2(Main.mouseX, Main.mouseY);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!visible) return;

            if (lockMouse) Mouse.SetPosition(0, 0);

            if (drawTimer++ < drawTime)
            {
                float progress = Utils.Clamp(drawTimer, 0, drawTime) / drawTime;
                DrawDarkness(spriteBatch, progress);
            }
            else
            {
                if (holdTimer++ < holdTime)
                {
                    DrawDarkness(spriteBatch, 1);
                    return;
                }
                else
                {
                    if (fadeTimer-- > 0)
                    {
                        float progress = (Utils.Clamp(fadeTimer, 0, fadeTime) / fadeTime);
                        DrawDarkness(spriteBatch, progress);
                    }
                    else if (fadeTimer <= 0)
                    {
                        ResetCounters();
                        visible = false;

                        if (lockMouse)
                        {
                            Mouse.SetPosition((int)storedMousePosition.X, (int)storedMousePosition.Y);
                            lockMouse = false;
                        }
                    }
                }
            }
        }

        private void ResetCounters()
        {
            drawTimer = 0;
            holdTimer = 0;
            fadeTimer = 0;
        }

        private void DrawDarkness(SpriteBatch spriteBatch, float value)
        {
            spriteBatch.Reload(BlendState.AlphaBlend);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * value);
            spriteBatch.Reload(SpriteSortMode.Deferred);

        }
    }
}