using CollisionLib;
using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.CustomCollision
{
    /// <summary>
    /// Per-tick spatial index over every active TileCollisionNPC's segments.
    /// Drives the centralized grounding pass for opted-in NPCs.
    /// </summary>
    public class CollisionIndex : ModSystem
    {
        /// <summary>
        /// Master switch for the custom-collider grounding system.
        /// Auto-disables if the per-tick code throws.
        /// </summary>
        public static bool Enabled = true;

        private struct Segment
        {
            public Vector2 a;
            public Vector2 b;
            public float minX;
            public float maxX;
            public float minY;
            public float maxY;
        }

        private const int ColumnSize = 16;

        private static List<Segment> segments = new List<Segment>();
        private static Dictionary<int, List<int>> bucketsByColumn = new Dictionary<int, List<int>>();

        public override void PostUpdateNPCs()
        {
            if (!Enabled) return;

            try
            {
                Rebuild();
                ApplyGroundingPass();
            }
            catch (System.Exception ex)
            {
                Main.NewText($"CollisionIndex disabled after error: {ex.Message}", Microsoft.Xna.Framework.Color.Orange);
                Enabled = false;
                segments.Clear();
                bucketsByColumn.Clear();
            }
        }

        public override void OnWorldUnload()
        {
            segments.Clear();
            bucketsByColumn.Clear();
        }

        private static void ApplyGroundingPass()
        {
            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.ModNPC is not OvermorrowNPC overmorrowNPC) continue;

                if (!overmorrowNPC.UsesCustomGrounding || overmorrowNPC.DropThroughActive)
                {
                    overmorrowNPC.SetCurrentSupportY(null);
                    continue;
                }

                ApplyGroundingTo(npc, overmorrowNPC);
            }
        }

        private static void ApplyGroundingTo(NPC npc, OvermorrowNPC overmorrowNPC)
        {
            float curFeetY = npc.Bottom.Y;
            float prevFeetY = npc.oldPosition.Y + npc.height;

            Rectangle hb = npc.Hitbox;
            float leftX = hb.Left + 1;
            float centerX = hb.Left + hb.Width * 0.5f;
            float rightX = hb.Right - 1;

            bool movingDown = curFeetY >= prevFeetY;

            if (movingDown)
            {
                bool landed = false;
                float landingY = 0f;
                TryLand(leftX, prevFeetY, curFeetY, ref landed, ref landingY);
                TryLand(centerX, prevFeetY, curFeetY, ref landed, ref landingY);
                TryLand(rightX, prevFeetY, curFeetY, ref landed, ref landingY);

                if (landed)
                {
                    npc.position.Y = landingY - npc.height;
                    npc.velocity.Y = Math.Min(npc.velocity.Y, 0f);
                    npc.collideY = true;
                    overmorrowNPC.SetCurrentSupportY(landingY);
                    return;
                }

                bool resting = false;
                float restingY = 0f;
                TryRest(leftX, curFeetY, ref resting, ref restingY);
                TryRest(centerX, curFeetY, ref resting, ref restingY);
                TryRest(rightX, curFeetY, ref resting, ref restingY);

                if (resting)
                {
                    npc.position.Y = restingY - npc.height;
                    npc.velocity.Y = 0f;
                    npc.collideY = true;
                    overmorrowNPC.SetCurrentSupportY(restingY);
                    return;
                }
            }

            overmorrowNPC.SetCurrentSupportY(null);
        }

        private static void TryLand(float worldX, float prevFeetY, float curFeetY, ref bool found, ref float bestY)
        {
            if (!TryGetLandingThisFrame(worldX, prevFeetY, curFeetY, out float y)) return;
            if (!found || y < bestY)
            {
                found = true;
                bestY = y;
            }
        }

        private static void TryRest(float worldX, float feetY, ref bool found, ref float bestY)
        {
            if (!TryGetGroundBeneath(worldX, feetY - 4f, out float y)) return;
            if (y > feetY || y < feetY - 4f) return;
            if (!found || y < bestY)
            {
                found = true;
                bestY = y;
            }
        }

        private static void Rebuild()
        {
            segments.Clear();
            bucketsByColumn.Clear();

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.ModNPC is not TileCollisionNPC tc || tc.colliders == null) continue;

                foreach (CollisionSurface surface in tc.colliders)
                {
                    if (surface == null || surface.endPoints == null || surface.endPoints.Length < 2) continue;

                    Vector2 a = surface.endPoints[0];
                    Vector2 b = surface.endPoints[1];
                    if (b.X < a.X) { Vector2 t = a; a = b; b = t; }

                    Segment seg = new Segment
                    {
                        a = a,
                        b = b,
                        minX = a.X,
                        maxX = b.X,
                        minY = a.Y < b.Y ? a.Y : b.Y,
                        maxY = a.Y > b.Y ? a.Y : b.Y
                    };

                    int segIdx = segments.Count;
                    segments.Add(seg);

                    int colStart = (int)Math.Floor(seg.minX / ColumnSize);
                    int colEnd = (int)Math.Floor(seg.maxX / ColumnSize);
                    for (int col = colStart; col <= colEnd; col++)
                    {
                        if (!bucketsByColumn.TryGetValue(col, out List<int> list))
                        {
                            list = new List<int>();
                            bucketsByColumn[col] = list;
                        }
                        list.Add(segIdx);
                    }
                }
            }
        }

        /// <summary>
        /// Highest collider surface at the given world X whose Y is at or below the probe Y.
        /// Returns false when no such surface exists at this column.
        /// </summary>
        public static bool TryGetGroundBeneath(float worldX, float atOrBelowY, out float surfaceY)
        {
            surfaceY = 0f;
            if (!Enabled) return false;
            bool found = false;
            int col = (int)Math.Floor(worldX / (float)ColumnSize);
            if (!bucketsByColumn.TryGetValue(col, out List<int> idxs)) return false;

            foreach (int i in idxs)
            {
                Segment s = segments[i];
                if (worldX < s.minX || worldX > s.maxX) continue;
                if (!CollisionGeometry.TryGetSurfaceHeight(s.a, s.b, worldX, out float y)) continue;
                if (y < atOrBelowY) continue;
                if (!found || y < surfaceY)
                {
                    surfaceY = y;
                    found = true;
                }
            }

            return found;
        }

        /// <summary>
        /// True when the NPC's feet at the given world X crossed a collider surface downward
        /// between the previous and current frame.
        /// </summary>
        public static bool TryGetLandingThisFrame(float worldX, float prevFeetY, float curFeetY, out float surfaceY)
        {
            surfaceY = 0f;
            if (!Enabled) return false;
            bool found = false;
            if (curFeetY < prevFeetY) return false;

            int col = (int)Math.Floor(worldX / (float)ColumnSize);
            if (!bucketsByColumn.TryGetValue(col, out List<int> idxs)) return false;

            foreach (int i in idxs)
            {
                Segment s = segments[i];
                if (worldX < s.minX || worldX > s.maxX) continue;
                if (!CollisionGeometry.TryGetSurfaceHeight(s.a, s.b, worldX, out float y)) continue;
                if (y < prevFeetY || y > curFeetY) continue;
                if (!found || y < surfaceY)
                {
                    surfaceY = y;
                    found = true;
                }
            }

            return found;
        }

        /// <summary>
        /// True when any collider surface is within the given vertical tolerance directly below feetY.
        /// </summary>
        public static bool HasGroundUnderfoot(float worldX, float feetY, float tolerance)
        {
            if (!Enabled) return false;
            int col = (int)Math.Floor(worldX / (float)ColumnSize);
            if (!bucketsByColumn.TryGetValue(col, out List<int> idxs)) return false;

            foreach (int i in idxs)
            {
                Segment s = segments[i];
                if (worldX < s.minX || worldX > s.maxX) continue;
                if (!CollisionGeometry.TryGetSurfaceHeight(s.a, s.b, worldX, out float y)) continue;
                if (y >= feetY && y <= feetY + tolerance) return true;
            }

            return false;
        }

        /// <summary>
        /// Enumerates every walkable segment that overlaps the given world region.
        /// </summary>
        public static IEnumerable<(Vector2 a, Vector2 b)> GetWalkableSurfaces(Rectangle region)
        {
            if (!Enabled) yield break;
            int colStart = (int)Math.Floor(region.Left / (float)ColumnSize);
            int colEnd = (int)Math.Floor(region.Right / (float)ColumnSize);
            HashSet<int> seen = new HashSet<int>();

            for (int col = colStart; col <= colEnd; col++)
            {
                if (!bucketsByColumn.TryGetValue(col, out List<int> idxs)) continue;
                foreach (int i in idxs)
                {
                    if (!seen.Add(i)) continue;
                    Segment s = segments[i];
                    if (s.maxX < region.Left || s.minX > region.Right) continue;
                    if (s.maxY < region.Top || s.minY > region.Bottom) continue;
                    yield return (s.a, s.b);
                }
            }
        }
    }
}
