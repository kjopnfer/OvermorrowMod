using System.Collections.Generic;
using Microsoft.Xna.Framework;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Pathfinding;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    /// <summary>
    /// What a path represents in the dungeon graph. Used by branch
    /// validation and content systems to reason about a path's role
    /// without inspecting cells.
    /// </summary>
    public enum PathRole
    {
        /// <summary>The critical path from start door to end door.</summary>
        Spine,

        /// <summary>
        /// A branch that re-enters its parent at two points. Creates one
        /// cycle in the cell graph; if it bridges across its parent's
        /// combat, it must contain its own combat to preserve the
        /// "every start-to-end path crosses some combat" invariant.
        /// </summary>
        ClosedLoopBranch,

        /// <summary>
        /// A branch that attaches to its parent at one point and ends in
        /// a placeholder cell (treasure, chest, NPC slot). Cannot create
        /// a bypass because there's no second connection back to the
        /// parent.
        /// </summary>
        DeadEndBranch,
    }

    /// <summary>
    /// One named, addressable path through the dungeon. Holds the cells
    /// the path placed (excluding shared endpoints), the path's combat
    /// anchor if it has one, and a parent reference so branches know
    /// which path they hang off of.
    /// </summary>
    /// <remarks>
    /// Steps excludes the path's combat anchor and excludes the parent's
    /// nodes the branch attaches to. Steps is the set of attachable
    /// "interior" cells: the bookshelves, corridors, shafts, stairs that
    /// were placed by this path's A* runs. Use these for picking aux
    /// branch endpoints.
    /// </remarks>
    public sealed class DungeonPath
    {
        public int Id { get; }
        public PathRole Role { get; }
        public DungeonPath Parent { get; }
        public IReadOnlyList<PathStep> Steps { get; }

        /// <summary>
        /// Anchor of this path's combat, or null if the path has no
        /// combat (dead-end branches, same-side closed loops).
        /// </summary>
        public Point? CombatAnchor { get; }

        /// <summary>
        /// Anchor of this path's terminal placeholder cell (treasure,
        /// chest, etc.), or null. Set on dead-end branches.
        /// </summary>
        public Point? PlaceholderAnchor { get; }

        public DungeonPath(int id, PathRole role, DungeonPath parent,
                           IReadOnlyList<PathStep> steps,
                           Point? combatAnchor,
                           Point? placeholderAnchor = null)
        {
            Id = id;
            Role = role;
            Parent = parent;
            Steps = steps;
            CombatAnchor = combatAnchor;
            PlaceholderAnchor = placeholderAnchor;
        }

        /// <summary>
        /// True if <paramref name="cellAnchor"/> sits on the "before
        /// combat" side of this path. Uses X-based partition (subgoals
        /// are sorted by X during spine planning, so X is monotonic
        /// enough). Returns false when the path has no combat — callers
        /// that care about that case should special-case it.
        /// </summary>
        public bool IsBeforeCombat(Point cellAnchor)
        {
            if (CombatAnchor == null) return false;
            return cellAnchor.X < CombatAnchor.Value.X;
        }

        /// <summary>
        /// True if <paramref name="cellAnchor"/> sits on the "after
        /// combat" side of this path's combat (X past the right edge of
        /// the combat footprint). Default <paramref name="combatWidth"/>
        /// matches CombatRoom's 3-wide footprint.
        /// </summary>
        public bool IsAfterCombat(Point cellAnchor, int combatWidth = 3)
        {
            if (CombatAnchor == null) return false;
            return cellAnchor.X > CombatAnchor.Value.X + combatWidth - 1;
        }

        /// <summary>
        /// True if the closed-loop branch from <paramref name="nodeA"/>
        /// to <paramref name="nodeB"/> on this parent path needs its own
        /// combat to preserve the no-bypass invariant. Loops crossing
        /// the parent's combat divide need one; same-side loops don't.
        /// </summary>
        public bool ClosedLoopAcrossCombat(Point nodeA, Point nodeB)
        {
            if (CombatAnchor == null) return false;
            bool aBefore = IsBeforeCombat(nodeA);
            bool bBefore = IsBeforeCombat(nodeB);
            return aBefore != bBefore;
        }
    }
}
