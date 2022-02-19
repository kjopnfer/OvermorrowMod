using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Buffs.Hexes
{
    public class LesserIchor : ModHex
    {
        public override void Update()
        {
			if (Main.rand.NextFloat() < 0.09210526f)
			{
				Vector2 position = npc.Center + new Vector2(Main.rand.Next(-npc.width / 2, npc.width / 2), Main.rand.Next(-npc.height / 2, npc.height / 2));
				Dust dust = Main.dust[Terraria.Dust.NewDust(position, 30, 30, DustID.Ichor, 0f, 10f, 0, new Color(255, 255, 255), 1f)];
				dust.noGravity = true;
			}
		}
    }
}