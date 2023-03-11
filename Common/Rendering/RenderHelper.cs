using Terraria;

namespace OvermorrowMod.Common.Rendering
{
	public static class RenderHelper
	{
		public static void SetRenderer(this NPC npc, Renderer renderer)
		{
			npc.GetGlobalNPC<RenderNPC>().Renderer = renderer;
		}

		public static void SetRenderer<T>(this NPC npc) where T : Renderer, new()
		{
			npc.GetGlobalNPC<RenderNPC>().Renderer = RenderingSystem.GetRenderer<T>();
		}

		public static void SetRenderer<T>(this Projectile projectile) where T : Renderer, new()
		{
			projectile.GetGlobalProjectile<RenderProj>().Renderer = RenderingSystem.GetRenderer<T>();
		}

		public static void SetRenderer(this Projectile projectile, Renderer renderer)
		{
			projectile.GetGlobalProjectile<RenderProj>().Renderer = renderer;
		}
	}
}