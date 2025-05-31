using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Globals
{
    public enum WeaponID
    {
        None = 0,
        Shortbow = 1,
        Longbow = 2,
        Swarmbow = 3,
        Stormbow = 4,
        Dagger = 5,
        Spear = 6,
        Revolver = 7
    }

    public static class WeaponType
    {
        public static bool[] IsDagger = ItemID.Sets.Factory.CreateBoolSet();
        public static bool[] IsSpear = ItemID.Sets.Factory.CreateBoolSet();

        public static bool[] IsShortbow = ItemID.Sets.Factory.CreateBoolSet(ItemID.WoodenBow, ItemID.BorealWoodBow, ItemID.RichMahoganyBow, ItemID.PalmWoodBow, ItemID.EbonwoodBow, ItemID.ShadewoodBow, ItemID.CopperBow, ItemID.TinBow, ItemID.IronBow, ItemID.LeadBow, ItemID.SilverBow, ItemID.TungstenBow, ItemID.GoldBow, ItemID.PlatinumBow, ItemID.DemonBow, ItemID.TendonBow, ItemID.MoltenFury);
        public static bool[] IsLongbow = ItemID.Sets.Factory.CreateBoolSet();
        public static bool[] IsSwarmbow = ItemID.Sets.Factory.CreateBoolSet(ItemID.BeesKnees, ItemID.HellwingBow);
        public static bool[] IsStormbow = ItemID.Sets.Factory.CreateBoolSet(ItemID.DaedalusStormbow, ItemID.BloodRainBow);

        public static bool[] IsRevolver = ItemID.Sets.Factory.CreateBoolSet(ItemID.Revolver, ItemID.TheUndertaker);

        /// <summary>
        /// This shit does NOT work
        /// </summary>
        public static bool HasWeaponType(this Item item)
        {
            var ids = Enum.GetValues(typeof(WeaponID));
            foreach (var id in ids)
            {
                return id switch
                {
                    1 => IsShortbow[item.type],
                    2 => IsLongbow[item.type],
                    3 => IsSwarmbow[item.type],
                    4 => IsStormbow[item.type],
                    5 => IsDagger[item.type],
                    6 => IsSpear[item.type],
                    7 => IsRevolver[item.type],
                    _ => false
                };
            }

            return false;
        }

        public static bool IsGun(this Item item)
        {
            return item.GetWeaponTypeID() switch
            {
                WeaponID.Revolver => true,
                _ => false
            };
        }

        public static bool IsBow(this Item item)
        {
            return item.GetWeaponTypeID() switch
            {
                WeaponID.Shortbow or WeaponID.Longbow or WeaponID.Swarmbow or WeaponID.Stormbow => true,
                _ => false
            };
        }

        public static bool IsWeaponType(this Item item, int weaponID)
        {
            return weaponID switch
            {
                1 => IsShortbow[item.type],
                2 => IsLongbow[item.type],
                3 => IsSwarmbow[item.type],
                4 => IsStormbow[item.type],
                5 => IsDagger[item.type],
                6 => IsSpear[item.type],
                7 => IsRevolver[item.type],
                _ => false
            };
        }

        public static bool IsWeaponType(this Item item, WeaponID weaponID)
        {
            return item.IsWeaponType((int)weaponID);
        }

        public static string GetWeaponType(this Item item)
        {
            var lookup = Enum.GetValues(typeof(WeaponID))
                .Cast<WeaponID>()
                .ToDictionary(weaponID => (int)weaponID, weaponID => weaponID.ToString());

            foreach (KeyValuePair<int, string> entry in lookup)
            {
                if (item.IsWeaponType(entry.Key)) return entry.Value;
            }

            return "None";
        }

        public static WeaponID GetWeaponTypeID(this Item item)
        {
            var lookup = Enum.GetValues(typeof(WeaponID))
                .Cast<WeaponID>()
                .ToDictionary(weaponID => (int)weaponID, weaponID => weaponID);

            foreach (KeyValuePair<int, WeaponID> entry in lookup)
            {
                if (item.IsWeaponType(entry.Key)) return entry.Value;
            }

            return WeaponID.None;
        }
    }

    public partial class OvermorrowGlobalItem : GlobalItem
    {
    }
}