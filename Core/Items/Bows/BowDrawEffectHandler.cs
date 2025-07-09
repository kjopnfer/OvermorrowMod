using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Weapons.Bows;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Interfaces;
using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Core.Items.Bows
{
    public static class BowDrawEffectsHandler
    {
        public static void DrawAllChargingEffects(HeldBow bow, Player player, SpriteBatch spriteBatch, float chargeProgress)
        {
            // Draw accessory effects
            var bowPlayer = player.GetModPlayer<BowPlayer>();
            foreach (var effect in bowPlayer.ActiveDrawEffects)
            {
                effect.DrawChargingEffects(bow, player, spriteBatch, chargeProgress);
            }
        }

        public static void DrawAllArrowEffects(HeldBow bow, Player player, SpriteBatch spriteBatch, Vector2 arrowPosition, float chargeProgress)
        {
            var bowPlayer = player.GetModPlayer<BowPlayer>();
            foreach (var effect in bowPlayer.ActiveDrawEffects)
            {
                effect.DrawArrowEffects(bow, player, spriteBatch, arrowPosition, chargeProgress);
            }
        }

        public static void DrawAllBowEffects(HeldBow bow, Player player, SpriteBatch spriteBatch, Vector2 bowPosition, float chargeProgress)
        {
            var bowPlayer = player.GetModPlayer<BowPlayer>();
            foreach (var effect in bowPlayer.ActiveDrawEffects)
            {
                effect.DrawBowEffects(bow, player, spriteBatch, bowPosition, chargeProgress);
            }
        }
    }
}