using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Globals;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Items.Guns
{
    public abstract partial class HeldGun
    {
        public Projectile LoadedBullet { private set; get; }
        public int LoadedBulletType { private set; get; }
        public int LoadedBulletItemType { private set; get; }

        private int AmmoSlotID;

        protected readonly struct Round
        {
            public readonly int ItemType;
            public readonly int ProjectileType;

            public Round(int itemType, int projectileType)
            {
                ItemType = itemType;
                ProjectileType = projectileType;
            }
        }

        private readonly List<Round> loadedRounds = new();

        /// <summary>
        /// Number of rounds currently chambered. Capacity is <see cref="MaxShots"/>.
        /// </summary>
        public int LoadedCount => loadedRounds.Count;

        /// <summary>
        /// Guns that reload draw from a pre-loaded chamber; machine guns fire straight from inventory.
        /// </summary>
        private bool UsesMagazine => CanReload();

        private void SyncMagazineCounter() => ShotsFired = Math.Max(0, MaxShots - loadedRounds.Count);

        protected List<BulletObject> BulletDisplay = new();

        private bool LoadGunInfo()
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();
            if (!gunPlayer.playerGunInfo.TryGetValue(ParentItem, out var info)) return false;

            loadedRounds.Clear();
            int count = Math.Min(info.loadedItemTypes.Count, info.loadedProjectileTypes.Count);
            for (int i = 0; i < count; i++)
                loadedRounds.Add(new Round(info.loadedItemTypes[i], info.loadedProjectileTypes[i]));

            return true;
        }

        private void SaveGunInfo()
        {
            if (!UsesMagazine) return;

            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();
            var itemTypes = new List<int>(loadedRounds.Count);
            var projectileTypes = new List<int>(loadedRounds.Count);
            foreach (var round in loadedRounds)
            {
                itemTypes.Add(round.ItemType);
                projectileTypes.Add(round.ProjectileType);
            }

            gunPlayer.playerGunInfo[ParentItem] = new HeldGunInfo(itemTypes, projectileTypes, CurrentStats.BonusBullets, CurrentStats.BonusDamage, CurrentStats.BonusAmmo);
        }

        /// <summary>
        /// Fills the chamber to capacity with factory rounds the first time the gun is equipped, without
        /// drawing from the inventory. Once these are spent, reloading pulls from the inventory as usual.
        /// </summary>
        private void PreloadMagazine()
        {
            loadedRounds.Clear();

            int itemType = ConvertBullet != ItemID.None ? ConvertBullet : ItemID.MusketBall;
            int projectileType = ConvertBullet != ItemID.None ? BulletType : ProjectileID.Bullet;

            for (int i = 0; i < MaxShots; i++)
                loadedRounds.Add(new Round(itemType, projectileType));
        }

        /// <summary>
        /// Pre-consumes ammo from the inventory into the chamber, topping the existing rounds up to capacity.
        /// Each round remembers its own ammo type so a magazine may hold a mix. The Endless Musket Pouch fills
        /// without depleting.
        /// </summary>
        private void LoadMagazine()
        {
            ModUtils.AutofillAmmoSlots(player, AmmoID.Bullet);

            int capacity = MaxShots;
            for (int i = 0; i <= 3 && loadedRounds.Count < capacity; i++)
            {
                Item item = player.inventory[54 + i];
                if (item.type == ItemID.None || item.ammo != AmmoID.Bullet) continue;

                bool endless = item.type == ItemID.EndlessMusketPouch;
                int projectileType = (ConvertBullet != ItemID.None && item.type == ConvertBullet) ? BulletType : item.shoot;

                while (loadedRounds.Count < capacity)
                {
                    loadedRounds.Add(new Round(item.type, projectileType));

                    if (endless || !CanConsumeAmmo(player)) continue;

                    item.stack--;
                    if (item.stack <= 0)
                    {
                        item.TurnToAir();
                        break;
                    }
                }
            }

            SyncMagazineCounter();
        }

        private bool FindAmmo()
        {
            LoadedBulletItemType = -1;
            if (ConvertBullet != ItemID.None)
            {
                for (int i = 0; i <= 3; i++)
                {
                    Item item = player.inventory[54 + i];
                    if (item.type == ItemID.None || item.ammo != AmmoID.Bullet) continue;

                    if (item.type == ConvertBullet)
                    {
                        LoadedBulletType = BulletType;
                        LoadedBulletItemType = item.type;
                        AmmoSlotID = 54 + i;
                        return true;
                    }
                }
            }

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

        private bool ShouldSaveAmmo()
        {
            float totalSaveChance = Math.Min(CurrentStats.AmmoSaveChance, 100f);
            return Main.rand.NextFloat(0f, 100f) < totalSaveChance;
        }

        private void ConsumeAmmo()
        {
            if (!CanConsumeAmmo(player)) return;

            if (player.inventory[AmmoSlotID].type != ItemID.EndlessMusketPouch)
                player.inventory[AmmoSlotID].stack--;
        }

        private string GetBulletTexture()
        {
            return CurrentStats.BulletUITexture;
        }

        private void ReloadBulletDisplay()
        {
            BulletDisplay.Clear();

            for (int i = 0; i < loadedRounds.Count; i++)
            {
                BulletDisplay.Add(new BulletObject(GetBulletTexture(), Main.rand.Next(0, 9) * 7));
            }
        }

        private void ForceCorrectBulletDisplay()
        {
            while (BulletDisplay.Count > MaxShots)
            {
                BulletDisplay.RemoveAt(BulletDisplay.Count - 1);
            }
        }

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
    }
}
