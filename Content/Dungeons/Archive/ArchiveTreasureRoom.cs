using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.Loot;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid;
using System;
using System.Collections.Generic;

namespace OvermorrowMod.Content.Dungeons.Archive
{
    /// <summary>
    /// Places the exit door at its marker and, at one marked spot picked at random, a big chest offering a pick of three
    /// Rare or better gear.
    /// </summary>
    public class ArchiveTreasureRoom : TreasureRoom
    {
        private static readonly (byte, byte, byte) ExitDoorMarker = (32, 30, 27);
        private static readonly (byte, byte, byte) RewardSpotMarker = (55, 148, 110);

        private static readonly RarityModifier ChestModifier = new(common: -70, rare: 41, epic: 27, legendary: 2);

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
                var chest = TileUtils.PlaceTileWithEntity<BigChest, BigChest_TE>(spot.X, spot.Y);
                if (chest != null)
                {
                    chest.ContentsModifier = ChestModifier;
                    chest.ContentsKinds = ItemKind.Weapon | ItemKind.Accessory | ItemKind.Armor;
                    chest.ContentsFavored = ItemKind.Weapon | ItemKind.Armor;
                    chest.OfferCount = 3;
                }
            }

            if (doorTile == null) return null;
            return TileUtils.PlaceTileWithEntity<ArchiveDoor, ArchiveDoor_TE>(doorTile.Value.X, doorTile.Value.Y);
        }
    }
}
