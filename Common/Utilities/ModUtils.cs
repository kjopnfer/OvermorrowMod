using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Weapons.Guns;
using OvermorrowMod.Core.Globals;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Common.Utilities
{
    /// <summary>
    /// General utilies that are not restricted to a particular type.
    /// </summary>
    public static class ModUtils
    {
        /// <summary>
        /// Determines the direction (-1 or 1) of the target entity relative to the source entity.
        /// Returns 1 if the target is to the right, -1 if to the left.
        /// </summary>
        /// <param name="source">The entity determining the direction.</param>
        /// <param name="target">The entity whose position is compared.</param>
        /// <returns>-1 if the target is to the left, 1 if to the right.</returns>
        public static int GetDirection(this Entity source, Entity target)
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
        public static int GetDirection(this Entity source, Vector2 position)
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

        public static void SetWeaponType(this Item item, GunType gunType)
        {
            item.GetGlobalItem<GlobalGun>().GunType = gunType;
        }

        public static GunType GetWeaponType(this Item item)
        {
            return item.GetGlobalItem<GlobalGun>().GunType;
        }
    }
}