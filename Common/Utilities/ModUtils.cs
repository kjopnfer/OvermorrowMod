using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Tooltips;
using OvermorrowMod.Common.Items.Guns;
using OvermorrowMod.Core.Globals;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Utilities
{
    /// <summary>
    /// General utilies that are not restricted to a particular type.
    /// </summary>
    public static class ModUtils
    {
        /// <summary>
        /// Adds health regeneration at the specified rate per second.
        /// </summary>
        /// <param name="player">The player to modify</param>
        /// <param name="healthPerSecond">Health regenerated per second</param>
        public static void AddHealthRegenPerSecond(this Player player, float healthPerSecond)
        {
            // 1 lifeRegen = 0.5 health per second, so multiply by 2
            player.lifeRegen += (int)(healthPerSecond * 2);
        }

        /// <summary>
        /// Adds mana regeneration at the specified rate per second.
        /// </summary>
        /// <param name="player">The player to modify</param>
        /// <param name="manaPerSecond">Mana regenerated per second</param>
        public static void AddManaRegenPerSecond(this Player player, float manaPerSecond)
        {
            // Assuming same pattern as health: 1 manaRegen = 0.5 mana per second
            player.manaRegen += (int)(manaPerSecond * 2);
        }

        public static bool HasNearbyPlayers(Vector2 position, float maxDistance)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead)
                {
                    if (Vector2.Distance(player.Center, position) <= maxDistance)
                        return true;
                }
            }
            return false;
        }

        public static bool HasNearbyNPCs(Vector2 position, float maxDistance)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.townNPC && npc.life > 0 && npc.CanBeChasedBy())
                {
                    if (Vector2.Distance(npc.Center, position) <= maxDistance)
                        return true;
                }
            }
            return false;
        }

        public static NPC FindNearestNPC(this Entity entity)
        {
            NPC nearestNPC = null;
            float nearestDistance = float.MaxValue;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.townNPC && npc.life > 0 && !npc.friendly && npc.CanBeChasedBy())
                {
                    float distance = Vector2.Distance(entity.Center, npc.Center);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestNPC = npc;
                    }
                }
            }

            return nearestNPC;
        }

        public static bool CheckEntityBottomSlopeCollision(this Entity entity)
        {
            Tile bottomLeftTile = Main.tile[(int)entity.Hitbox.BottomLeft().X / 16, (int)entity.Hitbox.BottomLeft().Y / 16];
            Tile bottomRightTile = Main.tile[(int)entity.Hitbox.BottomRight().X / 16, (int)entity.Hitbox.BottomRight().Y / 16];

            return (bottomLeftTile.HasTile && Main.tileSolid[bottomLeftTile.TileType]) || (bottomRightTile.HasTile && Main.tileSolid[bottomRightTile.TileType]);
        }

        /// <summary>
        /// Determines the direction (-1 or 1) of the target entity relative to the source entity.
        /// Returns 1 if the target is to the right, -1 if to the left.
        /// </summary>
        /// <param name="source">The entity determining the direction.</param>
        /// <param name="target">The entity whose position is compared.</param>
        /// <returns>-1 if the target is to the left, 1 if to the right.</returns>
        public static int GetDirectionFrom(this Entity source, Entity target)
        {
            return target.Center.X > source.Center.X ? 1 : -1;
        }

        /// <summary>
        /// Determines the direction (-1 or 1) of a target position relative to the source entity.
        /// Returns 1 if the position is to the right, -1 if to the left.
        /// </summary>
        /// <param name="source">The entity determining the direction.</param>
        /// <param name="position">The world position to compare.</param>
        /// <returns>-1 if the position is to the left, 1 if to the right.</returns>
        public static int GetDirectionFrom(this Entity source, Vector2 position)
        {
            return position.X > source.Center.X ? 1 : -1;
        }

        /// <summary>
        /// Converts seconds to ticks.
        /// </summary>
        /// <param name="seconds">The number of seconds.</param>
        /// <returns>The equivalent number of ticks.</returns>
        public static int SecondsToTicks(float seconds)
        {
            return (int)(seconds * 60f);
        }

        /// <summary>
        /// Converts the number of tiles to pixels, can be used for distance checks.
        /// </summary>
        /// <param name="seconds">The number of tiles.</param>
        /// <returns>The equivalent number of pixels.</returns>
        public static int TilesToPixels(float tiles)
        {
            return (int)(tiles * 16f);
        }

        #region Weapons
        /// <summary>
        /// Loops through the player's inventory and then places any suitable ammo types into the ammo slots if they are empty or the wrong ammo type.
        /// </summary>
        public static void AutofillAmmoSlots(Player player, int ammoID)
        {
            for (int j = 0; j <= 3; j++) // Check if any of the ammo slots are empty or are the right ammo
            {
                Item ammoItem = player.inventory[54 + j];
                if (ammoItem.type != ItemID.None && ammoItem.ammo == ammoID) continue;

                // Loop through the player's inventory in order to find any useable ammo types to use
                for (int i = 0; i <= 49; i++)
                {
                    Item item = player.inventory[i];
                    if (item.type == ItemID.None || item.ammo != ammoID) continue;

                    Item tempItem = ammoItem;
                    player.inventory[54 + j] = item;
                    player.inventory[i] = tempItem;

                    break;
                }
            }
        }
        #endregion

        #region Tooltips
        /// <summary>
        /// Quick method to create a simple projectile tooltip
        /// </summary>
        public static List<TooltipEntity> CreateProjectileTooltip(this ModItem item, string title, string[] description, float damage, DamageClass damageClass)
        {
            return new List<TooltipEntity>
            {
                TooltipEntity.CreateProjectileTooltip(title, description, damage, damageClass)
            };
        }

        /// <summary>
        /// Quick method to create a simple buff tooltip
        /// </summary>
        public static List<TooltipEntity> CreateBuffTooltip(this ModItem item, string title, string[] description, float duration, bool isBuff = true)
        {
            return new List<TooltipEntity>
            {
                TooltipEntity.CreateBuffTooltip(title, description, duration, isBuff ? BuffTooltipType.Buff : BuffTooltipType.Debuff)
            };
        }

        /// <summary>
        /// Combine multiple tooltip creation methods
        /// </summary>
        public static List<TooltipEntity> CreateMultipleTooltips(this ModItem item, params TooltipEntity[] tooltips)
        {
            return new List<TooltipEntity>(tooltips);
        }
        #endregion
    }
}