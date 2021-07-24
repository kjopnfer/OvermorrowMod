using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Buffs.Hexes
{
    public class SoulFlame : ModHex
    {
        public override void Draw(SpriteBatch spriteBatch, ref Color drawColor)
        {
            drawColor = Color.LightBlue;
        }
        public override void Update()
        {
            npc.position -= npc.velocity * 0.75f;
            Dust.NewDust(npc.position, npc.width, npc.height, DustID.Fire, npc.velocity.X, npc.velocity.Y, 0, Color.LightBlue);
        }
        public override void UpdateLifeRegen(ref int damage)
        {
            if (npc.lifeRegen > 0) npc.lifeRegen = 0;
            npc.lifeRegen -= 30;
            if (damage < 50) damage = 50;
        }
    }
}