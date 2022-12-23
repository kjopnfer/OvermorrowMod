using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.VanillaOverrides.Elements {
    public partial class ElementalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public List<Element> ElementResistance = new List<Element>() { Element.None };
        public List<Element> ElementWeakness = new List<Element>() { Element.None };

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            float newDamage = damage * ElementalModifiers.CheckInteraction(item.Elemental().ElementTypes, ElementResistance, ElementWeakness);
            damage = (int)Math.Round(newDamage);
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            float newDamage = damage * ElementalModifiers.CheckInteraction(projectile.Elemental().ElementTypes, ElementResistance, ElementWeakness);
            damage = (int)Math.Round(newDamage);
        }
    }
}