using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Common.VanillaOverrides.Elements
{
    public enum Element
    {
        None,
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

    public static class ElementalModifiers
    {
        public static float GetModifier(HashSet<Element> incomingElements, HashSet<Element> elementalResistance, HashSet<Element> elementalWeakness)
        {
            float result = 1;

            /*foreach (Element incomingElement in incomingElements)
            {
                if (elementalWeakness.Contains(incomingElement))
                {
                    result += 0.5f;
                }

                if (elementalResistance.Contains(incomingElement))
                {
                    result -= 0.5f;
                }
            }*/

            return result < 0 ? 0 : result;
        }

        public static ElementalNPC Elemental(this NPC player)
        {
            return player.GetGlobalNPC<ElementalNPC>();
        }

        public static ElementalProjectile Elemental(this Projectile projectile)
        {
            return projectile.GetGlobalProjectile<ElementalProjectile>();
        }

        public static ElementalItem Elemental(this Item item)
        {
            return item.GetGlobalItem<ElementalItem>();
        }
    }
}