using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Globals;

namespace OvermorrowMod.Common.Weapons.Guns
{
    public abstract partial class HeldGun : ModProjectile
    {
        /// <summary>
        /// Gets a specific digit from a number.
        /// </summary>
        public int GetPlace(int value, int place)
        {
            return ((value % (place * 10)) - (value % place)) / place;
        }

        /// <summary>
        /// Gets the appropriate bullet texture based on gun type.
        /// </summary>
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

        /// <summary>
        /// Loads gun info from player data.
        /// </summary>
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

        /// <summary>
        /// Saves gun info to player data.
        /// </summary>
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
        /// Consumes the given ammo if allowed and handles any exception cases.
        /// </summary>
        private void ConsumeAmmo()
        {
            if (!CanConsumeAmmo(player)) return;

            if (player.inventory[AmmoSlotID].type != ItemID.EndlessMusketPouch)
                player.inventory[AmmoSlotID].stack--;
        }

        /// <summary>
        /// Reloads the bullet display with the current MaxShots.
        /// </summary>
        private void ReloadBulletDisplay()
        {
            BulletDisplay.Clear(); // Clear existing bullets to prevent duplicates

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
            while (BulletDisplay.Count > MaxShots + BonusAmmo)
            {
                BulletDisplay.RemoveAt(BulletDisplay.Count - 1);
            }
        }

        /// <summary>
        /// Removes a BulletObject from the player's list of bullets.
        /// </summary>
        public void UpdateBulletDisplay()
        {
            // Create a new list to avoid collection modified exception
            List<BulletObject> activeBullets = new List<BulletObject>();

            foreach (BulletObject bullet in BulletDisplay)
            {
                if (bullet.isActive)
                {
                    activeBullets.Add(bullet);
                }
            }

            BulletDisplay = activeBullets;
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

        /// <summary>
        /// Checks if all reload zones have been clicked successfully.
        /// </summary>
        private bool CheckEventSuccess()
        {
            foreach (ReloadZone clickZone in _clickZones)
            {
                if (!clickZone.HasClicked) return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the number of reload zones left to click.
        /// </summary>
        private int GetClicksLeft()
        {
            var numLeft = ClickZones.Count;
            foreach (ReloadZone clickZone in _clickZones)
            {
                if (clickZone.HasClicked) numLeft--;
            }

            return numLeft;
        }

        /// <summary>
        /// Resets all reload zones to unclicked state.
        /// </summary>
        private void ResetReloadZones()
        {
            foreach (ReloadZone clickZone in _clickZones)
            {
                clickZone.HasClicked = false;
            }
        }

        /// <summary>
        /// Checks if a click percentage is within any reload zone.
        /// </summary>
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

        // Cannot make this static and put this into GunEffects because spawning gores is stupid
        protected void SpawnBulletCasing(Projectile projectile, Player player, Vector2 position, Vector2 offset = default, float scale = 0.75f, bool sticky = true)
        {
            Vector2 velocity = new Vector2(player.direction * -0.03f, 0.01f);
            int gore = Gore.NewGore(null, position + offset, velocity, Mod.Find<ModGore>("BulletCasing").Type, scale);

            Main.gore[gore].sticky = sticky;
        }

        /// <summary>
        /// Drops multiple bullet casings at once, used for reload effects
        /// </summary>
        protected void DropMultipleCasings(Projectile projectile, Player player, int count)
        {
            for (int i = 0; i < count; i++)
            {
                SpawnBulletCasing(projectile, player, projectile.Center);
            }
        }
    }
}