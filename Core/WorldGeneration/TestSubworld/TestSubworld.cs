using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core.LoadingScreen;
using ReLogic.Content;
using ReLogic.Graphics;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Core.WorldGeneration.TestSubworld
{
    public class TestSubworld : Subworld
    {
        public override int Width => 4800;
        public override int Height => 2400;

        // Regen on every entry. Without this, SubworldLibrary saves the
        // generated world and re-enter just reloads it, so the Regenerate
        // button has no effect.
        public override bool ShouldSave => false;

        public override List<GenPass> Tasks =>
        [
            new TestGenPass("Loading", 1)
        ];

        public override void OnLoad()
        {
        }

        public override void Update()
        {
            // Subworlds don't call TileEntity.Update() automatically
            foreach (KeyValuePair<int, TileEntity> pair in TileEntity.ByID)
            {
                pair.Value.Update();
            }
        }

        public override void DrawMenu(GameTime gameTime)
        {
            var currentTip = LoadingScreenTooltips.GetCurrentTip();

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Backgrounds + "ArchiveBackground", AssetRequestMode.ImmediateLoad).Value;
            Main.spriteBatch.Draw(texture, new Vector2(Main.screenWidth, 0) / 2 - new Vector2(texture.Width * 1.5f, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, new Vector2(Main.screenWidth, 0) / 2 - new Vector2(texture.Width / 2f, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, new Vector2(Main.screenWidth, 0) / 2 + new Vector2(texture.Width / 2f, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            Texture2D gradient = ModContent.Request<Texture2D>(AssetDirectory.Textures + "gradient_rectangle2", AssetRequestMode.ImmediateLoad).Value;
            Main.spriteBatch.Draw(gradient, new Vector2(0, Main.screenHeight) - new Vector2(0, gradient.Height * 0.9f), null, Color.Red, 0f, Vector2.Zero, new Vector2(4f, 1f), SpriteEffects.None, 0f);

            DynamicSpriteFont font = FontAssets.DeathText.Value;
            Main.spriteBatch.DrawString(font, Main.statusText, new Vector2(Main.screenWidth, Main.screenHeight) / 2 - font.MeasureString(Main.statusText) / 2, Color.White);

            Vector2 titleSize = font.MeasureString(currentTip.Title);
            Vector2 titlePos = new Vector2(Main.screenWidth / 2f, Main.screenHeight - titleSize.Y * 2.25f);
            Main.spriteBatch.DrawString(font, currentTip.Title, titlePos, new Color(167, 153, 104), 0f, new Vector2(titleSize.X / 2f, 0f), 0.65f, SpriteEffects.None, 1f);

            Vector2 textSize = font.MeasureString(currentTip.Text);
            Vector2 textPos = new Vector2(Main.screenWidth / 2f, Main.screenHeight - textSize.Y * 1.5f);
            Main.spriteBatch.DrawString(font, currentTip.Text, textPos, Color.White, 0f, new Vector2(textSize.X / 2f, 0f), 0.5f, SpriteEffects.None, 1f);
        }

        public override void OnEnter()
        {
            LoadingScreenTooltips.Reset(TipSource.JermaQuotes);
            base.OnEnter();
        }

        public override void OnExit()
        {
            LoadingScreenTooltips.Reset(TipSource.JermaQuotes);
            base.OnExit();
        }
    }
}
