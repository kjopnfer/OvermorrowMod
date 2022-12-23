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
                case NPCID.JungleSlime:
                case NPCID.YellowSlime:
                case NPCID.RedSlime:
                case NPCID.PurpleSlime:
                case NPCID.BlackSlime:
                case NPCID.BabySlime:
                case NPCID.Pinky:
                case NPCID.GreenSlime:
                case NPCID.BlueSlime:
                    ElementWeakness = new List<Element> { Element.Fire };
                    ElementResistance = new List<Element> { Element.Water };
                    break;
                case NPCID.Slimer:
                case NPCID.Slimer2:
                case NPCID.Slimeling:
                    ElementWeakness = new List<Element> { Element.Fire };
                    ElementResistance = new List<Element> { Element.Water, Element.Dark };
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
                case NPCID.ZombieMerman:
                case NPCID.TorchZombie:
                case NPCID.ArmedTorchZombie:
                case NPCID.MaggotZombie:
                    ElementWeakness = new List<Element> { Element.Light, Element.Fire };
                    ElementResistance = new List<Element> { Element.Dark };
                    break;
                case NPCID.TwiggyZombie:
                case NPCID.ZombieMushroom:
                case NPCID.ZombieMushroomHat:
                    ElementResistance = new List<Element> { Element.Dark, Element.Nature };
                    break;
            }
        }
    }
}