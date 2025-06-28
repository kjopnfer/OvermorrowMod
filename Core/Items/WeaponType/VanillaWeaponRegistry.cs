using MonoMod.Core.Utils;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Items
{
    // Static registry for vanilla weapon classifications
    public static class VanillaWeaponRegistry
    {
        private static Dictionary<int, WeaponType> _itemClassifications;
        private static Dictionary<int, WeaponType> _projectileClassifications;

        public static void Initialize()
        {
            InitializeItemClassifications();
            InitializeProjectileClassifications();
        }

        private static void InitializeItemClassifications()
        {
            _itemClassifications = new Dictionary<int, WeaponType>
            {

                // Greatswords/Large Swords
                [ItemID.BreakerBlade] = WeaponType.Greatsword,
                [ItemID.FieryGreatsword] = WeaponType.Greatsword,

                // Broadswords
                [ItemID.Frostbrand] = WeaponType.Broadsword,
                [ItemID.Gladius] = WeaponType.Broadsword,
                [ItemID.IceBlade] = WeaponType.Broadsword,
                [ItemID.Meowmere] = WeaponType.Broadsword,
                [ItemID.BeamSword] = WeaponType.Broadsword,
                [ItemID.BeeKeeper] = WeaponType.Broadsword,
                [ItemID.Bladetongue] = WeaponType.Broadsword,
                [ItemID.BloodButcherer] = WeaponType.Broadsword,
                [ItemID.BoneSword] = WeaponType.Broadsword,
                [ItemID.LightsBane] = WeaponType.Broadsword,
                [ItemID.BladeofGrass] = WeaponType.Broadsword,
                [ItemID.NightsEdge] = WeaponType.Broadsword,
                [ItemID.TrueNightsEdge] = WeaponType.Broadsword,
                [ItemID.Excalibur] = WeaponType.Broadsword,
                [ItemID.TrueExcalibur] = WeaponType.Broadsword,
                [ItemID.TerraBlade] = WeaponType.Broadsword,
                [ItemID.BorealWoodSword] = WeaponType.Broadsword,
                [ItemID.CactusSword] = WeaponType.Broadsword,
                [ItemID.Cutlass] = WeaponType.Broadsword,
                [ItemID.CopperBroadsword] = WeaponType.Broadsword,
                [ItemID.TinBroadsword] = WeaponType.Broadsword,
                [ItemID.IronBroadsword] = WeaponType.Broadsword,
                [ItemID.LeadBroadsword] = WeaponType.Broadsword,
                [ItemID.SilverBroadsword] = WeaponType.Broadsword,
                [ItemID.TungstenBroadsword] = WeaponType.Broadsword,
                [ItemID.GoldBroadsword] = WeaponType.Broadsword,
                [ItemID.PlatinumBroadsword] = WeaponType.Broadsword,
                [ItemID.MythrilSword] = WeaponType.Broadsword,
                [ItemID.OrichalcumSword] = WeaponType.Broadsword,
                [ItemID.PalladiumSword] = WeaponType.Broadsword,
                [ItemID.Seedler] = WeaponType.Broadsword,
                [ItemID.Starfury] = WeaponType.Broadsword,
                [ItemID.StarWrath] = WeaponType.Broadsword,
                [ItemID.TheHorsemansBlade] = WeaponType.Broadsword,
                [ItemID.TitaniumSword] = WeaponType.Broadsword,

                // Shortswords
                [ItemID.CopperShortsword] = WeaponType.Shortsword,
                [ItemID.TinShortsword] = WeaponType.Shortsword,
                [ItemID.IronShortsword] = WeaponType.Shortsword,
                [ItemID.LeadShortsword] = WeaponType.Shortsword,
                [ItemID.SilverShortsword] = WeaponType.Shortsword,
                [ItemID.TungstenShortsword] = WeaponType.Shortsword,
                [ItemID.GoldShortsword] = WeaponType.Shortsword,
                [ItemID.PlatinumShortsword] = WeaponType.Shortsword,

                // Bows
                [ItemID.WoodenBow] = WeaponType.Bow,
                [ItemID.BorealWoodBow] = WeaponType.Bow,
                [ItemID.RichMahoganyBow] = WeaponType.Bow,
                [ItemID.PalmWoodBow] = WeaponType.Bow,
                [ItemID.EbonwoodBow] = WeaponType.Bow,
                [ItemID.ShadewoodBow] = WeaponType.Bow,
                [ItemID.PearlwoodBow] = WeaponType.Bow,
                [ItemID.CopperBow] = WeaponType.Bow,
                [ItemID.TinBow] = WeaponType.Bow,
                [ItemID.IronBow] = WeaponType.Bow,
                [ItemID.LeadBow] = WeaponType.Bow,
                [ItemID.SilverBow] = WeaponType.Bow,
                [ItemID.TungstenBow] = WeaponType.Bow,
                [ItemID.GoldBow] = WeaponType.Bow,
                [ItemID.PlatinumBow] = WeaponType.Bow,
                [ItemID.DemonBow] = WeaponType.Bow,
                [ItemID.TendonBow] = WeaponType.Bow,
                [ItemID.BloodRainBow] = WeaponType.Bow,
                [ItemID.Marrow] = WeaponType.Bow,
                [ItemID.IceBow] = WeaponType.Bow,
                [ItemID.Tsunami] = WeaponType.Bow,

                // Guns - Revolvers
                [ItemID.Revolver] = WeaponType.Revolver,
                [ItemID.TheUndertaker] = WeaponType.Revolver,

                // Guns - Handguns
                [ItemID.FlintlockPistol] = WeaponType.Handgun,
                [ItemID.RedRyder] = WeaponType.Handgun,
                [ItemID.Handgun] = WeaponType.Handgun,
                [ItemID.PhoenixBlaster] = WeaponType.Handgun,

                // Guns - Shotguns
                [ItemID.Boomstick] = WeaponType.Shotgun,
                [ItemID.Shotgun] = WeaponType.Shotgun,
                [ItemID.QuadBarrelShotgun] = WeaponType.Shotgun,
                [ItemID.OnyxBlaster] = WeaponType.Shotgun,
                [ItemID.TacticalShotgun] = WeaponType.Shotgun,

                // Guns - Muskets
                [ItemID.Musket] = WeaponType.Musket,

                // Guns - Rifles
                [ItemID.SniperRifle] = WeaponType.Sniper,
                [ItemID.Uzi] = WeaponType.SubMachineGun,
                [ItemID.Megashark] = WeaponType.MachineGun,
                [ItemID.SDMG] = WeaponType.MachineGun,
                [ItemID.Minishark] = WeaponType.MachineGun,
                [ItemID.ChainGun] = WeaponType.MachineGun,
                [ItemID.ClockworkAssaultRifle] = WeaponType.Rifle,
                [ItemID.VenusMagnum] = WeaponType.Rifle,

                // Guns - Launchers
                [ItemID.GrenadeLauncher] = WeaponType.Launcher,
                [ItemID.RocketLauncher] = WeaponType.Launcher,
                [ItemID.ProximityMineLauncher] = WeaponType.Launcher,
                [ItemID.SnowmanCannon] = WeaponType.Launcher,
                [ItemID.StyngerBolt] = WeaponType.Launcher,
                [ItemID.ElectrosphereLauncher] = WeaponType.Launcher,

                // Staves
                [ItemID.AmethystStaff] = WeaponType.Staff,
                [ItemID.TopazStaff] = WeaponType.Staff,
                [ItemID.SapphireStaff] = WeaponType.Staff,
                [ItemID.EmeraldStaff] = WeaponType.Staff,
                [ItemID.RubyStaff] = WeaponType.Staff,
                [ItemID.DiamondStaff] = WeaponType.Staff,
                [ItemID.WandofSparking] = WeaponType.Staff,
                [ItemID.ThunderStaff] = WeaponType.Staff,
                [ItemID.FrostStaff] = WeaponType.Staff,

                // Whips
                [ItemID.BlandWhip] = WeaponType.Whip,
                [ItemID.SwordWhip] = WeaponType.Whip,
                [ItemID.MaceWhip] = WeaponType.Whip,
                [ItemID.ScytheWhip] = WeaponType.Whip,
                [ItemID.BoneWhip] = WeaponType.Whip,
                [ItemID.FireWhip] = WeaponType.Whip,
                [ItemID.CoolWhip] = WeaponType.Whip,
                [ItemID.ThornWhip] = WeaponType.Whip,
                [ItemID.RainbowWhip] = WeaponType.Whip,
                [ItemID.ScytheWhip] = WeaponType.Whip,
                [ItemID.MaceWhip] = WeaponType.Whip,
                [ItemID.RainbowWhip] = WeaponType.Whip,
            };
        }

        private static void InitializeProjectileClassifications()
        {
            _projectileClassifications = new Dictionary<int, WeaponType>
            {
                // Sword projectiles
                [ProjectileID.NightsEdge] = WeaponType.Rapier,
                [ProjectileID.TrueNightsEdge] = WeaponType.Rapier,
                [ProjectileID.Excalibur] = WeaponType.Rapier,
                [ProjectileID.TrueExcalibur] = WeaponType.Rapier,
                [ProjectileID.TerraBlade2] = WeaponType.Rapier,
                [ProjectileID.StarWrath] = WeaponType.Rapier,
                [ProjectileID.Starfury] = WeaponType.Rapier,

                // Bow projectiles (arrows)
                [ProjectileID.WoodenArrowFriendly] = WeaponType.Bow,
                [ProjectileID.FireArrow] = WeaponType.Bow,
                [ProjectileID.UnholyArrow] = WeaponType.Bow,
                [ProjectileID.JestersArrow] = WeaponType.Bow,
                [ProjectileID.HellfireArrow] = WeaponType.Bow,
                [ProjectileID.HolyArrow] = WeaponType.Bow,
                [ProjectileID.FrostArrow] = WeaponType.Bow,

                // Magic projectiles
                [ProjectileID.AmethystBolt] = WeaponType.Staff,
                [ProjectileID.TopazBolt] = WeaponType.Staff,
                [ProjectileID.SapphireBolt] = WeaponType.Staff,
                [ProjectileID.EmeraldBolt] = WeaponType.Staff,
                [ProjectileID.RubyBolt] = WeaponType.Staff,
                [ProjectileID.DiamondBolt] = WeaponType.Staff,
                [ProjectileID.UnholyTridentFriendly] = WeaponType.Staff,

                // Whip projectiles
                [ProjectileID.BlandWhip] = WeaponType.Whip,
                [ProjectileID.SwordWhip] = WeaponType.Whip,
                [ProjectileID.MaceWhip] = WeaponType.Whip,
                [ProjectileID.ScytheWhip] = WeaponType.Whip,
                [ProjectileID.BoneWhip] = WeaponType.Whip,
                [ProjectileID.FireWhip] = WeaponType.Whip,
                [ProjectileID.CoolWhip] = WeaponType.Whip,
                [ProjectileID.ThornWhip] = WeaponType.Whip,
                [ProjectileID.RainbowWhip] = WeaponType.Whip,
                [ProjectileID.MaceWhip] = WeaponType.Whip,
            };
        }

        public static WeaponType GetWeaponType(int itemID)
        {
            return _itemClassifications.TryGetValue(itemID, out WeaponType weaponType) ? weaponType : WeaponType.None;
        }

        public static WeaponType GetProjectileWeaponType(int projectileType)
        {
            return _projectileClassifications.TryGetValue(projectileType, out WeaponType weaponType) ? weaponType : WeaponType.None;
        }

        public static bool IsWeaponType(int itemID, WeaponType type)
        {
            var weaponType = GetWeaponType(itemID);
            return weaponType.HasFlag(type);
        }

        public static bool IsProjectileWeaponType(int projectileType, WeaponType type)
        {
            var weaponType = GetProjectileWeaponType(projectileType);
            return weaponType.HasFlag(type);
        }

        // Get all vanilla items of a specific weapon type
        public static int[] GetVanillaItemsOfType(WeaponType weaponType)
        {
            return _itemClassifications
                .Where(kvp => kvp.Value.HasFlag(weaponType))
                .Select(kvp => kvp.Key)
                .ToArray();
        }

        // Get all vanilla projectiles of a specific weapon type
        public static int[] GetVanillaProjectilesOfType(WeaponType weaponType)
        {
            return _projectileClassifications
                .Where(kvp => kvp.Value.HasFlag(weaponType))
                .Select(kvp => kvp.Key)
                .ToArray();
        }

        // Check if a vanilla item has been classified
        public static bool IsVanillaItemClassified(int itemID)
        {
            return _itemClassifications.ContainsKey(itemID);
        }

        // Check if a vanilla projectile has been classified
        public static bool IsVanillaProjectileClassified(int projectileType)
        {
            return _projectileClassifications.ContainsKey(projectileType);
        }
    }
}