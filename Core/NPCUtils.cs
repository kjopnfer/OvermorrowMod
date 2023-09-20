using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Players;
using OvermorrowMod.Common.TilePiles;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Microsoft.Xna.Framework.Input;

namespace OvermorrowMod.Core
{
    public enum Direction
    {
        Left = 0,
        Right = 1,
        Up = 2,
        Down = 3,
    }

    public static partial class ModUtils
    {
        public static bool CheckEntityBottomSlopeCollision(this Entity entity)
        {
            Tile bottomLeftTile = Main.tile[(int)entity.Hitbox.BottomLeft().X / 16, (int)entity.Hitbox.BottomLeft().Y / 16];
            Tile bottomRightTile = Main.tile[(int)entity.Hitbox.BottomRight().X / 16, (int)entity.Hitbox.BottomRight().Y / 16];

            return (bottomLeftTile.HasTile && Main.tileSolid[bottomLeftTile.TileType]) || (bottomRightTile.HasTile && Main.tileSolid[bottomRightTile.TileType]);
        }


        #region Vanilla Code Adaptions
        /// <summary>
        /// Moves the npc to a Vector2.
        /// The lower the turnResistance, the less time it takes to adjust direction.
        /// Example: npc.MoveToPlayer(new Vector2(100, 0), 10, 14);
        /// toPlayer makes the vector consider the player.Center for you.
        /// </summary>
        public static void FlyTo(this NPC npc, Vector2 targetPosition, float speed, float turnResistance = 10f, bool reverse = false)
        {
            Vector2 move = targetPosition - npc.Center;
            float magnitude = Magnitude(move);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }

            move = ((reverse ? -npc.velocity : npc.velocity) * turnResistance + move) / (turnResistance + 1f);
            magnitude = Magnitude(move);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }

            npc.velocity = reverse ? -move : move;
        }

        // Adapted from Mod of Redemption
        /// <summary>
        /// 
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="targetPosition">The position for the NPC to move towards</param>
        /// <param name="moveInterval">The rate of increase for the NPC's speed</param>
        /// <param name="moveSpeed">The maximum movement speed of the NPC</param>
        /// <param name="maxJumpTilesX">The max number of tiles it can jump across</param>
        /// <param name="maxJumpTilesY">The max number of tiles it can jump over</param>
        /// <param name="jumpUpPlatforms">Whether or not the NPC can jump up platforms</param>
        /// <param name="target"></param>
        public static void HorizontallyMove(this NPC npc, Vector2 targetPosition, float moveInterval, float moveSpeed,
            int maxJumpTilesX, int maxJumpTilesY, bool jumpUpPlatforms, Entity target = null)
        {
            // If velocity is less than -1 or greater than 1...
            if (npc.velocity.X < -moveSpeed || npc.velocity.X > moveSpeed)
            {
                if (npc.velocity.Y == 0f) // ...and npc is not falling or jumping, slow down x velocity.
                {
                    npc.velocity *= 0.8f;
                }
            }
            else if (npc.velocity.X < moveSpeed && targetPosition.X > npc.Center.X) 
            {
                if (npc.confused && !npc.boss)
                {
                    npc.velocity.X -= moveInterval;
                    if (npc.velocity.X < -moveSpeed)
                    {
                        npc.velocity.X = -moveSpeed;
                    }
                }
                else
                {
                    npc.velocity.X += moveInterval;
                    if (npc.velocity.X > moveSpeed)
                    {
                        npc.velocity.X = moveSpeed;
                    }
                }
            }
            else if (npc.velocity.X > -moveSpeed && targetPosition.X < npc.Center.X) 
            {
                if (npc.confused && !npc.boss)
                {
                    npc.velocity.X += moveInterval;
                    if (npc.velocity.X < moveSpeed)
                    {
                        npc.velocity.X = moveSpeed;
                    }
                }
                else
                {
                    npc.velocity.X -= moveInterval;
                    if (npc.velocity.X < -moveSpeed)
                    {
                        npc.velocity.X = -moveSpeed;
                    }
                }
            }

            npc.WalkupHalfBricks(ref npc.gfxOffY, ref npc.stepSpeed);

            // If there's a solid floor underneath the NPC...
            if (npc.HitTileOnSide((int)Direction.Down))
            {
                // If the npc's velocity is going in the same direction as the npc's direction...
                if (npc.velocity.X < 0f && npc.direction == -1 || npc.velocity.X > 0f && npc.direction == 1)
                {
                    // ...attempt to jump if needed.
                    Vector2 newVec = npc.AttemptJump(maxJumpTilesX, maxJumpTilesY, moveSpeed, jumpUpPlatforms, target);
                    if (!npc.noTileCollide)
                    {
                        Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
                        newVec = Collision.TileCollision(npc.position, newVec, npc.width, npc.height);
                        Vector4 slopeVec = Collision.SlopeCollision(npc.position, newVec, npc.width, npc.height);
                        Vector2 slopeVel = new(slopeVec.Z, slopeVec.W);
                        npc.position = new Vector2(slopeVec.X, slopeVec.Y);
                        npc.velocity = slopeVel;
                    }

                    if (npc.velocity != newVec)
                    {
                        npc.velocity = newVec;
                        npc.netUpdate = true;
                    }
                }
            }
        }

        public static void WalkupHalfBricks(this Entity entity, ref float gfxOffY, ref float stepSpeed)
        {
            if (entity == null) return;

            if (entity.velocity.Y >= 0f)
            {
                int offset = 0;
                if (entity.velocity.X < 0f) offset = -1;
                if (entity.velocity.X > 0f) offset = 1;
                Vector2 position = entity.position;
                position.X += entity.velocity.X;
                int tileX = (int)((position.X + (double)(entity.width / 2) + (entity.width / 2 + 1) * offset) / 16.0);
                int tileY = (int)((position.Y + (double)entity.height - 1.0) / 16.0);

                if (tileX * 16 < position.X + (double)entity.width && tileX * 16 + 16 > (double)position.X && (Main.tile[tileX, tileY].HasUnactuatedTile && Main.tile[tileX, tileY].Slope == 0 && Main.tile[tileX, tileY - 1].Slope == 0 && Main.tileSolid[Main.tile[tileX, tileY].TileType] && !Main.tileSolidTop[Main.tile[tileX, tileY].TileType] || Main.tile[tileX, tileY - 1].IsHalfBlock && Main.tile[tileX, tileY - 1].HasUnactuatedTile) && (!Main.tile[tileX, tileY - 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX, tileY - 1].TileType] || Main.tileSolidTop[Main.tile[tileX, tileY - 1].TileType] || Main.tile[tileX, tileY - 1].IsHalfBlock && (!Main.tile[tileX, tileY - 4].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX, tileY - 4].TileType] || Main.tileSolidTop[Main.tile[tileX, tileY - 4].TileType])) && (!Main.tile[tileX, tileY - 2].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX, tileY - 2].TileType] || Main.tileSolidTop[Main.tile[tileX, tileY - 2].TileType]) && (!Main.tile[tileX, tileY - 3].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX, tileY - 3].TileType] || Main.tileSolidTop[Main.tile[tileX, tileY - 3].TileType]) && (!Main.tile[tileX - offset, tileY - 3].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX - offset, tileY - 3].TileType]))
                {
                    float tileWorldY = tileY * 16;
                    if (Main.tile[tileX, tileY].IsHalfBlock) tileWorldY += 8f;
                    if (Main.tile[tileX, tileY - 1].IsHalfBlock) tileWorldY -= 8f;

                    if (tileWorldY < position.Y + (double)entity.height)
                    {
                        float tileWorldYHeight = position.Y + entity.height - tileWorldY;
                        float heightNeeded = 16.1f;
                        if (tileWorldYHeight <= (double)heightNeeded)
                        {
                            gfxOffY += entity.position.Y + entity.height - tileWorldY;
                            entity.position.Y = tileWorldY - entity.height;
                            stepSpeed = tileWorldYHeight >= 9.0 ? 2f : 1f;
                        }
                    }
                    else
                    {
                        gfxOffY = Math.Max(0f, gfxOffY - stepSpeed);
                    }
                }
                else
                {
                    gfxOffY = Math.Max(0f, gfxOffY - stepSpeed);
                }
            }
            else
            {
                gfxOffY = Math.Max(0f, gfxOffY - stepSpeed);
            }
        }

        // Adapted from Mod of Redemption
        /// <summary>
        /// Code based on vanilla jumping code, checks for and attempts to jump over tiles and gaps when needed.
        /// </summary>
        /// <param name="tileDistX">The tile amount that the NPC can jump across</param>
        /// <param name="tileDistY">The tile amount that the NPC can jump over</param>
        /// <param name="maxSpeedX">The maximum speed of the NPC</param>
        /// <param name="jumpUpPlatforms"></param>
        /// <param name="target"></param>
        /// <param name="ignoreTiles"></param>
        /// <returns>A new velocity based on all calculated factors</returns>
        public static Vector2 AttemptJump(this NPC npc, int tileDistX = 3, int tileDistY = 4, float maxSpeedX = 1f, bool jumpUpPlatforms = false, Entity target = null, bool ignoreTiles = false)
        {
            try
            {
                tileDistX -= 2;
                Vector2 newVelocity = npc.velocity;
                int tileX = Math.Max(10, Math.Min(Main.maxTilesX - 10, (int)((npc.position.X + npc.width * 0.5f + (npc.width * 0.5f + 8f) * npc.direction) / 16f)));
                int tileY = Math.Max(10, Math.Min(Main.maxTilesY - 10, (int)((npc.position.Y + npc.height - 15f) / 16f)));

                int tileHeight = (int)(npc.height / 16f);
                if (npc.height > tileHeight * 16) tileHeight += 1;

                //attempt to jump over walls if possible.

                if (ignoreTiles && target != null && Math.Abs(npc.position.X + npc.width * 0.5f - target.Center.X) < npc.width + 120)
                {
                    float dist = (int)Math.Abs(npc.position.Y + npc.height * 0.5f - target.Center.Y) / 16;
                    if (dist < tileDistY + 2) newVelocity.Y = -8f + dist * -0.5f;
                }

                if (newVelocity.Y == npc.velocity.Y)
                {
                    npc.ModifyJumpVelocityY(ref newVelocity, tileX, tileY, tileDistY, jumpUpPlatforms, target);
                }

                // If the npc isn't jumping already...
                if (newVelocity.Y == npc.velocity.Y)
                {
                    // ...and there's a gap in front of the npc, attempt to jump across it.
                    if (npc.CheckGap(tileX, tileY, target))
                    {
                        npc.ModifyJumpVelocityX(ref newVelocity, tileX, tileY, tileDistX, maxSpeedX);
                    }
                }

                return newVelocity;
            }
            catch (Exception e)
            {
                OvermorrowModFile.Instance.Logger.Error("ATTEMPT JUMP ERROR:", e);
                return npc.velocity;
            }
        }

        private static void ModifyJumpVelocityY(this NPC npc, ref Vector2 velocity, int tileX, int tileY, int tileDistY, bool jumpUpPlatforms, Entity target)
        {
            Rectangle hitbox = new((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
            int tileIteratorY = Math.Max(10, Math.Min(Main.maxTilesY - 10, tileY - tileDistY));

            for (int y = tileY; y >= tileIteratorY; y--)
            {
                Tile tile = Framing.GetTileSafely(tileX, y);
                Tile tileNear = Main.tile[Math.Min(Main.maxTilesX, tileX - npc.direction), y];
                if (tile.HasUnactuatedTile && (y != tileY || tile.Slope == 0) && Main.tileSolid[tile.TileType] && (jumpUpPlatforms || !Main.tileSolidTop[tile.TileType]))
                {
                    if (!Main.tileSolidTop[tile.TileType])
                    {
                        Rectangle tileHitbox = new(tileX * 16, y * 16, 16, 16)
                        {
                            Y = hitbox.Y
                        };

                        if (tileHitbox.Intersects(hitbox))
                        {
                            velocity = npc.velocity;
                            break;
                        }
                    }

                    if (tileNear.HasUnactuatedTile && Main.tileSolid[tileNear.TileType] && !Main.tileSolidTop[tileNear.TileType])
                    {
                        velocity = npc.velocity;
                        break;
                    }

                    if (target != null && y * 16 < target.Center.Y) continue;
                    velocity.Y = -(5f + (tileY - y) * (tileY - y > 3 ? 1f - (tileY - y - 2) * 0.0525f : 1f));
                }
            }
        }

        private static void ModifyJumpVelocityX(this NPC npc, ref Vector2 velocity, int tileX, int tileY, int tileDistX, float maxSpeedX)
        {
            int tileIteratorX = Math.Max(10, Math.Min(Main.maxTilesX - 10, tileX + npc.direction * tileDistX));

            velocity.Y = -8f;
            velocity.X *= 1.5f * (1f / maxSpeedX);

            if (tileX <= tileIteratorX)
            {
                for (int x = tileX; x < tileIteratorX; x++)
                {
                    Tile tile = Framing.GetTileSafely(x, tileY + 1);
                    if (x != tileX && !tile.HasUnactuatedTile)
                    {
                        velocity.Y -= 0.0325f;
                        velocity.X += npc.direction * 0.255f;
                    }
                }
            }
            else if (tileX > tileIteratorX)
            {
                for (int x = tileIteratorX; x < tileX; x++)
                {
                    Tile tile = Framing.GetTileSafely(x, tileY + 1);
                    if (x != tileIteratorX && !tile.HasUnactuatedTile)
                    {
                        velocity.Y -= 0.0325f;
                        velocity.X += npc.direction * 0.255f;
                    }
                }
            }
        }


        // Adapted from Mod of Redemption
        /// <summary>
        /// Checks if a Entity object (Player, NPC, Item or Projectile) has hit a tile on it's sides.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="direction">The direction to check collision for</param>
        /// <param name="noYMovement">If true, will not calculate unless the Entity is not moving on the Y axis.</param>
        public static bool HitTileOnSide(this Entity entity, int direction, bool noYMovement = true)
        {
            if (!noYMovement || entity.velocity.Y == 0f)
            {
                Vector2 dummyVec = default;
                return HitTileOnSide(entity.Hitbox, direction, ref dummyVec);
            }

            return false;
        }

        // Adapted from Mod of Redemption
        /// <summary>
        /// Checks if a bounding box has hit a tile on it's sides.
        /// </summary>
        public static bool HitTileOnSide(Rectangle boundingBox, int direction, ref Vector2 hitTilePos)
        {
            Vector2 position = new Vector2(boundingBox.X, boundingBox.Y);
            int width = boundingBox.Width;
            int height = boundingBox.Height;
            switch (direction)
            {
                case (int)Direction.Left:
                    if (Collision.SolidCollision(position - new Vector2(1, 0), 8, height))
                        return true;
                    break;
                case (int)Direction.Right:
                    if (Collision.SolidCollision(position + new Vector2(width - 8, 0), 9, height))
                        return true;
                    break;
                case (int)Direction.Up:
                    if (Collision.SolidCollision(position - new Vector2(0, 1), width, 8))
                        return true;
                    break;
                case (int)Direction.Down:
                    if (Collision.SolidCollision(position + new Vector2(0, height - 8), width, 9, true))
                        return true;
                    break;
            }

            int tilePosX = 0;
            int tilePosY = 0;
            int tilePosWidth = 0;
            int tilePosHeight = 0;

            switch (direction)
            {
                case (int)Direction.Left:
                    tilePosX = (int)(position.X - 8f) / 16;
                    tilePosY = (int)position.Y / 16;
                    tilePosWidth = tilePosX + 1;
                    tilePosHeight = (int)(position.Y + height) / 16;
                    break;
                case (int)Direction.Right:
                    tilePosX = (int)(position.X + width + 8f) / 16;
                    tilePosY = (int)position.Y / 16;
                    tilePosWidth = tilePosX + 1;
                    tilePosHeight = (int)(position.Y + height) / 16;
                    break;
                case (int)Direction.Up:
                    tilePosX = (int)position.X / 16;
                    tilePosY = (int)(position.Y - 8f) / 16;
                    tilePosWidth = (int)(position.X + width) / 16;
                    tilePosHeight = tilePosY + 1;
                    break;
                case (int)Direction.Down:
                    tilePosX = (int)position.X / 16;
                    tilePosY = (int)(position.Y + height + 8f) / 16;
                    tilePosWidth = (int)(position.X + width) / 16;
                    tilePosHeight = tilePosY + 1;
                    break;
            }

            for (int x = tilePosX; x < tilePosWidth; x++)
            {
                for (int y = tilePosY; y < tilePosHeight; y++)
                {
                    if (Framing.GetTileSafely(x, y) == null) return false;

                    bool solidTop = direction == (int)Direction.Down && Main.tileSolidTop[Framing.GetTileSafely(x, y).TileType];
                    if (Framing.GetTileSafely(x, y).HasUnactuatedTile && (Main.tileSolid[Framing.GetTileSafely(x, y).TileType] || solidTop))
                    {
                        hitTilePos = new Vector2(x, y);
                        return true;
                    }
                }
            }
            return false;
        }
        
        /// <summary>
        /// Checks to see if there is a gap in front of the NPC
        /// </summary>
        private static bool CheckGap(this NPC npc, int tileX, int tileY, Entity target)
        {
            if (npc.directionY < 0 && (!Main.tile[tileX, tileY + 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX, tileY + 1].TileType]) && (!Main.tile[tileX + npc.direction, tileY + 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX + npc.direction, tileY + 1].TileType]))
            {
                if (!Main.tile[tileX + npc.direction, tileY + 2].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX, tileY + 2].TileType] || target == null || target.Center.Y + target.height * 0.25f < tileY * 16f)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}