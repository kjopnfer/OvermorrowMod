using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.RoomManager;
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

        public override int Width => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "ArchiveTiles", AssetRequestMode.ImmediateLoad).Value.Width;
        public override int Height => ModContent.Request<Texture2D>(AssetDirectory.TexGen + "ArchiveTiles", AssetRequestMode.ImmediateLoad).Value.Height + 50;

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

            Main.windSpeedCurrent = 0.05f;

            // For whatever reason, subworlds do not call these by themselves.
            foreach (KeyValuePair<int, TileEntity> pair in TileEntity.ByID)
            {
                var tileEntity = pair.Value;
                tileEntity.Update();

                if (tileEntity is NPCSpawnPoint spawnPoint)
                {
                    if (spawnPoint.ChildNPC == null && !spawnPoint.HasBeenCleared)
                        spawnPoint.SpawnNPC();
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

            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
