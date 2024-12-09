using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using ReLogic.Content;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Core.WorldGeneration.ArchiveSubworld
{
    public class ArchiveSubworld : Subworld
    {

        public override int Width => ModContent.Request<Texture2D>(AssetDirectory.TextureMaps + "ArchiveTiles", AssetRequestMode.ImmediateLoad).Value.Width;

        public override int Height => ModContent.Request<Texture2D>(AssetDirectory.TextureMaps + "ArchiveTiles", AssetRequestMode.ImmediateLoad).Value.Height;

        public static int GetWidth() => new ArchiveSubworld().Width;
        public static int GetHeight() => new ArchiveSubworld().Height;


        public override List<GenPass> Tasks => new()
        {
            new SetupGenPass("Loading", 1)
        };

        public override void OnLoad()
        {
            Main.NewText("change time??");

            if (Main.dayTime)
            {
                Main.dayTime = false;
                Main.time = 0.0;
            }
        }

        public override void Update()
        {
            Main.NewText("bruh");

            Main.dayTime = false;
            Main.time = 0.0;
        }

        public override void OnEnter()
        {
            // Create a popup message or title card or something


            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
