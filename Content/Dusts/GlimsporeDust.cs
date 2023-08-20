using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Dusts
{
	public class GlimsporeDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			Player player = Main.player[(int)(dust.fadeIn + 0.5f)];
			dust.noGravity = true;
			dust.noLight = false;
			dust.color = Color.White;
		}

		public override bool Update(Dust dust)
		{
			Player player = Main.player[(int)(dust.fadeIn + 0.5f)];
			Vector2 playerPos = player.Center;
			dust.velocity += new Vector2(player.Center.X - dust.position.X, player.Center.Y - dust.position.Y);
			dust.rotation += (dust.velocity.X + dust.velocity.Y) / 2;
			dust.alpha += 4;
			if (dust.alpha == 256)
				dust.active = false;

			return false;
		}
	}
}