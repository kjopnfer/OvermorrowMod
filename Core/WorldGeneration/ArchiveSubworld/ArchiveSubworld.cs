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

namespace OvermorrowMod.Core.WorldGeneration.ArchiveSubworld
{
    public class ArchiveSubworld : Subworld
    {
        public override int Width => 5600;
        public override int Height => 2000;

        public static int GetWidth() => new ArchiveSubworld().Width;
        public static int GetHeight() => new ArchiveSubworld().Height;


        public override List<GenPass> Tasks =>
        [
            new SetupGenPass("Loading", 1)
        ];

        public override void OnLoad()
        {
            if (Main.dayTime)
            {
                Main.dayTime = false;
                Main.time = 0.0;
            }
        }

        public override void DrawMenu(GameTime gameTime)
        {
            var currentTip = LoadingScreenTooltips.GetCurrentTip();

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Backgrounds + "ArchiveBackground", AssetRequestMode.ImmediateLoad).Value;
            // Left
            Main.spriteBatch.Draw(texture, new Vector2(Main.screenWidth, 0) / 2 - new Vector2(texture.Width * 1.5f, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Middle
            Main.spriteBatch.Draw(texture, new Vector2(Main.screenWidth, 0) / 2 - new Vector2(texture.Width / 2f, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Right
            Main.spriteBatch.Draw(texture, new Vector2(Main.screenWidth, 0) / 2 + new Vector2(texture.Width / 2f, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            Texture2D gradient = ModContent.Request<Texture2D>(AssetDirectory.Textures + "gradient_rectangle2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Main.spriteBatch.Draw(gradient, new Vector2(0, Main.screenHeight) - new Vector2(0, gradient.Height * 0.9f), null, Color.Red, 0f, Vector2.Zero, new Vector2(4f, 1f), SpriteEffects.None, 0f);

            Main.spriteBatch.DrawString(FontAssets.DeathText.Value, Main.statusText, new Vector2(Main.screenWidth, Main.screenHeight) / 2 - FontAssets.DeathText.Value.MeasureString(Main.statusText) / 2, Color.White);

            Vector2 tipTitleOffset = new Vector2(-FontAssets.DeathText.Value.MeasureString(currentTip.Title).X / 4, -FontAssets.DeathText.Value.MeasureString(currentTip.Title).Y * 2.25f);
            Main.spriteBatch.DrawString(FontAssets.DeathText.Value, currentTip.Title, tipTitleOffset + new Vector2(Main.screenWidth / 2, Main.screenHeight), new Color(167, 153, 104), 0f, Vector2.Zero, 0.65f, SpriteEffects.None, 1f);

            Vector2 offset = new Vector2(-FontAssets.DeathText.Value.MeasureString(currentTip.Text).X / 4, -FontAssets.DeathText.Value.MeasureString(currentTip.Text).Y * 1.5f);
            Main.spriteBatch.DrawString(FontAssets.DeathText.Value, currentTip.Text, offset + new Vector2(Main.screenWidth / 2, Main.screenHeight), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
        }

        public override void Update()
        {
            Main.dayTime = false;
            Main.time = 16000.0;

            Main.windSpeedCurrent = 0.05f;

            // For whatever reason, subworlds do not call these by themselves.
            // The NPCSpawnPoint.Update() method now handles lazy loading automatically
            foreach (KeyValuePair<int, TileEntity> pair in TileEntity.ByID)
            {
                var tileEntity = pair.Value;
                tileEntity.Update();
            }
        }

        public override void OnEnter()
        {
            LoadingScreenTooltips.Reset();

            base.OnEnter();
        }

        public override void OnExit()
        {
            LoadingScreenTooltips.Reset();

            base.OnExit();
        }
    }
}