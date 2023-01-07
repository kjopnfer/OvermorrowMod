using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.VanillaOverrides.Elements
{
    public partial class ElementalNPC : GlobalNPC
    {
        public override void SetDefaults(NPC npc)
        {
            switch (npc.type)
            {
                case NPCID.YellowSlime:
                case NPCID.RedSlime:
                case NPCID.PurpleSlime:
                case NPCID.BlackSlime:
                case NPCID.BabySlime:
                case NPCID.Pinky:
                case NPCID.GreenSlime:
                case NPCID.BlueSlime:
                case NPCID.SlimeRibbonWhite:
                case NPCID.SlimeRibbonYellow:
                case NPCID.SlimeRibbonGreen:
                case NPCID.SlimeRibbonRed:
                case NPCID.KingSlime:
                    ElementWeakness = new HashSet<Element> { Element.Fire };
                    ElementResistance = new HashSet<Element> { Element.Water };
                    break;
                case NPCID.IceSlime:
                case NPCID.SpikedIceSlime:
                case NPCID.IceBat:
                case NPCID.IceTortoise:
                case NPCID.IceElemental:
                case NPCID.IceGolem:
                case NPCID.IcyMerman:
                    ElementWeakness = new HashSet<Element> { Element.Fire };
                    ElementResistance = new HashSet<Element> { Element.Water, Element.Ice };
                    break;
                case NPCID.Slimer:
                case NPCID.Slimer2:
                case NPCID.Slimeling:
                case NPCID.DungeonSlime:
                case NPCID.Crimslime:
                    ElementWeakness = new HashSet<Element> { Element.Fire };
                    ElementResistance = new HashSet<Element> { Element.Water, Element.Dark };
                    break;
                case NPCID.ToxicSludge:
                case NPCID.SwampThing:
                case NPCID.JungleSlime:
                case NPCID.SpikedJungleSlime:
                    ElementWeakness = new HashSet<Element> { Element.Fire };
                    ElementResistance = new HashSet<Element> { Element.Water, Element.Nature };
                    break;
                case NPCID.FireImp:
                case NPCID.LavaSlime:
                case NPCID.Lavabat:
                    ElementWeakness = new HashSet<Element> { Element.Water };
                    ElementResistance = new HashSet<Element> { Element.Fire };
                    break;
                case NPCID.BigRainZombie:
                case NPCID.SmallRainZombie:
                case NPCID.BigFemaleZombie:
                case NPCID.SmallFemaleZombie:
                case NPCID.BigTwiggyZombie:
                case NPCID.SmallTwiggyZombie:
                case NPCID.BigSwampZombie:
                case NPCID.SmallSwampZombie:
                case NPCID.BigSlimedZombie:
                case NPCID.SmallSlimedZombie:
                case NPCID.BigPincushionZombie:
                case NPCID.SmallPincushionZombie:
                case NPCID.BigBaldZombie:
                case NPCID.SmallBaldZombie:
                case NPCID.BigZombie:
                case NPCID.SmallZombie:
                case NPCID.Zombie:
                case NPCID.BaldZombie:
                case NPCID.ZombieEskimo:
                case NPCID.PincushionZombie:
                case NPCID.SlimedZombie:
                case NPCID.SwampZombie:
                case NPCID.FemaleZombie:
                case NPCID.ZombieRaincoat:
                case NPCID.ArmedZombie:
                case NPCID.ArmedZombieEskimo:
                case NPCID.ArmedZombiePincussion:
                case NPCID.ArmedZombieSlimed:
                case NPCID.ArmedZombieSwamp:
                case NPCID.ArmedZombieTwiggy:
                case NPCID.ArmedZombieCenx:
                case NPCID.BloodZombie:
                case NPCID.TorchZombie:
                case NPCID.ArmedTorchZombie:
                case NPCID.MaggotZombie:
                case NPCID.DoctorBones:
                case NPCID.TheGroom:
                case NPCID.Frankenstein:
                case NPCID.Eyezor:
                case NPCID.Drippler:
                case NPCID.TheBride:
                    ElementWeakness = new HashSet<Element> { Element.Light, Element.Fire };
                    ElementResistance = new HashSet<Element> { Element.Dark };
                    break;
                case NPCID.TwiggyZombie:
                case NPCID.ZombieMushroom:
                case NPCID.ZombieMushroomHat:
                    ElementResistance = new HashSet<Element> { Element.Dark, Element.Nature };
                    break;
                case NPCID.MeteorHead:
                    ElementWeakness = new HashSet<Element> { Element.Water };
                    ElementResistance = new HashSet<Element> { Element.Fire, Element.Earth };
                    break;
                case NPCID.AngryBones:
                case NPCID.DarkCaster:
                case NPCID.CursedSkull:
                case NPCID.SkeletronHead:
                case NPCID.SkeletronHand:
                case NPCID.UndeadMiner:
                case NPCID.Tim:
                case NPCID.ArmoredSkeleton:
                case NPCID.Mummy:
                case NPCID.DarkMummy:
                case NPCID.Wraith:
                case NPCID.CursedHammer:
                case NPCID.Corruptor:
                case NPCID.SeekerHead:
                case NPCID.SeekerBody:
                case NPCID.SeekerTail:
                case NPCID.SkeletonArcher:
                case NPCID.PossessedArmor:
                case NPCID.Vampire:
                case NPCID.RuneWizard:
                case NPCID.CrimsonAxe:
                case NPCID.FaceMonster:
                case NPCID.FloatyGross:
                case NPCID.Reaper:
                case NPCID.RaggedCaster:
                case NPCID.Necromancer:
                case NPCID.NecromancerArmored:
                case NPCID.BoneLee:
                case NPCID.GiantCursedSkull:
                case NPCID.SkeletonSniper:
                case NPCID.TacticalSkeleton:
                case NPCID.SkeletonCommando:
                case NPCID.AngryBonesBig:
                case NPCID.AngryBonesBigHelmet:
                case NPCID.AngryBonesBigMuscle:
                case NPCID.PirateGhost:
                    ElementWeakness = new HashSet<Element> { Element.Light };
                    ElementResistance = new HashSet<Element> { Element.Dark };
                    break;
                case NPCID.UndeadViking:
                    ElementWeakness = new HashSet<Element> { Element.Light };
                    ElementResistance = new HashSet<Element> { Element.Dark, Element.Ice };
                    break;
                case NPCID.BlueArmoredBones:
                case NPCID.BlueArmoredBonesMace:
                case NPCID.BlueArmoredBonesNoPants:
                case NPCID.BlueArmoredBonesSword:
                    ElementWeakness = new HashSet<Element> { Element.Light };
                    ElementResistance = new HashSet<Element> { Element.Dark, Element.Water };
                    break;
                case NPCID.RustyArmoredBonesAxe:
                case NPCID.RustyArmoredBonesFlail:
                case NPCID.RustyArmoredBonesSword:
                case NPCID.RustyArmoredBonesSwordNoArmor:
                    ElementWeakness = new HashSet<Element> { Element.Light };
                    ElementResistance = new HashSet<Element> { Element.Dark, Element.Earth };
                    break;
                case NPCID.HellArmoredBones:
                case NPCID.HellArmoredBonesMace:
                case NPCID.HellArmoredBonesSpikeShield:
                case NPCID.HellArmoredBonesSword:
                case NPCID.DiabolistRed:
                case NPCID.DiabolistWhite:
                    ElementWeakness = new HashSet<Element> { Element.Light };
                    ElementResistance = new HashSet<Element> { Element.Dark, Element.Fire };
                    break;
                case NPCID.ZombieMerman:
                    ElementWeakness = new HashSet<Element> { Element.Light };
                    ElementResistance = new HashSet<Element> { Element.Dark, Element.Water };
                    break;
                case NPCID.LightMummy:
                case NPCID.EnchantedSword:
                case NPCID.IlluminantBat:
                case NPCID.IlluminantSlime:
                case NPCID.HallowBoss:
                case NPCID.QueenSlimeBoss:
                case NPCID.QueenSlimeMinionBlue:
                case NPCID.QueenSlimeMinionPink:
                case NPCID.QueenSlimeMinionPurple:
                    ElementWeakness = new HashSet<Element> { Element.Dark };
                    ElementResistance = new HashSet<Element> { Element.Light };
                    break;
                case NPCID.Paladin:
                    ElementResistance = new HashSet<Element> { Element.Light };
                    break;
                case NPCID.BoneSerpentHead:
                case NPCID.BoneSerpentBody:
                case NPCID.BoneSerpentTail:
                    ElementWeakness = new HashSet<Element> { Element.Light };
                    ElementResistance = new HashSet<Element> { Element.Dark, Element.Fire };
                    break;
                case NPCID.Demon:
                case NPCID.VoodooDemon:
                case NPCID.RedDevil:
                    ElementResistance = new HashSet<Element> { Element.Dark, Element.Fire };
                    break;
                case NPCID.Clinger:
                    ElementWeakness = new HashSet<Element> { Element.Light };
                    ElementResistance = new HashSet<Element> { Element.Dark, Element.Fire };
                    break;
                case NPCID.BigHornetStingy:
                case NPCID.LittleHornetStingy:
                case NPCID.BigHornetSpikey:
                case NPCID.LittleHornetSpikey:
                case NPCID.BigHornetLeafy:
                case NPCID.LittleHornetLeafy:
                case NPCID.LittleHornetHoney:
                case NPCID.BigHornetHoney:
                case NPCID.LittleHornetFatty:
                case NPCID.BigHornetFatty:
                case NPCID.Hornet:
                case NPCID.BlackRecluse:
                case NPCID.BlackRecluseWall:
                case NPCID.WallCreeper:
                case NPCID.WallCreeperWall:
                case NPCID.MossHornet:
                case NPCID.Bee:
                case NPCID.QueenBee:
                case NPCID.HornetFatty:
                case NPCID.HornetHoney:
                case NPCID.HornetLeafy:
                case NPCID.HornetSpikey:
                case NPCID.HornetStingy:
                case NPCID.JungleCreeper:
                case NPCID.JungleCreeperWall:
                    ElementWeakness = new HashSet<Element> { Element.Fire };
                    break;
                case NPCID.ManEater:
                case NPCID.Snatcher:
                case NPCID.AngryTrapper:
                case NPCID.FungoFish:
                case NPCID.MushiLadybug:
                case NPCID.FungiBulb:
                case NPCID.GiantFungiBulb:
                case NPCID.Plantera:
                case NPCID.PlanterasTentacle:
                    ElementWeakness = new HashSet<Element> { Element.Fire };
                    ElementResistance = new HashSet<Element> { Element.Nature, Element.Water };
                    break;
                case NPCID.Harpy:
                case NPCID.WyvernHead:
                case NPCID.WyvernBody:
                case NPCID.WyvernBody2:
                case NPCID.WyvernBody3:
                case NPCID.WyvernLegs:
                case NPCID.WyvernTail:
                    ElementWeakness = new HashSet<Element> { Element.Electric };
                    break;
                case NPCID.Golem:
                case NPCID.GolemHead:
                case NPCID.GolemFistLeft:
                case NPCID.GolemFistRight:
                case NPCID.GraniteGolem:
                case NPCID.GraniteFlyer:
                case NPCID.DesertBeast:
                case NPCID.SandShark:
                case NPCID.SandSlime:
                case NPCID.RockGolem:
                    ElementResistance = new HashSet<Element> { Element.Earth };
                    break;
                case NPCID.SandsharkCorrupt:
                case NPCID.SandsharkCrimson:
                    ElementResistance = new HashSet<Element> { Element.Earth, Element.Dark };
                    break;
                case NPCID.SandsharkHallow:
                    ElementResistance = new HashSet<Element> { Element.Earth, Element.Light };
                    break;
                case NPCID.SandElemental:
                case NPCID.Tumbleweed:
                    ElementResistance = new HashSet<Element> { Element.Earth, Element.Wind };
                    break;
                case NPCID.AngryNimbus:
                    ElementWeakness = new HashSet<Element> { Element.Nature };
                    ElementResistance = new HashSet<Element> { Element.Wind, Element.Water, Element.Electric };
                    break;
                case NPCID.BloodNautilus:
                case NPCID.BloodSquid:
                case NPCID.GoblinShark:
                case NPCID.BloodEelHead:
                case NPCID.BloodEelBody:
                case NPCID.BloodEelTail:
                    ElementResistance = new HashSet<Element> { Element.Dark, Element.Water };
                    break;
                case NPCID.DukeFishron:
                    ElementWeakness = new HashSet<Element> { Element.Electric };
                    ElementResistance = new HashSet<Element> { Element.Water };
                    break;
            }
        }
    }
}