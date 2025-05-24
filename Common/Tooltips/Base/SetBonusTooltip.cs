using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace OvermorrowMod.Common.Tooltips
{
    public class SetBonusTooltip : TooltipEntity
    {
        public readonly string SetName;
        public readonly List<int> SetItems;

        public SetBonusTooltip(Texture2D setIcon, string setTitle, string setName, string setDescription, List<int> setItems)
        {
            Priority = 1;
            ObjectIcon = setIcon;
            Title = setTitle;
            Description = setDescription;
            SetName = setName;
            SetItems = setItems;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, float containerWidth, Vector2 titleSize, Color primaryColor)
        {
        }
    }
}