using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Content.Buffs.Hexes
{
    public class CursedFlames : ModHex
    {
        public override void Update()
        {
            if (Main.rand.NextBool(5))
            {
                Vector2 position = npc.Center + new Vector2(Main.rand.Next(-npc.width / 2, npc.width / 2), Main.rand.Next(-npc.height / 2, npc.height / 2));
                Dust dust = Terraria.Dust.NewDustPerfect(position, 75, new Vector2(0f, -5.526316f), 0, new Color(255, 255, 255), 1f);
            }

        }
        public override void UpdateLifeRegen(ref int damage)
        {
            if (npc.lifeRegen > 0) npc.lifeRegen = 0;
            npc.lifeRegen -= 30;
            if (damage < 50) damage = 50;
        }
    }
}