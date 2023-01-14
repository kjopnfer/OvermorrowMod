using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.VanillaOverrides.Elements
{
    public class ElementalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public HashSet<Element> ElementTypes = new HashSet<Element>() { Element.None };

        public override void SetDefaults(Projectile projectile)
        {
            switch (projectile.type)
            {
                case ProjectileID.FireArrow:
                case ProjectileID.WandOfSparkingSpark:
                case ProjectileID.BallofFire:
                case ProjectileID.Flamarang:
                case ProjectileID.Flamelash:
                case ProjectileID.Sunfury:
                case ProjectileID.HellfireArrow:
                case ProjectileID.Flames:
                case ProjectileID.HeatRay:
                case ProjectileID.InfernoFriendlyBolt:
                case ProjectileID.InfernoFriendlyBlast:
                case ProjectileID.ImpFireball:
                case ProjectileID.Hellwing:
                case ProjectileID.Spark:
                    ElementTypes = new HashSet<Element>() { Element.Fire };
                    break;
                case ProjectileID.UnholyArrow:
                case ProjectileID.VilethornBase:
                case ProjectileID.VilethornTip:
                case ProjectileID.BallOHurt:
                case ProjectileID.DemonScythe:
                case ProjectileID.DarkLance:
                case ProjectileID.UnholyTridentFriendly:
                case ProjectileID.TheRottedFork:
                case ProjectileID.TheMeatball:
                case ProjectileID.NightBeam:
                case ProjectileID.BloodRain:
                case ProjectileID.DeathSickle:
                case ProjectileID.ShadowBeamFriendly:
                case ProjectileID.LostSoulFriendly:
                case ProjectileID.VampireKnife:
                case ProjectileID.EatersBite:
                case ProjectileID.TinyEater:
                case ProjectileID.Raven:
                case ProjectileID.FlamingJack:
                case ProjectileID.BoneArrowFromMerchant:
                    ElementTypes = new HashSet<Element>() { Element.Dark };
                    break;
                case ProjectileID.CursedBullet:
                case ProjectileID.CursedFlameFriendly:
                case ProjectileID.ClingerStaff:
                case ProjectileID.CursedDartFlame:
                case ProjectileID.CursedDart:
                case ProjectileID.ShadowFlameArrow:
                case ProjectileID.ShadowFlameKnife:
                case ProjectileID.ShadowFlame:
                    ElementTypes = new HashSet<Element>() { Element.Dark, Element.Fire };
                    break;
                case ProjectileID.JestersArrow:
                case ProjectileID.Starfury:
                case ProjectileID.RainbowRodBullet:
                case ProjectileID.CrystalStorm:
                case ProjectileID.LightBeam:
                case ProjectileID.RainbowFront:
                case ProjectileID.RainbowBack:
                case ProjectileID.PaladinsHammerFriendly:
                case ProjectileID.CrystalBullet:
                case ProjectileID.CrystalDart:
                case ProjectileID.CrystalVileShardHead:
                case ProjectileID.CrystalVileShardShaft:
                case ProjectileID.CrystalPulse:
                case ProjectileID.CrystalPulse2:
                case ProjectileID.HallowStar:
                case ProjectileID.HolyArrow:
                case ProjectileID.Gungnir:
                    ElementTypes = new HashSet<Element>() { Element.Light };
                    break;
                case ProjectileID.WaterStream:
                case ProjectileID.BlueMoon:
                case ProjectileID.RainFriendly:
                case ProjectileID.Swordfish:
                case ProjectileID.ObsidianSwordfish:
                case ProjectileID.Anchor:
                case ProjectileID.Flairon:
                case ProjectileID.FlaironBubble:
                case ProjectileID.MiniSharkron:
                case ProjectileID.Typhoon:
                case ProjectileID.Bubble:
                    ElementTypes = new HashSet<Element>() { Element.Water };
                    break;
                case ProjectileID.ThornChakram:
                case ProjectileID.Seed:
                case ProjectileID.PoisonedKnife:
                case ProjectileID.MushroomSpear:
                case ProjectileID.Mushroom:
                case ProjectileID.TerraBeam:
                case ProjectileID.NettleBurstEnd:
                case ProjectileID.NettleBurstLeft:
                case ProjectileID.NettleBurstRight:
                case ProjectileID.Bee:
                case ProjectileID.Wasp:
                case ProjectileID.Leaf:
                case ProjectileID.ChlorophyteBullet:
                case ProjectileID.ChlorophytePartisan:
                case ProjectileID.ChlorophyteDrill:
                case ProjectileID.ChlorophyteArrow:
                case ProjectileID.CrystalLeafShot:
                case ProjectileID.SporeCloud:
                case ProjectileID.ChlorophyteOrb:
                case ProjectileID.FlowerPow:
                case ProjectileID.FlowerPowPetal:
                case ProjectileID.PoisonFang:
                case ProjectileID.PineNeedleFriendly:
                case ProjectileID.HornetStinger:
                case ProjectileID.VenomSpider:
                case ProjectileID.DangerousSpider:
                case ProjectileID.JumperSpider:
                case ProjectileID.SeedlerNut:
                case ProjectileID.SeedlerThorn:
                case ProjectileID.ToxicCloud:
                case ProjectileID.ToxicCloud2:
                case ProjectileID.ToxicCloud3:
                case ProjectileID.ToxicBubble:
                    ElementTypes = new HashSet<Element>() { Element.Nature };
                    break;
                case ProjectileID.IceBoomerang:
                case ProjectileID.IceBolt:
                case ProjectileID.FrostBoltSword:
                case ProjectileID.FrostArrow:
                case ProjectileID.IceSickle:
                case ProjectileID.FrostHydra:
                case ProjectileID.FrostBlastFriendly:
                case ProjectileID.Blizzard:
                case ProjectileID.NorthPoleWeapon:
                case ProjectileID.NorthPoleSpear:
                case ProjectileID.NorthPoleSnowflake:
                case ProjectileID.FrostBoltStaff:
                case ProjectileID.FrostDaggerfish:
                    ElementTypes = new HashSet<Element>() { Element.Ice };
                    break;
                case ProjectileID.AmethystBolt:
                case ProjectileID.TopazBolt:
                case ProjectileID.SapphireBolt:
                case ProjectileID.EmeraldBolt:
                case ProjectileID.RubyBolt:
                case ProjectileID.DiamondBolt:
                    ElementTypes = new HashSet<Element>() { Element.Earth };
                    break;
                case ProjectileID.Meteor1:
                case ProjectileID.Meteor2:
                case ProjectileID.Meteor3:
                    ElementTypes = new HashSet<Element>() { Element.Earth, Element.Fire };
                    break;
                case ProjectileID.GreenLaser:
                case ProjectileID.PurpleLaser:
                case ProjectileID.MagnetSphereBolt:
                case ProjectileID.LaserMachinegunLaser:
                case ProjectileID.ElectrosphereMissile:
                case ProjectileID.Electrosphere:
                case ProjectileID.InfluxWaver:
                case ProjectileID.ChargedBlasterLaser:
                case ProjectileID.ChargedBlasterOrb:
                    ElementTypes = new HashSet<Element>() { Element.Electric };
                    break;
            }
        }
    }
}