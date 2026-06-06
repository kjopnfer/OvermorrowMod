using OvermorrowMod.Core.NPCs;
using System;
using System.Collections.Generic;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    /// <summary>
    /// Defines everything a single dungeon is made of: its size, palette, spawns, room kit,
    /// and A* tuning. The generator is biome-agnostic and reads all of this from the content,
    /// so a new dungeon variant is a new <see cref="DungeonContent"/> rather than a generator change.
    /// </summary>
    public abstract class DungeonContent
    {
        /// <summary>
        /// Grid width in cells.
        /// </summary>
        public abstract int Cols { get; }

        /// <summary>
        /// Grid height in cells.
        /// </summary>
        public abstract int Rows { get; }

        /// <summary>
        /// Solid fill tile for the dungeon's stone body.
        /// </summary>
        public abstract int FillTile { get; }

        /// <summary>
        /// Interior lining tile.
        /// </summary>
        public abstract int LiningTile { get; }

        /// <summary>
        /// Resolves the colors cells paint with into this dungeon's walls, tiles, and shared decor.
        /// </summary>
        public abstract DungeonPalette Palette { get; }

        /// <summary>
        /// Enemies-to-cells ratio passed to the encounter selector.
        /// </summary>
        public virtual float BaseDensity => 1.0f;

        /// <summary>
        /// Probability that the elite pass places an elite this run.
        /// </summary>
        public virtual float EliteChance => 0.10f;

        /// <summary>
        /// Painted-color to spawn-pool map used to resolve enemy spawns.
        /// </summary>
        public abstract IReadOnlyDictionary<(byte R, byte G, byte B), SpawnPool> SpawnBindings { get; }

        /// <summary>
        /// Factories for rooms guaranteed to appear on the spine, in order.
        /// </summary>
        public abstract List<Func<GridRoom>> RequiredRooms { get; }

        public abstract GridRoom CreateCombat(bool isFeature);
        public abstract GridRoom CreateTreasure(bool isFeature);
        public abstract GridRoom CreateDoor(bool isFeature);

        /// <summary>
        /// The dungeon's basic walkable room. Used to cap a spine endpoint that
        /// has no door and as a safe room for the player to spawn in.
        /// </summary>
        public abstract GridRoom CreateFiller(bool isFeature);

        /// <summary>
        /// The dungeon's vertical connector (a 1x1 cell open top/bottom), used to
        /// hand-build a vertical exit branch from the spine to a fork door.
        /// </summary>
        public abstract GridRoom CreateVerticalConnector(bool isFeature);

        /// <summary>
        /// Per-room-type A* weight. Below 1 is preferred, above 1 is avoided.
        /// </summary>
        public abstract IReadOnlyDictionary<Type, double> TypeWeights { get; }

        /// <summary>
        /// Per-room-type maximum consecutive run length along a path.
        /// </summary>
        public abstract IReadOnlyDictionary<Type, int> StreakLimits { get; }

        /// <summary>
        /// Per-room-type minimum consecutive run length a path may end on or leave.
        /// </summary>
        public abstract IReadOnlyDictionary<Type, int> MinStreakLimits { get; }

        /// <summary>
        /// Maximum consecutive vertical moves a path may make before it must turn.
        /// </summary>
        public virtual int MaxVerticalRun => 2;
    }
}
