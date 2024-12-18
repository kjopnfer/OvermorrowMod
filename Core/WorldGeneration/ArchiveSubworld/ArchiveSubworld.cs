using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using ReLogic.Content;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Core.WorldGeneration.ArchiveSubworld
{
    public class ArchiveSubworld : Subworld
    {

        public override int Width => ModContent.Request<Texture2D>(AssetDirectory.TextureMaps + "ArchiveTiles", AssetRequestMode.ImmediateLoad).Value.Width;
        public override int Height => ModContent.Request<Texture2D>(AssetDirectory.TextureMaps + "ArchiveTiles", AssetRequestMode.ImmediateLoad).Value.Height + 50;

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

        public override void Update()
        {
            Main.dayTime = false;
            Main.time = 0.0;

            // For whatever reason, subworlds do not call these by themselves.
            foreach (KeyValuePair<int, TileEntity> pair in TileEntity.ByID)
            {
                var tileEntity = pair.Value;
                tileEntity.Update();
            }
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
