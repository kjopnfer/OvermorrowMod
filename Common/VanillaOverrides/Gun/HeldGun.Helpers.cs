using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria.DataStructures;
using System.IO;
using System;

namespace OvermorrowMod.Common.VanillaOverrides.Gun
{
    public abstract partial class HeldGun : ModProjectile
    {
        public int GetPlace(int value, int place)
        {
            return ((value % (place * 10)) - (value % place)) / place;
        }

        private string BulletTexture()
        {
            switch (GunType)
            {
                case GunType.Shotgun:
                    return "GunBullet_Shotgun";
                default:
                    return "GunBullet";
            }
        }

        private void LoadGunInfo()
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();

            if (gunPlayer.playerGunInfo.ContainsKey(ParentItem))
            {
                ShotsFired = gunPlayer.playerGunInfo[ParentItem].shotsFired;
                BonusBullets = gunPlayer.playerGunInfo[ParentItem].bonusBullets;
                BonusDamage = gunPlayer.playerGunInfo[ParentItem].bonusDamage;
                BonusAmmo = gunPlayer.playerGunInfo[ParentItem].bonusAmmo;
            }
        }
        private void SaveGunInfo()
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();

            if (!gunPlayer.playerGunInfo.ContainsKey(ParentItem))
            {
                gunPlayer.playerGunInfo.Add(ParentItem, new HeldGunInfo(ShotsFired, BonusBullets, BonusDamage, BonusAmmo));
            }
            else
            {
                gunPlayer.playerGunInfo[ParentItem] = new HeldGunInfo(ShotsFired, BonusBullets, BonusDamage, BonusAmmo);
            }
        }

        /// <summary>
        /// Loops through the ammo slots, loads in the first bullet found into the bow.
        /// </summary>
        private bool FindAmmo()
        {
            LoadedBulletItemType = -1;
            if (ConvertBullet != ItemID.None) // There is a bullet given for conversion, try to find that bullet.
            {
                for (int i = 0; i <= 3; i++)
                {
                    Item item = player.inventory[54 + i];
                    if (item.type == ItemID.None || item.ammo != AmmoID.Bullet) continue;

                    // The bullet needed to convert is found, so convert the bullet and exit the loop.
                    if (item.type == ConvertBullet)
                    {
                        LoadedBulletType = BulletType;
                        LoadedBulletItemType = item.type;

                        AmmoSlotID = 54 + i;

                        return true;
                    }
                }
            }

            // If here, then there is no conversion bullet OR no conversion bullet was found.
            // Thus, run the default behavior to find any bullets to fire.
            if (LoadedBulletItemType == -1)
            {
                for (int i = 0; i <= 3; i++)
                {
                    Item item = player.inventory[54 + i];
                    if (item.type == ItemID.None || item.ammo != AmmoID.Bullet) continue;

                    LoadedBulletType = item.shoot;
                    LoadedBulletItemType = item.type;

                    AmmoSlotID = 54 + i;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Consumes the given ammo if allowed and handles any exception cases
        /// </summary>
        private void ConsumeAmmo()
        {
            if (!CanConsumeAmmo(player)) return;

            if (player.inventory[AmmoSlotID].type != ItemID.EndlessMusketPouch)
                player.inventory[AmmoSlotID].stack--;
        }

        private void ReloadBulletDisplay()
        {
            for (int _ = 0; _ < MaxShots + BonusAmmo; _++)
            {
                BulletDisplay.Add(new BulletObject(BulletTexture(), Main.rand.Next(0, 9) * 7));
            }
        }

        /// <summary>
        /// Sometimes the thing arbitrarily decides to just have an additional bullet despite literally nothing adding to it.
        /// </summary>
        private void ForceCorrectBulletDisplay()
        {
            if (BulletDisplay.Count > MaxShots + BonusAmmo)
            {
                BulletDisplay.RemoveAt(BulletDisplay.Count - 1);
            }
        }

        /// <summary>
        /// Removes a BulletObject from the player's list of bullets.
        /// </summary>
        public void UpdateBulletDisplay()
        {
            List<BulletObject> removedList = BulletDisplay;

            for (int i = BulletDisplay.Count - 1; i >= 0; i--)
            {
                if (!BulletDisplay[i].isActive) removedList.RemoveAt(i);
            }

            BulletDisplay = removedList;
        }

        /// <summary>
        /// Deactivates a single bullet from the player's ammo. Does not remove from the player's bullet list.
        /// </summary>
        public void PopBulletDisplay()
        {
            for (int i = BulletDisplay.Count - 1; i >= 0; i--)
            {
                if (BulletDisplay[i].isActive && !BulletDisplay[i].startDeath)
                {
                    BulletDisplay[i].Deactivate();
                    return;
                }
            }
        }

        private bool CheckEventSuccess()
        {
            foreach (ReloadZone clickZone in _clickZones)
            {
                if (!clickZone.HasClicked) return false;
            }

            return true;
        }

        private int GetClicksLeft()
        {
            var numLeft = ClickZones.Count;
            foreach (ReloadZone clickZone in _clickZones)
            {
                if (clickZone.HasClicked) numLeft--;
            }

            return numLeft;
        }
        private void ResetReloadZones()
        {
            foreach (ReloadZone clickZone in _clickZones)
            {
                clickZone.HasClicked = false;
            }
        }

        private bool CheckInZone(float clickPercentage, out int zoneIndex)
        {
            clickPercentage = clickPercentage * 100;

            int zoneCounter = 0;
            foreach (ReloadZone clickZone in _clickZones)
            {
                if (clickPercentage >= clickZone.StartPercentage && clickPercentage <= clickZone.EndPercentage)
                {
                    zoneIndex = zoneCounter;
                    return true;
                }

                zoneCounter++;
            }

            zoneIndex = -1;

            return false;
        }
    }
}