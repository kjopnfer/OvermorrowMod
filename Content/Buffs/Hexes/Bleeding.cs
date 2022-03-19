using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Content.Buffs.Hexes
{
    public class Bleeding : ModHex
    {
        public override void Update()
        {
            int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.Blood, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 0, default(Color), 1f);
            Main.dust[dust].velocity *= 1.8f;
            Main.dust[dust].velocity.Y -= 0.5f;
        }
        public override void UpdateLifeRegen(ref int damage)
        {
            if (npc.lifeRegen > 0) { npc.lifeRegen = 0; }
            npc.lifeRegen -= 8;
        }
    }
}