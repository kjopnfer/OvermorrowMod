using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace OvermorrowMod.Common.Pathfinding
{
    public class RelativeCoordinate<TExtraInfo>
    {
        public short X;
        public short Y;
        public JumpPathStep[] Curve;
        public float Cost;
        public TExtraInfo Info;

        public override bool Equals(object obj)
        {
            return obj is RelativeCoordinate<TExtraInfo> other &&
                other.X == X && other.Y == Y &&
                Cost == other.Cost
                && Curve.Length == other.Curve.Length
                && Curve.SequenceEqual(other.Curve);
        }

        public override int GetHashCode()
        {
            int hc = Curve.Length;
            foreach (var val in Curve)
            {
                hc = unchecked(hc * 17 + val.GetHashCode());
            }
            hc += unchecked(hc * 19 + X.GetHashCode());
            hc += unchecked(hc * 19 + Y.GetHashCode());
            hc += unchecked(hc * 19 + Cost.GetHashCode());
            return hc;
        }
    }

    public struct JumpPathStep
    {
        public int XPos;
        public int YPos;
        public bool IsFalling;
        public float Cost;
    }

    public class PossibleMove<TExtraInfo>
    {
        public int XTarget;
        public int YTarget;
        public int XStart;
        public int YStart;
        public float Cost;
        public float CumulativeCost;
        public float Fitness;
        public bool IsLeap;
        public bool IsFall;
        public TExtraInfo Info;
        public PossibleMove<TExtraInfo>[] Parents;
    }

    public class PathFinderState
    {
        public Dictionary<(int, int), bool> CanFitInTile { get; } = new();
        public Dictionary<(int, int), bool> CanStandOnTile { get; } = new();
        public Dictionary<(int, int), bool> CanFallThroughTile { get; } = new();
        public HashSet<(int, int)> LocalVisitedState { get; set; }
        public (int X, int Y) EntitySize { get; }

        /// <summary>
        /// Set the width and height of the entity in tiles
        /// </summary>
        /// <param name="width">The width of the entity in tiles</param>
        /// <param name="height">The height of the entity in tiles</param>
        public PathFinderState(int width, int height)
        {
            EntitySize = (width, height);
        }

        public void Reset()
        {
            CanFitInTile.Clear();
            CanStandOnTile.Clear();
            CanFallThroughTile.Clear();
        }

        public void Visualize()
        {
            Main.spriteBatch.Begin();
            foreach (var ((x, y), fit) in CanFitInTile)
            {
                if (!CanStandOnTile.TryGetValue((x, y), out bool stand)) continue;
                var fallThrough = CanFallThroughTile.GetValueOrDefault((x, y));

                if (fallThrough)
                {
                    Main.spriteBatch.Draw(TextureAssets.Tile[TileID.Stone].Value,
                        new Vector2(x * 16f, y * 16f) - Main.screenPosition,
                        new Rectangle(0, 0, 16, 16),
                        new Color(1.0f, 0f, 0f, 0.2f));
                }
                else if (fit && stand)
                {
                    Main.spriteBatch.Draw(TextureAssets.Tile[TileID.Stone].Value,
                        new Vector2(x * 16f, y * 16f) - Main.screenPosition,
                        new Rectangle(0, 0, 16, 16),
                        new Color(0f, 1.0f, 0f, 0.2f));
                }
                else if (fit)
                {
                    Main.spriteBatch.Draw(TextureAssets.Tile[TileID.Stone].Value,
                        new Vector2(x * 16f, y * 16f) - Main.screenPosition,
                        new Rectangle(0, 0, 16, 16),
                        new Color(0f, 0f, 1.00f, 0.2f));
                }
                else
                {
                    Main.spriteBatch.Draw(TextureAssets.Tile[TileID.Stone].Value,
                        new Vector2(x * 16f, y * 16f) - Main.screenPosition,
                        new Rectangle(0, 0, 16, 16),
                        new Color(1.0f, 1.0f, 0f, 0.2f));
                }
            }
            Main.spriteBatch.End();
        }

        // Invalidate states related to this tile
        public void Invalidate(int x, int y)
        {
            for (int i = x - EntitySize.X; i < x + EntitySize.X * 2; i++)
            {
                if (i < 0 || i > Main.tile.Width) continue;
                for (int j = y - EntitySize.Y; j < y + EntitySize.Y * 2; j++)
                {
                    if (j < 0 || j > Main.tile.Height) continue;
                    CanFitInTile.Remove((i, j));
                    CanStandOnTile.Remove((i, j));
                }
            }
        }
    }

    public abstract class BasePathFinder<TExtraInfo>
    {
        private int _stepTimeout;
        private int _maxDivergence;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="timeout">Maximum number of tiles to visit before timing out</param>
        /// <param name="maxDivergence">Maximum number of steps with no improvement</param>
        public BasePathFinder(PathFinderState state, int timeout, int maxDivergence)
        {
            _stepTimeout = timeout;
            _maxDivergence = maxDivergence;
            State = state;
        }

        PossibleMove<TExtraInfo> CurrentMove;

        protected PathFinderState State { get; }

        /// <summary>
        /// Static list of where this path finder can leap, i.e. move through the air.
        /// Normal movement is prioritized.
        /// The list is in relative coordinates. For example (-2, 0) means two steps to the left.
        /// Only include leaps that you actually want the AI to make here. Meaning they can be quite short,
        /// but they 
        /// </summary>
        protected abstract IEnumerable<RelativeCoordinate<TExtraInfo>> LeapTargets { get; }

        /// <summary>
        /// Static list of where this path finder can move, i.e. where it can go in ~one tick.
        /// </summary>
        protected abstract IEnumerable<RelativeCoordinate<TExtraInfo>> MoveTargets { get; }

        /// <summary>
        /// True if the AI should work in best-effort mode, where the closest point is returned, even if it is not exact.
        /// </summary>
        protected abstract bool IsBestEffort { get; }

        /// <summary>
        /// Should return the places the AI can fall to from the previous move
        /// </summary>
        protected abstract IEnumerable<RelativeCoordinate<TExtraInfo>> GetFallTargets(int x, int y, TExtraInfo info);

        
        /// <summary>
        /// Return the next move for the pathfinder, movement code is responsible for actually executing the move there.
        /// Returns null 
        /// </summary>
        /// <returns></returns>
        public virtual PossibleMove<TExtraInfo> Next(int x, int y, int targetX, int targetY)
        {
            targetY = ProjectToGround(targetX, targetY);
            if (CurrentMove == null || _lastXTarget != targetX || _lastYTarget != targetY)
            {
                CurrentMove = GetBestPath(State, x, y, targetX, targetY);
                _lastXTarget = targetX;
                _lastYTarget = targetY;
                return GetNextMove(x, y);
            }
            else
            {
                var move = GetNextMove(x, y);
                if (move == null)
                {
                    CurrentMove = GetBestPath(State, x, y, targetX, targetY);
                    _lastXTarget = targetX;
                    _lastYTarget = targetY;
                    return GetNextMove(x, y);
                }
                return move;
            }
        }

        // Find the first position below the current position where we can stand.
        // Returns the new y position, or the old one if no such position exists.
        protected int ProjectToGround(int x, int y)
        {
            var originalY = y;
            while (CanFitInTile(State, x, y))
            {
                if (CanStandOn(State, x, y)) return y;
                y++;
            }
            return originalY;
        }

        private PossibleMove<TExtraInfo> GetBestPath(PathFinderState state, int x, int y, int targetX, int targetY)
        {
            var moves = new PriorityQueue<PossibleMove<TExtraInfo>, float>();
            int steps = 0;
            float value = (float)Math.Sqrt((targetX - x) * (targetX - x) + (targetY - y) * (targetY - y));
            moves.Enqueue(new PossibleMove<TExtraInfo>
            {
                Cost = 0,
                IsLeap = false,
                XTarget = x,
                YTarget = y,
                XStart = x,
                YStart = y,
                Parents = Array.Empty<PossibleMove<TExtraInfo>>(),
                Fitness = value,
                CumulativeCost = 0,
                Info = default
            }, value);
            State.LocalVisitedState = new HashSet<(int, int)>();
            float lastValue = value;
            float bestValue = value;
            int diverges = 0;
            PossibleMove<TExtraInfo> bestMove = null;

            while (steps++ < _stepTimeout && moves.Count > 0 && diverges < _maxDivergence)
            {
                if (lastValue > bestValue)
                {
                    diverges++;
                }
                else
                {
                    diverges = 0;
                }

                var cheapestMove = moves.Dequeue();
                if (cheapestMove.XTarget == targetX && cheapestMove.YTarget == targetY)
                {
                    return cheapestMove;
                }
                if (cheapestMove.Fitness < bestValue)
                {
                    bestValue = cheapestMove.Fitness;
                    bestMove = cheapestMove;
                }
                lastValue = cheapestMove.Fitness;
                State.LocalVisitedState.Add((cheapestMove.XTarget, cheapestMove.YTarget));

                var next = ListPossibleTargets(state, cheapestMove, targetX, targetY);

                foreach (var move in next)
                {
                    if (State.LocalVisitedState.Contains((move.XTarget, move.YTarget))) continue;
                    moves.Enqueue(move, move.Fitness + move.CumulativeCost);
                }
            }

            if (IsBestEffort)
            {
                return bestMove;
            }
            return null;
        }

        private int _lastXTarget;
        private int _lastYTarget;

        private PossibleMove<TExtraInfo> GetNextMove(int x, int y)
        {
            if (CurrentMove == null) return null;

            PossibleMove<TExtraInfo> move;
            if (CurrentMove.XStart == x && CurrentMove.YStart == y)
            {
                move = CurrentMove;
                CurrentMove = null;
            }
            else
            {
                var idx = Array.FindIndex(CurrentMove.Parents, a => a.XStart == x && a.YStart == y);
                if (idx < 0)
                {
                    return null;
                }
                move = CurrentMove.Parents[idx];
            }

            return move;
        }

        /// <summary>
        /// Lists possible targets from the given point.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private IEnumerable<PossibleMove<TExtraInfo>> ListPossibleTargets(PathFinderState state, PossibleMove<TExtraInfo> parent, int targetX, int targetY)
        {
            int x = parent.XTarget;
            int y = parent.YTarget;
            PossibleMove<TExtraInfo>[] parents;
            if (parent.XStart == parent.XTarget && parent.YStart == parent.YTarget)
            {
                parents = Array.Empty<PossibleMove<TExtraInfo>>();
                bool canStand = CanStandOn(state, x, y);
                if (!canStand || CanFallThrough(x, y))
                {
                    foreach (var fall in GetFallTargets(x, y, parent.Info))
                    {
                        int xDiff2 = (targetX - x - fall.X);
                        int yDiff2 = (targetY - y - fall.Y);
                        yield return new PossibleMove<TExtraInfo>
                        {
                            Cost = fall.Cost,
                            XTarget = x + fall.X,
                            YTarget = y + fall.Y,
                            XStart = x,
                            YStart = y ,
                            Fitness = (float)Math.Sqrt(xDiff2 * xDiff2 + yDiff2 * yDiff2),
                            IsFall = true,
                            Info = fall.Info,
                            Parents = parents,
                            CumulativeCost = parent.CumulativeCost + fall.Cost
                        };
                    }
                    if (!canStand) yield break;
                } 

            }
            else
            {
                parents = parent.Parents.Append(parent).ToArray();
            }
            foreach (var target in LeapTargets)
            {
                JumpPathStep? finalStep = null;
                bool isSolidUnder = false;
                JumpPathStep? lastStep = null;

                foreach (var step in target.Curve)
                {
                    int stepToX = x + step.XPos;
                    int stepToY = y + step.YPos;

                    if (!CanFitInTile(state, stepToX, stepToY))
                    {
                        finalStep = lastStep;
                        break;
                    }
                    lastStep = step;
                    
                    if (step.IsFalling)
                    {
                        bool canStand = CanStandOn(state, stepToX, stepToY);
                        if (!canStand && step.YPos != 0) continue;

                        finalStep = step;
                        isSolidUnder = canStand;

                        break;
                    }
                }


                if (finalStep != null && (finalStep.Value.XPos != 0 || finalStep.Value.YPos != 0))
                {
                    var jumpStep = finalStep.Value;
                    int xDiff = (targetX - x - jumpStep.XPos);
                    int yDiff = (targetY - y - jumpStep.YPos);
                    var fallMove = new PossibleMove<TExtraInfo>
                    {
                        Cost = jumpStep.Cost,
                        XTarget = x + jumpStep.XPos,
                        YTarget = y + jumpStep.YPos,
                        XStart = x,
                        YStart = y,
                        Fitness = (float)Math.Sqrt(xDiff * xDiff + yDiff * yDiff),
                        IsLeap = true,
                        Info = target.Info,
                        Parents = parents,
                        CumulativeCost = parent.CumulativeCost + jumpStep.Cost
                    };

                    if (isSolidUnder)
                    {
                        yield return fallMove;
                    }

                    if (!isSolidUnder || CanFallThrough(x + jumpStep.XPos, y + jumpStep.YPos))
                    {
                        var fallParents = parents.Append(fallMove).ToArray();
                        foreach (var fall in GetFallTargets(x + jumpStep.XPos, y + jumpStep.YPos, target.Info))
                        {
                            int xDiff2 = xDiff - fall.X;
                            int yDiff2 = yDiff - fall.Y;
                            yield return new PossibleMove<TExtraInfo>
                            {
                                Cost = fall.Cost,
                                XTarget = x + jumpStep.XPos + fall.X,
                                YTarget = y + jumpStep.YPos + fall.Y,
                                XStart = x + jumpStep.XPos,
                                YStart = y + jumpStep.YPos,
                                Fitness = (float)Math.Sqrt(xDiff2 * xDiff2 + yDiff2 * yDiff2),
                                IsFall = true,
                                Info = fall.Info,
                                Parents = fallParents,
                                CumulativeCost = parent.CumulativeCost + jumpStep.Cost + fall.Cost
                            };
                        }
                    }
                }

            }

            foreach (var target in MoveTargets)
            {
                if (!CanFitInTile(state, x + target.X, y + target.Y)) continue;

                bool canStand = CanStandOn(state, x + target.X, y + target.Y);
                int xDiff = (targetX - x - target.X);
                int yDiff = (targetY - y - target.Y);
                var fallMove = new PossibleMove<TExtraInfo>
                {
                    Cost = target.Cost,
                    XTarget = x + target.X,
                    YTarget = y + target.Y,
                    XStart = x,
                    YStart = y,
                    Fitness = (float)Math.Sqrt(xDiff * xDiff + yDiff * yDiff),
                    IsLeap = false,
                    Info = target.Info,
                    Parents = parents,
                    CumulativeCost = parent.CumulativeCost + target.Cost
                };

                if (canStand)
                {
                    yield return fallMove;
                }
                if (target.Y == 0 && (!canStand || CanFallThrough(x + target.X, y + target.Y)))
                {
                    var fallParents = parents.Append(fallMove).ToArray();
                    foreach (var fall in GetFallTargets(x + target.X, y + target.Y, target.Info))
                    {
                        int xDiff2 = (targetX - x - target.X - fall.X);
                        int yDiff2 = (targetY - y - target.Y - fall.Y);
                        yield return new PossibleMove<TExtraInfo>
                        {
                            Cost = fall.Cost,
                            XTarget = x + target.X + fall.X,
                            YTarget = y + target.Y + fall.Y,
                            XStart = x + target.X,
                            YStart = y + target.Y,
                            Fitness = (float)Math.Sqrt(xDiff2 * xDiff2 + yDiff2 * yDiff2),
                            IsFall = true,
                            Info = fall.Info,
                            Parents = fallParents,
                            CumulativeCost = parent.CumulativeCost + target.Cost + fall.Cost
                        };
                    }
                }
            }
        }

        private bool CanFitInTile(int x, int y)
        {
            for (int i = x; i < x + State.EntitySize.X; i++)
            {
                for (int j = y; j < y + State.EntitySize.Y; j++)
                {
                    if (i >= Main.tile.Width || j >= Main.tile.Width) return false;
                    var tile = Main.tile[i, j];
                    if (tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType]) return false;
                }
            }
            return true;
        }

        protected bool CanFitInTile(PathFinderState state, int x, int y)
        {
            if (state.CanFitInTile.TryGetValue((x, y), out bool cond)) return cond;
            var fit = CanFitInTile(x, y);
            state.CanFitInTile[(x, y)] = fit;
            return fit;
        }

        private bool CanStandOn(int x, int y)
        {
            if (y == Main.tile.Height - 1) return true;
            for (int i = x; i < x + State.EntitySize.X; i++)
            {
                var tile = Main.tile[i, y + State.EntitySize.Y];
                if (tile.HasUnactuatedTile && (Main.tileSolidTop[tile.TileType] || Main.tileSolid[tile.TileType])) return true;
            }
            return false;
        }

        protected bool CanStandOn(PathFinderState state, int x, int y)
        {
            if (state.CanStandOnTile.TryGetValue((x, y), out bool cond)) return cond;
            var fit = CanStandOn(x, y);
            state.CanStandOnTile[(x, y)] = fit;
            return fit;
        }

        private bool CanFallThrough(int x, int y)
        {
            if (y == Main.tile.Height - 1) return false;
            for (int i = x; i < x + State.EntitySize.X; i++)
            {
                var tile = Main.tile[i, y + State.EntitySize.Y];
                if (tile.HasUnactuatedTile && !Main.tileSolidTop[tile.TileType]) return false;
            }
            return true;
        }

        protected bool CanFallThrough(PathFinderState state, int x, int y)
        {
            if (state.CanFallThroughTile.TryGetValue((x, y), out bool cond)) return cond;
            var fit = CanFallThrough(x, y);
            state.CanFallThroughTile[(x, y)] = fit;
            return fit;
        }
    }
}
