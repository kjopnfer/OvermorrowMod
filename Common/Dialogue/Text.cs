namespace OvermorrowMod.Common.Dialogue
{
    public class Text
    {
        public string text { get; }
        public int drawTime { get; }
        public int holdTime { get; }

        public Text(string text, int drawTime, int holdTime)
        {
            this.text = text;
            this.drawTime = drawTime;
            this.holdTime = holdTime;
        }
    }
}