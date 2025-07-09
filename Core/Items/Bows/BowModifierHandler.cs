using OvermorrowMod.Common.Weapons.Bows;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Interfaces;
using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Core.Items.Bows
{
    public class BowModifierHandler
    {
        private static List<IBowModifier> globalModifiers = new List<IBowModifier>();

        public static void RegisterGlobalModifier(IBowModifier modifier)
        {
            globalModifiers.Add(modifier);
        }

        public static BowStats GetModifiedStats(BowStats baseStats, Player player)
        {
            BowStats modifiedStats = baseStats.Clone();

            // Apply global modifiers
            foreach (var modifier in globalModifiers)
            {
                modifier.ModifyBowStats(modifiedStats, player);
            }

            // Apply accessory modifiers
            var bowPlayer = player.GetModPlayer<BowPlayer>();
            foreach (var modifier in bowPlayer.ActiveModifiers)
            {
                modifier.ModifyBowStats(modifiedStats, player);
            }

            return modifiedStats;
        }

        public static void TriggerPowerShot(HeldBow bow, Player player)
        {
            foreach (var modifier in globalModifiers)
            {
                modifier.OnPowerShot(bow, player);
            }

            var bowPlayer = player.GetModPlayer<BowPlayer>();
            foreach (var modifier in bowPlayer.ActiveModifiers)
            {
                modifier.OnPowerShot(bow, player);
            }
        }

        public static void TriggerArrowFired(HeldBow bow, Player player, Projectile arrow)
        {
            foreach (var modifier in globalModifiers)
            {
                modifier.OnArrowFired(bow, player, arrow);
            }

            var bowPlayer = player.GetModPlayer<BowPlayer>();
            foreach (var modifier in bowPlayer.ActiveModifiers)
            {
                modifier.OnArrowFired(bow, player, arrow);
            }
        }
    }
}