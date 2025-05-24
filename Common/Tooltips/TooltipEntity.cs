using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Tooltips
{
    /// <summary>
    /// Base class for all tooltip entities.
    /// </summary>
    public abstract class TooltipEntity
    {
        public int Priority { get; protected set; }
        public Texture2D ObjectIcon { get; protected set; }
        public string Title { get; protected set; }
        public string Description { get; protected set; }

        public abstract void Draw(SpriteBatch spriteBatch, Vector2 position, float containerWidth, Vector2 titleSize, Color primaryColor);

        // Factory methods for common tooltip types
        public static ProjectileTooltip CreateProjectileTooltip(string title, string description, float damage,
            ProjectileTooltipType type = ProjectileTooltipType.Projectile, Texture2D icon = null)
        {
            icon ??= ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "Default").Value;
            return new ProjectileTooltip(icon, title, description, damage, type);
        }

        public static BuffTooltip CreateBuffTooltip(string title, string description, float duration,
            BuffTooltipType type = BuffTooltipType.Buff, Texture2D icon = null)
        {
            icon ??= ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "Default").Value;
            return new BuffTooltip(icon, title, description, duration, type);
        }

        public static SetBonusTooltip CreateSetBonusTooltip(string title, string setName, string description,
            List<int> setItems, Texture2D icon = null)
        {
            icon ??= ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "Default").Value;
            return new SetBonusTooltip(icon, title, setName, description, setItems);
        }
    }
}