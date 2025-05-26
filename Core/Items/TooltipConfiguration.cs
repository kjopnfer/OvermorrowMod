using Microsoft.Xna.Framework;

namespace OvermorrowMod.Core.Items
{
    /// <summary>
    /// Configuration constants for the tooltip system
    /// </summary>
    public static class TooltipConfiguration
    {
        public const float CONTAINER_WIDTH = 350f;
        public const float MAXIMUM_TEXT_LENGTH = 330f;
        public const int BOTTOM_OFFSET = 20;
        public const int DIVIDER_OFFSET = 48;
        public const int KEYWORD_WIDTH = 200;
        public const int RIGHT_OFFSET = 75;
        public const int BOTTOM_PADDING = 14;
        public const int CONTAINER_OFFSET = 35;

        public static readonly Color PrimaryBackgroundColor = new Color(28, 31, 77);
        public static readonly Color TitleColor = new Color(139, 233, 253);
        public static readonly Color SubtitleColor = new Color(241, 250, 140);
        public static readonly Color BuffColor = new Color(80, 250, 123);
        public static readonly Color DebuffColor = new Color(255, 85, 85);
        public static readonly Color KeywordColor = new Color(255, 121, 198);
    }
}