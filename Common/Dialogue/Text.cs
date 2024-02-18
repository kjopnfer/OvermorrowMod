using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Common.Dialogue
{
    public class Text
    {
        public string text { get; }
        public int drawTime { get; }
        public int holdTime { get; }
        public Texture2D texture { get; } = null;

        // This gets used in Dialogue Bubbles, no texture is needed for these
        public Text(string text, int drawTime, int holdTime)
        {
            this.text = text;
            this.drawTime = drawTime;
            this.holdTime = holdTime;
        }

        // This gets used for the DialogueWindows where you don't need to hold the text
        public Text(Texture2D texture, string text, int drawTime)
        {
            this.text = text;
            this.drawTime = drawTime;
            this.holdTime = 1;
        }

        public Text(Texture2D texture, string text, int drawTime, int holdTime)
        {
            this.texture = texture;
            this.text = text;
            this.drawTime = drawTime;
            this.holdTime = holdTime;
        }
    }
}