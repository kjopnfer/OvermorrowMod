using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.VanillaOverrides.Elements
{
    public partial class ElementalItem : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            SetWeaponElements(item);
            SetAccessoryElements(item);
        }

        private void SetWeaponElements(Item item)
        {
            switch (item.type)
            {
                #region Melee
                case ItemID.ZombieArm:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.BloodButcherer:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.PurpleClubberfish:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.IceBlade:
                    ElementTypes = new HashSet<Element>() { Element.Ice };
                    break;
                case ItemID.LightsBane:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.RedPhaseblade:
                    ElementTypes = new HashSet<Element>() { Element.Electric };
                    break;
                case ItemID.OrangePhaseblade:
                    ElementTypes = new HashSet<Element>() { Element.Electric };
                    break;
                case ItemID.YellowPhaseblade:
                    ElementTypes = new HashSet<Element>() { Element.Electric };
                    break;
                case ItemID.GreenPhaseblade:
                    ElementTypes = new HashSet<Element>() { Element.Electric };
                    break;
                case ItemID.BluePhaseblade:
                    ElementTypes = new HashSet<Element>() { Element.Electric };
                    break;
                case ItemID.PurplePhaseblade:
                    ElementTypes = new HashSet<Element>() { Element.Electric };
                    break;
                case ItemID.WhitePhaseblade:
                    ElementTypes = new HashSet<Element>() { Element.Electric };
                    break;
                case ItemID.BeeKeeper:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                case ItemID.BladeofGrass:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                case ItemID.Muramasa:
                    ElementTypes = new HashSet<Element>() { Element.Water };
                    break;
                case ItemID.FieryGreatsword:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
                case ItemID.NightsEdge:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.TheRottedFork:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.Swordfish:
                    ElementTypes = new HashSet<Element>() { Element.Water };
                    break;
                case ItemID.ThunderSpear:
                    ElementTypes = new HashSet<Element>() { Element.Electric };
                    break;
                case ItemID.DarkLance:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.IceBoomerang:
                    ElementTypes = new HashSet<Element>() { Element.Ice };
                    break;
                case ItemID.ThornChakram:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                case ItemID.Flamarang:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
                case ItemID.Shroomerang:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                case ItemID.FlamingMace:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
                case ItemID.BallOHurt:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.TheMeatball:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.BlueMoon:
                    ElementTypes = new HashSet<Element>() { Element.Water };
                    break;
                case ItemID.Sunfury:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
                case ItemID.CorruptYoyo:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.CrimsonYoyo:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.JungleYoyo:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                #endregion
                #region Ranged
                case ItemID.DemonBow:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.TendonBow:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.MoltenFury:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
                case ItemID.BeesKnees:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                case ItemID.HellwingBow:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
                case ItemID.StarCannon:
                    ElementTypes = new HashSet<Element>() { Element.Light };
                    break;
                case ItemID.PhoenixBlaster:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
                case ItemID.MolotovCocktail:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
                case ItemID.FrostDaggerfish:
                    ElementTypes = new HashSet<Element>() { Element.Ice };
                    break;
                case ItemID.Beenade:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                case ItemID.BoneArrow:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.FlamingArrow:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
                case ItemID.FrostburnArrow:
                    ElementTypes = new HashSet<Element>() { Element.Ice };
                    break;
                case ItemID.JestersArrow:
                    ElementTypes = new HashSet<Element>() { Element.Light };
                    break;
                case ItemID.UnholyArrow:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.HellfireArrow:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
                #endregion
                #region Magic
                case ItemID.WandofSparking:
                    ElementTypes = new HashSet<Element>() { Element.Fire, Element.Nature };
                    break;
                case ItemID.Vilethorn:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.CrimsonRod:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.WeatherPain:
                    ElementTypes = new HashSet<Element>() { Element.Wind };
                    break;
                case ItemID.AquaScepter:
                    ElementTypes = new HashSet<Element>() { Element.Water };
                    break;
                case ItemID.Flamelash:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
                case ItemID.BeeGun:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                case ItemID.WaterBolt:
                    ElementTypes = new HashSet<Element>() { Element.Water };
                    break;
                case ItemID.DemonScythe:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.BookofSkulls:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.FlowerofFire:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
                case ItemID.ThunderStaff:
                    ElementTypes = new HashSet<Element>() { Element.Electric };
                    break;
                #endregion
                #region Summon
                case ItemID.HornetStaff:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                case ItemID.ImpStaff:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
                case ItemID.DD2FlameburstTowerT1Popper:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
                case ItemID.DD2LightningAuraT1Popper:
                    ElementTypes = new HashSet<Element>() { Element.Electric };
                    break;
                case ItemID.FlinxStaff:
                    ElementTypes = new HashSet<Element>() { Element.Ice };
                    break;
                case ItemID.ThornWhip:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                case ItemID.BoneWhip:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                    #endregion
            }
        }

        private void SetAccessoryElements(Item item)
        {
            switch (item.type)
            {
                #region Movement
                case ItemID.Aglet:
                    ElementTypes = new HashSet<Element>() { Element.Wind };
                    break;
                case ItemID.AmphibianBoots:
                    ElementTypes = new HashSet<Element>() { Element.Water };
                    break;
                case ItemID.BalloonHorseshoeHoney:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                case ItemID.AnkletoftheWind:
                    ElementTypes = new HashSet<Element>() { Element.Wind, Element.Nature };
                    break;
                case ItemID.ArcticDivingGear:
                    ElementTypes = new HashSet<Element>() { Element.Ice };
                    break;
                case ItemID.BalloonPufferfish:
                    ElementTypes = new HashSet<Element>() { Element.Wind, Element.Water };
                    break;
                case ItemID.BlizzardinaBalloon:
                    ElementTypes = new HashSet<Element>() { Element.Ice };
                    break;
                case ItemID.BlizzardinaBottle:
                    ElementTypes = new HashSet<Element>() { Element.Ice };
                    break;
                case ItemID.BundleofBalloons:
                    ElementTypes = new HashSet<Element>() { Element.Ice, Element.Wind };
                    break;
                case ItemID.CelestialShell:
                    ElementTypes = new HashSet<Element>() { Element.Light, Element.Dark };
                    break;
                case ItemID.CloudinaBalloon:
                    ElementTypes = new HashSet<Element>() { Element.Wind };
                    break;
                case ItemID.CloudinaBottle:
                    ElementTypes = new HashSet<Element>() { Element.Wind };
                    break;
                case ItemID.SandBoots:
                    ElementTypes = new HashSet<Element>() { Element.Earth };
                    break;
                case ItemID.FairyBoots:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                case ItemID.FartInABalloon:
                    ElementTypes = new HashSet<Element>() { Element.Wind };
                    break;
                case ItemID.FartinaJar:
                    ElementTypes = new HashSet<Element>() { Element.Wind };
                    break;
                case ItemID.FlurryBoots:
                    ElementTypes = new HashSet<Element>() { Element.Ice };
                    break;
                case ItemID.FrogFlipper:
                    ElementTypes = new HashSet<Element>() { Element.Water };
                    break;
                case ItemID.FrogGear:
                    ElementTypes = new HashSet<Element>() { Element.Water };
                    break;
                case ItemID.FrogLeg:
                    ElementTypes = new HashSet<Element>() { Element.Water };
                    break;
                case ItemID.FrogWebbing:
                    ElementTypes = new HashSet<Element>() { Element.Water };
                    break;
                case ItemID.FrostsparkBoots:
                    ElementTypes = new HashSet<Element>() { Element.Ice, Element.Electric };
                    break;
                case ItemID.HellfireTreads:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
                case ItemID.HoneyBalloon:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                case ItemID.LavaCharm:
                    ElementTypes = new HashSet<Element>() { Element.Fire, Element.Earth };
                    break;
                case ItemID.LavaWaders:
                    ElementTypes = new HashSet<Element>() { Element.Fire, Element.Earth };
                    break;
                case ItemID.LightningBoots:
                    ElementTypes = new HashSet<Element>() { Element.Electric };
                    break;
                case ItemID.MoonCharm:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.MoonShell:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.MoltenCharm:
                    ElementTypes = new HashSet<Element>() { Element.Fire, Element.Earth };
                    break;
                case ItemID.NeptunesShell:
                    ElementTypes = new HashSet<Element>() { Element.Water };
                    break;
                case ItemID.ObsidianHorseshoe:
                    ElementTypes = new HashSet<Element>() { Element.Earth };
                    break;
                case ItemID.ObsidianWaterWalkingBoots:
                    ElementTypes = new HashSet<Element>() { Element.Earth, Element.Water };
                    break;
                case ItemID.SailfishBoots:
                    ElementTypes = new HashSet<Element>() { Element.Water };
                    break;
                case ItemID.SandstorminaBalloon:
                    ElementTypes = new HashSet<Element>() { Element.Wind };
                    break;
                case ItemID.SandstorminaBottle:
                    ElementTypes = new HashSet<Element>() { Element.Wind };
                    break;
                case ItemID.SharkronBalloon:
                    ElementTypes = new HashSet<Element>() { Element.Water };
                    break;
                case ItemID.TerrasparkBoots:
                    ElementTypes = new HashSet<Element>() { Element.Ice, Element.Electric, Element.Earth, Element.Fire };
                    break;
                case ItemID.TsunamiInABottle:
                    ElementTypes = new HashSet<Element>() { Element.Water };
                    break;
                case ItemID.WaterWalkingBoots:
                    ElementTypes = new HashSet<Element>() { Element.Water };
                    break;
                #endregion
                #region Wings
                case ItemID.AngelWings:
                    ElementTypes = new HashSet<Element>() { Element.Light };
                    break;
                case ItemID.DemonWings:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.LeafWings:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                case ItemID.FrozenWings:
                    ElementTypes = new HashSet<Element>() { Element.Ice };
                    break;
                case ItemID.BatWings:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.FlameWings:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
                case ItemID.GhostWings:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.BoneWings:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.SpookyWings:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.FishronWings:
                    ElementTypes = new HashSet<Element>() { Element.Water };
                    break;
                case ItemID.WingsSolar:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
                #endregion
                #region Health and Mana
                case ItemID.ArcaneFlower:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                case ItemID.MagnetFlower:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                case ItemID.ManaFlower:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                case ItemID.NaturesGift:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                #endregion
                #region Combat
                case ItemID.BeeCloak:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                case ItemID.Bezoar:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                case ItemID.CelestialStone:
                    ElementTypes = new HashSet<Element>() { Element.Light, Element.Dark };
                    break;
                case ItemID.EyeoftheGolem:
                    ElementTypes = new HashSet<Element>() { Element.Light };
                    break;
                case ItemID.FireGauntlet:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
                case ItemID.FrozenTurtleShell:
                    ElementTypes = new HashSet<Element>() { Element.Ice };
                    break;
                case ItemID.FrozenShield:
                    ElementTypes = new HashSet<Element>() { Element.Ice };
                    break;
                case ItemID.HoneyComb:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                case ItemID.LavaSkull:
                    ElementTypes = new HashSet<Element>() { Element.Fire, Element.Earth };
                    break;
                case ItemID.MagmaStone:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
                case ItemID.MoonStone:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.MoltenQuiver:
                    ElementTypes = new HashSet<Element>() { Element.Fire, Element.Earth };
                    break;
                case ItemID.MoltenSkullRose:
                    ElementTypes = new HashSet<Element>() { Element.Fire, Element.Earth };
                    break;
                case ItemID.ObsidianRose:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
                case ItemID.ObsidianShield:
                    ElementTypes = new HashSet<Element>() { Element.Fire, Element.Earth };
                    break;
                case ItemID.ObsidianSkull:
                    ElementTypes = new HashSet<Element>() { Element.Fire, Element.Earth };
                    break;
                case ItemID.PutridScent:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.StalkersQuiver:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.StingerNecklace:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                case ItemID.SunStone:
                    ElementTypes = new HashSet<Element>() { Element.Light };
                    break;
                case ItemID.SweetheartNecklace:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                case ItemID.NecromanticScroll:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ItemID.PapyrusScarab:
                    ElementTypes = new HashSet<Element>() { Element.Dark, Element.Nature };
                    break;
                case ItemID.PygmyNecklace:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                #endregion
                #region Miscellaneous
                case ItemID.FlowerBoots:
                    ElementTypes = new HashSet<Element> { Element.Nature };
                    break;
                case ItemID.JellyfishNecklace:
                    ElementTypes = new HashSet<Element> { Element.Water };
                    break;
                #endregion
                #region Expert
                case ItemID.WormScarf:
                    ElementTypes = new HashSet<Element> { Element.Dark };
                    break;
                case ItemID.BrainOfConfusion:
                    ElementTypes = new HashSet<Element> { Element.Dark };
                    break;
                case ItemID.HiveBackpack:
                    ElementTypes = new HashSet<Element> { Element.Nature };
                    break;
                case ItemID.BoneGlove:
                    ElementTypes = new HashSet<Element> { Element.Dark };
                    break;
                case ItemID.BoneHelm:
                    ElementTypes = new HashSet<Element> { Element.Dark };
                    break;
                case ItemID.SporeSac:
                    ElementTypes = new HashSet<Element> { Element.Nature };
                    break;
                case ItemID.ShinyStone:
                    ElementTypes = new HashSet<Element> { Element.Light };
                    break;
                case ItemID.EmpressFlightBooster:
                    ElementTypes = new HashSet<Element> { Element.Light };
                    break;
                    #endregion
            }
        }
    }
}