using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Items.Bows;
using OvermorrowMod.Core.Items.Bows;
using Terraria;

namespace OvermorrowMod.Core.Interfaces
{
    /// <summary>
    /// Used to apply global effects via accessories or other sources.
    /// For example, creating another projectile for each shot.
    /// </summary>
    public interface IBowModifier
    {
        void ModifyBowStats(BowStats stats, Player player);
        void OnPowerShot(HeldBow bow, Player player);
        void OnArrowFired(HeldBow bow, Player player, Projectile arrow);
    }

    /// <summary>
    /// Used to apply global draw effects via accessories or other sources.
    /// For example, making all bows emit runes or glow a certain color.
    /// </summary>
    public interface IBowDrawEffects
    {
        void DrawChargingEffects(HeldBow bow, Player player, SpriteBatch spriteBatch, float chargeProgress);
        void DrawArrowEffects(HeldBow bow, Player player, SpriteBatch spriteBatch, Vector2 arrowPosition, float chargeProgress);
        void DrawBowEffects(HeldBow bow, Player player, SpriteBatch spriteBatch, Vector2 bowPosition, float chargeProgress);
    }
}