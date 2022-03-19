using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Content.Buffs.Hexes
{
    public class TimeSlow : ModHex
    {
        public override void Draw(SpriteBatch spriteBatch, ref Color drawColor)
        {
            drawColor = Color.Gray;
        }

        public override void Update()
        {
            npc.position -= npc.velocity * 0.75f;
        }
    }
}