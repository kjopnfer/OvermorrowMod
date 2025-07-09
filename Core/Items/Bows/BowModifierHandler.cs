using OvermorrowMod.Common.Weapons.Bows;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Interfaces;
using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Core.Items.Bows
{
    public class BowModifierHandler
    {
        public static BowStats GetModifiedStats(BowStats baseStats, Player player)
        {
            BowStats modifiedStats = baseStats.Clone();

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
            var bowPlayer = player.GetModPlayer<BowPlayer>();
            foreach (var modifier in bowPlayer.ActiveModifiers)
            {
                modifier.OnPowerShot(bow, player);
            }
        }

        public static void TriggerArrowFired(HeldBow bow, Player player, Projectile arrow)
        {
            var bowPlayer = player.GetModPlayer<BowPlayer>();
            foreach (var modifier in bowPlayer.ActiveModifiers)
            {
                modifier.OnArrowFired(bow, player, arrow);
            }
        }
    }
}