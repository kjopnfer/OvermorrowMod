using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Materials
{
	public class HeartStone : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Heart of Stone");
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 16));
		}

		public override void SetDefaults()
		{
			item.width = 52;
			item.height = 80;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.value = 550;
			item.rare = ItemRarityID.Green;
			item.maxStack = 999;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void PostUpdate()
		{
			Lighting.AddLight(item.Center, Color.White.ToVector3() * 0.55f * Main.essScale);
		}
	}
}