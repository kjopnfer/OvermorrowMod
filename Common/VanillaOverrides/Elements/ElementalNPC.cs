using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.VanillaOverrides.Elements
{
    public partial class ElementalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public HashSet<Element> ElementResistance = new HashSet<Element>() { Element.None };
        public HashSet<Element> ElementWeakness = new HashSet<Element>() { Element.None };

        private CombatText GetRecentCombatText()
        {
            for (int i = 99; i >= 0; i--)
            {
                CombatText ctToCheck = Main.combatText[i];
                if (ctToCheck.lifeTime == 60 || ctToCheck.lifeTime == 120)
                {
                    if (ctToCheck.alpha == 1f)
                    {
                        if ((ctToCheck.color == CombatText.DamagedHostile || ctToCheck.color == CombatText.DamagedHostileCrit))
                        {
                            return ctToCheck;
                        }
                    }
                }
            }

            return null;
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            CombatText text = GetRecentCombatText();
            if (text == null) return;

            float modifier = ElementalModifiers.GetModifier(item.Elemental().ElementTypes, ElementResistance, ElementWeakness);

            if (modifier > 1) text.color = Color.LightCyan;
            if (modifier < 1) text.color = Color.DarkRed;
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            float modifier = ElementalModifiers.GetModifier(item.Elemental().ElementTypes, ElementResistance, ElementWeakness);

            CombatText text = GetRecentCombatText();
            if (modifier > 1) text.color = Color.Green;
            if (modifier < 1) text.color = Color.Red;

            // Commented out because we're not using elemental modifiers at the moment
            //damage = (int)Math.Round(damage * modifier);
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            CombatText text = GetRecentCombatText();
            if (text == null) return;

            float modifier = ElementalModifiers.GetModifier(projectile.Elemental().ElementTypes, ElementResistance, ElementWeakness);

            if (modifier > 1) text.color = Color.LightCyan;
            if (modifier < 1) text.color = Color.DarkRed;
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            float modifier = ElementalModifiers.GetModifier(projectile.Elemental().ElementTypes, ElementResistance, ElementWeakness);
            //damage = (int)Math.Round(damage * modifier);
        }
    }
}