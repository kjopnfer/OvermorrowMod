using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Content.Buffs.Hexes
{
    public class SoulFlame : ModHex
    {
        public override void Update()
        {
            Dust.NewDust(npc.position, npc.width, npc.height, DustID.IceTorch, npc.velocity.X, npc.velocity.Y, 0, Color.LightBlue);
        }

        public override void UpdateLifeRegen(ref int damage)
        {
            if (npc.lifeRegen > 0) npc.lifeRegen = 0;
            npc.lifeRegen -= 16;
        }
    }
}