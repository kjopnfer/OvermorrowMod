using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Runs;
using OvermorrowMod.Core.UI.ClassSelection;
using OvermorrowMod.Core.WorldGeneration;
using ReLogic.Graphics;
using SubworldLibrary;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace OvermorrowMod.Core.UI.RunResume
{
    public class RunResumePrompt : UIState
    {
        public static bool visible = false;

        private const int PanelWidth = 360;
        private const int PanelHeight = 150;
        private const int ButtonWidth = 120;
        private const int ButtonHeight = 32;
        private const int ButtonGap = 24;

        private static readonly int ConfirmWindow = ModUtils.SecondsToTicks(3);

        private ClassSelectionButton resumeButton;
        private ClassSelectionButton abandonButton;
        private int confirmTimer;

        public override void OnInitialize()
        {
            resumeButton = new ClassSelectionButton { kind = ClassButtonKind.Resume };
            resumeButton.Width.Set(ButtonWidth, 0);
            resumeButton.Height.Set(ButtonHeight, 0);
            resumeButton.onClick = OnResumeClicked;
            Append(resumeButton);

            abandonButton = new ClassSelectionButton { kind = ClassButtonKind.Abandon };
            abandonButton.Width.Set(ButtonWidth, 0);
            abandonButton.Height.Set(ButtonHeight, 0);
            abandonButton.onClick = OnAbandonClicked;
            Append(abandonButton);
        }

        public void Show()
        {
            visible = true;
            confirmTimer = 0;
            abandonButton.kind = ClassButtonKind.Abandon;
        }

        public void Hide() => visible = false;

        private void OnResumeClicked()
        {
            Hide();
            SubworldSystem.Enter(Main.LocalPlayer.GetModPlayer<RunSavePlayer>().ActiveSubworld);
        }

        private void OnAbandonClicked()
        {
            if (confirmTimer == 0)
            {
                confirmTimer = ConfirmWindow;
                abandonButton.kind = ClassButtonKind.ConfirmAbandon;
                return;
            }

            Hide();
            RunReset.Reset(Main.LocalPlayer);
            ClassSelectionManager.Show();
        }

        private static Rectangle PanelRect => new Rectangle((Main.screenWidth - PanelWidth) / 2, (Main.screenHeight - PanelHeight) / 2, PanelWidth, PanelHeight);

        public override void Update(GameTime gameTime)
        {
            if (!visible) return;

            if (confirmTimer > 0 && --confirmTimer == 0)
                abandonButton.kind = ClassButtonKind.Abandon;

            bool escPressed = Main.keyState.IsKeyDown(Keys.Escape) && !Main.oldKeyState.IsKeyDown(Keys.Escape);
            bool clickedOutside = Main.mouseLeft && Main.mouseLeftRelease && !PanelRect.Contains(Main.MouseScreen.ToPoint());
            if (escPressed || clickedOutside)
            {
                Hide();
                return;
            }

            Rectangle panel = PanelRect;
            float buttonsY = panel.Bottom - ButtonHeight - 16f;
            float buttonsStartX = panel.X + (PanelWidth - (ButtonWidth * 2 + ButtonGap)) / 2f;

            resumeButton.Left.Set(buttonsStartX, 0);
            resumeButton.Top.Set(buttonsY, 0);

            abandonButton.Left.Set(buttonsStartX + ButtonWidth + ButtonGap, 0);
            abandonButton.Top.Set(buttonsY, 0);

            Recalculate();
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!visible) return;

            Rectangle panel = PanelRect;
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            spriteBatch.Draw(pixel, panel, new Color(20, 20, 30, 240));
            DrawBorder(spriteBatch, panel, new Color(160, 140, 90), 2);

            DynamicSpriteFont titleFont = FontAssets.DeathText.Value;
            string title = "Run in progress";
            Vector2 titleSize = titleFont.MeasureString(title);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, titleFont, title, new Vector2(panel.Center.X, panel.Y + 12f), new Color(220, 200, 130), 0f, new Vector2(titleSize.X / 2f, 0f), new Vector2(0.6f));

            var run = Main.LocalPlayer.GetModPlayer<RunSavePlayer>();
            string summary = run.TimerTicks >= 0 ? $"{run.BiomeName}, {SubworldTimer.Format(run.TimerTicks)} left" : run.BiomeName;
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            Vector2 summarySize = ChatManager.GetStringSize(font, summary, Vector2.One);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, summary, new Vector2(panel.Center.X, panel.Y + 58f), Color.White, 0f, new Vector2(summarySize.X / 2f, 0f), Vector2.One);

            if (panel.Contains(Main.MouseScreen.ToPoint()))
                Main.LocalPlayer.mouseInterface = true;

            base.Draw(spriteBatch);
        }

        private static void DrawBorder(SpriteBatch sb, Rectangle r, Color c, int t)
        {
            Texture2D px = TextureAssets.MagicPixel.Value;
            sb.Draw(px, new Rectangle(r.X, r.Y, r.Width, t), c);
            sb.Draw(px, new Rectangle(r.X, r.Bottom - t, r.Width, t), c);
            sb.Draw(px, new Rectangle(r.X, r.Y, t, r.Height), c);
            sb.Draw(px, new Rectangle(r.Right - t, r.Y, t, r.Height), c);
        }
    }
}
