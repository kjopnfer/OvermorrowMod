using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Common.Tooltips
{
    public enum BuffTooltipType
    {
        Buff,
        Debuff
    }

    public class BuffTooltip : TooltipEntity
    {
        public readonly float BuffTime;
        public readonly BuffTooltipType Type;

        public BuffTooltip(Texture2D buffIcon, string buffTitle, string buffDescription, float buffTime, BuffTooltipType type)
        {
            Priority = 3;
            ObjectIcon = buffIcon;
            Title = buffTitle;
            Description = buffDescription;
            BuffTime = buffTime;
            Type = type;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, float containerWidth, Vector2 titleSize, Color primaryColor)
        {
        }
    }
}