using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;
using Terraria.Audio;
using OvermorrowMod.Core;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Players;
using OvermorrowMod.Content.Items.Accessories.CapturedMirage;

namespace OvermorrowMod.Common.VanillaOverrides.Bow
{
    public abstract partial class HeldBow : ModProjectile
    {
        private bool IsPowerShot() => flashCounter >= 6 && flashCounter <= 36;

        /// <summary>
        /// Consumes the given ammo if allowed and handles any exception cases
        /// </summary>
        private void ConsumeAmmo()
        {
            if (!CanConsumeAmmo(player)) return;

            if (player.inventory[AmmoSlotID].type != ItemID.EndlessQuiver)
                player.inventory[AmmoSlotID].stack--;
        }

        /// <summary>
        /// Loops through the ammo slots, loads in the first arrow found into the bow.
        /// </summary>
        /// <returns></returns>
        private bool FindAmmo()
        {
            LoadedArrowItemType = -1;
            if (ConvertArrow != ItemID.None) // There is an arrow given for conversion, try to find that arrow.
            {
                for (int i = 0; i <= 3; i++)
                {
                    Item item = player.inventory[54 + i];
                    if (item.type == ItemID.None || item.ammo != AmmoID.Arrow) continue;

                    // The arrow needed to convert is found, so convert the arrow and exit the loop.
                    if (item.type == ConvertArrow)
                    {
                        LoadedArrowType = ArrowType;
                        LoadedArrowItemType = item.type;

                        AmmoSlotID = 54 + i;

                        return true;
                    }
                }
            }

            // If here, then there is no conversion arrow OR no conversion arrow was found.
            // Thus, run the default behavior to find any arrows to fire.
            if (LoadedArrowItemType == -1)
            {
                for (int i = 0; i <= 3; i++)
                {
                    Item item = player.inventory[54 + i];
                    if (item.type == ItemID.None || item.ammo != AmmoID.Arrow) continue;

                    LoadedArrowType = item.shoot;
                    LoadedArrowItemType = item.type;

                    AmmoSlotID = 54 + i;

                    return true;
                }
            }
            //if (LoadedArrowItemType == -1) Main.NewText("No ammo found.");

            return false;
        }


        private int GetRandomArrow()
        {
            for (int i = 0; i <= 3; i++)
            {
                Item item = player.inventory[54 + i];
                if (item.type == ItemID.None || item.ammo != AmmoID.Arrow) continue;

                // The arrow needed to convert is found, so convert the arrow and exit the loop.
                if (item.shoot != LoadedArrowType)
                {
                    return item.shoot;
                }
            }

            return LoadedArrowType;
        }
    }
}