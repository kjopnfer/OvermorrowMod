using System.Collections.Generic;

namespace OvermorrowMod.Common.VanillaOverrides.Elements
{
    public enum Element
    {
        Fire,
        Ice,
        Water,
        Earth,
        Nature,
        Wind,
        Electric,
        Light,
        Dark
    }

    public class ElementalModifiers
    {
        private static Dictionary<Element, HashSet<Element>> ElementalResistance = new Dictionary<Element, HashSet<Element>>()
        {
            { Element.Fire, new HashSet<Element>() { Element.Nature, Element.Wind } },
            { Element.Ice, new HashSet<Element>() { Element.Water, Element.Nature } },
            { Element.Water, new HashSet<Element>() { Element.Nature, Element.Fire } },
            { Element.Earth, new HashSet<Element> { Element.Nature, Element.Water } },
            { Element.Nature, new HashSet<Element> { Element.Water, Element.Light} },
            { Element.Wind, new HashSet<Element>() { Element.Earth, Element.Wind } },
            { Element.Electric, new HashSet<Element> { Element.Water, Element.Nature} },
        };

        private static Dictionary<Element, HashSet<Element>> ElementalWeakness = new Dictionary<Element, HashSet<Element>>()
        {
            { Element.Fire, new HashSet<Element>() { Element.Water, Element.Earth } },
            { Element.Ice, new HashSet<Element>() { Element.Fire, Element.Light} },
            { Element.Water, new HashSet<Element>() { Element.Electric, Element.Ice } },
            { Element.Earth, new HashSet<Element> { Element.Nature, Element.Water } },
            { Element.Nature, new HashSet<Element> { Element.Fire, Element.Ice} },
            { Element.Wind, new HashSet<Element>() { Element.Electric, Element.Fire } },
            { Element.Electric, new HashSet<Element> { Element.Earth } },
            { Element.Light, new HashSet<Element> { Element.Dark, Element.Nature } },
            { Element.Dark, new HashSet<Element>() { Element.Light } },
        };

        private static bool CheckWeakness(Element incomingElement, Element outgoingElement)
        {
            var weakness = ElementalWeakness[outgoingElement];
            return weakness.Contains(incomingElement);
        }

        private static bool CheckResistance(Element incomingElement, Element outgoingElement)
        {
            var resistance = ElementalResistance[outgoingElement];
            return resistance.Contains(incomingElement);
        }

        public static float CheckInteraction(Element incomingElement, Element outgoingElement)
        {
            if (CheckWeakness(incomingElement, outgoingElement)) return 2;

            if (CheckResistance(incomingElement, outgoingElement)) return 0.5f;

            return 1;
        }
    }
}