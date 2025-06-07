using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.RoomManager;
using ReLogic.Content;
using ReLogic.Graphics;
using SubworldLibrary;
using System;
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

        public override int Width => 5000;
        public override int Height => 2000;

        public static int GetWidth() => new ArchiveSubworld().Width;
        public static int GetHeight() => new ArchiveSubworld().Height;


        public override List<GenPass> Tasks => new()
        {
            new SetupGenPass("Loading", 1)
        };

        public override void OnLoad()
        {
            if (Main.dayTime)
            {
                Main.dayTime = false;
                Main.time = 0.0;
            }
        }

        // TODO: This should be part of a LoadingTooltips class or something
        List<(string Title, string Text)> tooltips = new List<(string, string)>
        {
            ("Combat Targeting", "NPCs prioritize players with higher aggro. Taunts increase it; stealth or invisibility can reduce it."),
            ("Hidden Items", "Certain items can be obtained by clearing rooms or interacting with objects in the environment."),
            //("Companions", "The Adventurer's Guild hosts a variety of companions. Hire them to make full use of their abilities to help you in combat."),
            ("Enemy Perception", "Enemies start passive and become alert when a player is nearby. Staying out of range or using stealth can avoid combat."),
            ("Scouting", "Use items that increase your range to help spot threats early and plan your approach."),
            ("Support Enemies", "Prioritize taking out enemies that buff their allies in order to reduce overall enemy effectiveness."),
            ("Barrier", "NPCs with barrier will absorb damage before their health is affected. Remove it to deal real damage."),
            ("Stealth Mechanics", "Stealthed enemies are difficult to spot. Detection gear or status effects can expose them.")
        };
        bool generatedMenuTip = false;

        string tipTitle = "";
        string tipText = "";

        public override void DrawMenu(GameTime gameTime)
        {
            if (!generatedMenuTip)
            {
                var random = new Random();
                (int index, var tooltip) = (random.Next(tooltips.Count), tooltips[random.Next(tooltips.Count)]);
                tipTitle = tooltip.Title;
                tipText = tooltip.Text;

                generatedMenuTip = true;
            }

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

            Vector2 tipTitleOffset = new Vector2(-FontAssets.DeathText.Value.MeasureString(tipTitle).X / 4, -FontAssets.DeathText.Value.MeasureString(tipTitle).Y * 2.25f);
            Main.spriteBatch.DrawString(FontAssets.DeathText.Value, tipTitle, tipTitleOffset + new Vector2(Main.screenWidth / 2, Main.screenHeight), new Color(167, 153, 104), 0f, Vector2.Zero, 0.65f, SpriteEffects.None, 1f);

            Vector2 offset = new Vector2(-FontAssets.DeathText.Value.MeasureString(tipText).X / 4, -FontAssets.DeathText.Value.MeasureString(tipText).Y * 1.5f);
            Main.spriteBatch.DrawString(FontAssets.DeathText.Value, tipText, offset + new Vector2(Main.screenWidth / 2, Main.screenHeight), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f);
            //Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text, offset + new Vector2(Main.screenWidth, Main.screenHeight) / 2 - FontAssets.DeathText.Value.MeasureString(text) / 2, Color.White);
        }

        //public static Room CenterRoom = new Room();
        public override void Update()
        {
            Main.dayTime = false;
            Main.time = 16000.0;

            Main.windSpeedCurrent = 0.05f;

            // For whatever reason, subworlds do not call these by themselves.
            foreach (KeyValuePair<int, TileEntity> pair in TileEntity.ByID)
            {
                var tileEntity = pair.Value;
                tileEntity.Update();

                if (tileEntity is NPCSpawnPoint spawnPoint)
                {
                    if (spawnPoint.ChildNPC == null && spawnPoint.SpawnerCooldown <= 0)
                    {
                        Main.NewText("subworld spawning", Color.Red);
                        spawnPoint.SpawnNPC();
                    }
                }
            }
        }

        public override void OnEnter()
        {
            // Create a popup message or title card or something

            foreach (KeyValuePair<int, TileEntity> pair in TileEntity.ByID)
            {
                var tileEntity = pair.Value;
                tileEntity.Update();

                if (tileEntity is NPCSpawnPoint spawnPoint)
                {
                    spawnPoint.SpawnNPC();
                }
            }

            generatedMenuTip = false;

            base.OnEnter();
        }

        public override void OnExit()
        {
            generatedMenuTip = false;

            base.OnExit();
        }
    }
}
