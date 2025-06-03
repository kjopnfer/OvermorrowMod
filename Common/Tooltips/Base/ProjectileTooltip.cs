using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Tooltips
{
    public enum ProjectileTooltipType
    {
        Projectile,
        Minion,
        Trap
    }

    public class ProjectileTooltip : TooltipEntity
    {
        public readonly float ProjectileDamage;
        public readonly ProjectileTooltipType Type;
        public readonly DamageClass DamageClass;

        public ProjectileTooltip(Texture2D projectileIcon, string projectileTitle, string[] projectileDescription, float projectileDamage, ProjectileTooltipType type, DamageClass damageClass)
        {
            Priority = 2;
            ObjectIcon = projectileIcon;
            Title = projectileTitle;
            Description = projectileDescription;
            ProjectileDamage = projectileDamage;
            Type = type;
            DamageClass = damageClass;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, float containerWidth, Vector2 titleSize, Color primaryColor)
        {
        }
    }
}