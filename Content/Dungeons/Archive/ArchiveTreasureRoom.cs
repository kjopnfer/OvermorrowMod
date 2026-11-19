using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Misc;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Dungeons.Archive
{
    /// <summary>
    /// Places the exit door at its marker and the reward at one marked spot picked at random.
    /// </summary>
    public class ArchiveTreasureRoom : TreasureRoom
    {
        private static readonly (byte, byte, byte) ExitDoorMarker = (32, 30, 27);
        private static readonly (byte, byte, byte) RewardSpotMarker = (55, 148, 110);

        protected override string AsepritePath => AssetDirectory.GrandArchives + "TreasureRoom.aseprite";

        protected override IDungeonDoor PlaceFeatures(Point origin, Random rand)
        {
            Point? doorTile = null;
            var rewardSpots = new List<Point>();
            TexGen.HarvestAsepriteLayer(SheetLayer.Objects, AsepritePath, origin.X, origin.Y, (x, y, color) =>
            {
                if (color == ExitDoorMarker) doorTile = new Point(x, y);
                else if (color == RewardSpotMarker) rewardSpots.Add(new Point(x, y));
            });

            if (rewardSpots.Count > 0)
            {
                Point spot = rewardSpots[rand.Next(rewardSpots.Count)];
                ModContent.GetInstance<TreasureReward_TE>().Place(spot.X, spot.Y);
            }

            if (doorTile == null) return null;
            return TileUtils.PlaceTileWithEntity<ArchiveDoor, ArchiveDoor_TE>(doorTile.Value.X, doorTile.Value.Y);
        }
    }
}
