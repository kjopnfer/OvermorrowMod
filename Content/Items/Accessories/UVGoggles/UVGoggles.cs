using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Players;
using OvermorrowMod.Core;
using System.Diagnostics;

namespace OvermorrowMod.Content.Items.Accessories.UVGoggles
{
	[AutoloadEquip(new EquipType[] { EquipType.Face })]
	public class UVGoggles : ModItem
	{
		public static bool visible = false;
		public override void SetStaticDefaults()
		{
			ArmorIDs.Head.Sets.DrawFullHair[Item.faceSlot] = true;
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 24;
			Item.accessory = true;
			Item.rare = ItemRarityID.Orange;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			visible = !hideVisual;
			player.GetModPlayer<OvermorrowModPlayer>().UVBubbles.Add(new OvermorrowModPlayer.UVBubble(player.MountedCenter, 360f));
			/*player.GetModPlayer<UVGogglesPplayer>().UVReveal(player.Center, 360);
			//player.GetModPlayer<UVGogglesPplayer>().UVEffect = true; //old and bad code do not use bad idea just changes textures n shit
			player.GetModPlayer<UVGogglesPplayer>().Hidden = hideVisual;*/
		}
	}
	public class GoggleGlowmask : PlayerDrawLayer
    {
		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
			Player player = drawInfo.drawPlayer;
			if (!UVGoggles.visible) return false;
			for (int i = 0; player.armor.Length > i; i++)
				if (player.armor[i].type == ModContent.ItemType<UVGoggles>())
				{
					if (player.armor[10].netID == ItemID.FamiliarWig || (player.armor[10].netID == 0 && (player.armor[0].netID == 0 || player.armor[0].netID == ItemID.FamiliarWig)))
						return true;
					else 
						return false;
				}
			return false;
        }
		public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.FaceAcc);
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
			Texture2D Glowmask = (Texture2D)ModContent.Request<Texture2D>("OvermorrowMod/Content/Items/Accessories/UVGoggles/UVGoggles_Face_Glowmask");
			if (drawInfo.drawPlayer.name.ToLower().Contains("frankfires"))
				Glowmask = (Texture2D)ModContent.Request<Texture2D>("OvermorrowMod/Content/Items/Accessories/UVGoggles/UVGoggles_Face_Glowmask_Dev");

			//Vector2 Position = drawInfo.Center + (Vector2.UnitY * drawInfo.mountOffSet / 2f) - Main.screenPosition - Vector2.UnitY * 9f;
			Vector2 Position = drawInfo.Position + new Vector2(drawInfo.drawPlayer.width / 2, drawInfo.drawPlayer.width - 8f) - Main.screenPosition;
			if (drawInfo.drawPlayer.mount.Active)
			{
				Position.Y += drawInfo.drawPlayer.mount.HeightBoost;

				Main.NewText(drawInfo.drawPlayer.mount.HeightBoost);
			}

            Position = Position.ToPoint().ToVector2();

			float alpha = (255 - drawInfo.drawPlayer.immuneAlpha) / 255f;
			Color color = Color.White;
			Rectangle frame = drawInfo.drawPlayer.bodyFrame;
			Vector2 origin = drawInfo.headVect;
			float rotation = drawInfo.drawPlayer.headRotation;

			drawInfo.DrawDataCache.Add(new DrawData(Glowmask, Position, frame, color * alpha, rotation, origin, 1f, drawInfo.drawPlayer.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0));
		}
    }
}
