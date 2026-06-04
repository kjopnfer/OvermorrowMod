using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.NPCs;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    public class ArchiveContent : DungeonContent
    {
        public override int Cols => 35;
        public override int Rows => 30;

        public override int FillTile => ModContent.TileType<CastleBrick>();
        public override int LiningTile => ModContent.TileType<ArchiveWood>();

        public override DungeonPalette Palette { get; } = new ArchivePalette();

        public override IReadOnlyDictionary<(byte R, byte G, byte B), SpawnPool> SpawnBindings
        {
            get
            {
                ArchiveSpawnPool.Initialize();
                return new Dictionary<(byte R, byte G, byte B), SpawnPool>
                {
                    [(255, 0, 0)] = ArchiveSpawnPool.BaseGroundPool,
                    [(221, 255, 0)] = ArchiveSpawnPool.WallPool,
                };
            }
        }

        public override List<Func<GridRoom>> RequiredRooms => new()
        {
            () => new FireplaceRoom(),
            () => new CombatRoom(),
            () => new WritingRoom(),
        };

        public override GridRoom CreateCombat(bool isFeature) => new CombatRoom { IsFeature = isFeature };
        public override GridRoom CreateTreasure(bool isFeature) => new ChestRoom { IsFeature = isFeature };
        public override GridRoom CreateDoor(bool isFeature) => new DoorRoom { IsFeature = isFeature };

        public override IReadOnlyDictionary<Type, double> TypeWeights => new Dictionary<Type, double>
        {
            [typeof(ShaftCell)] = 1.4,
            [typeof(DescendingStair)] = 0.7,
            [typeof(AscendingStair)] = 0.7,
            [typeof(FireplaceRoom)] = 1.5,
            [typeof(LoungeRoom)] = 0.3,
            [typeof(CombatRoom)] = 0.7
        };

        public override IReadOnlyDictionary<Type, int> StreakLimits => new Dictionary<Type, int>
        {
            [typeof(BookshelfCell)] = 4,
            [typeof(CorridorCell)] = 5,
            [typeof(FireplaceRoom)] = 1
        };

        public override IReadOnlyDictionary<Type, int> MinStreakLimits => new Dictionary<Type, int>
        {
            [typeof(BookshelfCell)] = 2
        };
    }
}
