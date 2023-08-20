using System;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ObjectData;
using OvermorrowMod.Content.Tiles.UVBiome.UVSoil;
using Terraria.DataStructures;
using Terraria.Enums;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using OvermorrowMod.Common.Players;
using Steamworks;
using OvermorrowMod.Content.Dusts;

namespace OvermorrowMod.Content.Tiles.UVBiome.GlimsporeVines
{
	public class GlimsporeItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Glimspore Bulbs");
			//Tooltip.SetDefault("visible while the ultraviolet goggles are equipped");
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}
		public override void SetDefaults()
		{
			Item.material = true;
			Item.width = 24;
			Item.height = 24;
			Item.maxStack = 999;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<GlimsporeVine>();
		}
	}
	public class GlimsporeVine : ModTile
	{
		private const int FrameWidth = 16;
        private const int FrameHeight = 16;
		private const int Padding = 2;
        public override void SetStaticDefaults()
        {
			DustType = ModContent.DustType<GlimsporeDust>();
			HitSound = SoundID.Grass;
			Main.tileCut[Type] = true;
			Main.tileFrameImportant[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 0, 1);
			TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
			TileObjectData.newTile.WaterDeath = true;
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Glimspore Vines");
			AddMapEntry(new Color(42, 90, 166), name);
		}
		void DoItemDrop(int i, int j)
		{
			int ItemType = ModContent.ItemType<GlimsporeItem>();
			int Quantity = 1;
			for (int q = 0; 4 > q; q++) 
			{
				if (Main.rand.Next(Quantity + 1) == 0)
					Quantity++; 
			}
			if (Main.rand.Next(512) == 0)
			{
				//ItemType = ModContent.ItemType<Items.StrangeSpore>(); port me
				Quantity = 1;
			}
			Item.NewItem(null, new Vector2(i, j).ToWorldCoordinates(), ItemType, Quantity);
		}
		public override void MouseOver(int i, int j)
		{
			if (Framing.GetTileSafely(i, j).TileFrameX == FrameWidth + Padding && Framing.GetTileSafely(i, j).TileFrameY < (FrameHeight + Padding) * 4)
			{
				Player player = Main.LocalPlayer;
				player.cursorItemIconEnabled = true;
				player.cursorItemIconID = ModContent.ItemType<GlimsporeItem>();
			}
		}
		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			if (Framing.GetTileSafely(i, j).TileFrameY < (FrameHeight + Padding) * 4)
			{
				r = 0.06f;
				g = 0.13f;
				b = 0.24f;
			}
			else
			{
				r = 0;
				g = 0;
				b = 0;
			}
		}
		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			Tile tileBelow = Framing.GetTileSafely(i, j + 1);
			if (tileBelow.TileType == ModContent.TileType<GlimsporeVine>() && tileBelow.HasTile)
				WorldGen.KillTile(i, j +1);
			if (tile.TileFrameX == FrameWidth + Padding)
				DoItemDrop(i, j);
			DustType = (tile.TileFrameY < (FrameHeight + Padding) * 4) ? ModContent.DustType<GlimsporeDust>() : -1; //maybe find a nicer dust instead of none
		}
		public static void UpdateFrame(int i,int j, bool GrowChance)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			Tile tileBelow = Framing.GetTileSafely(i, j + 1);
			if (Main.rand.Next(0, 2) == 0 && GrowChance && tile.TileFrameX != FrameWidth + Padding)
				tile.TileFrameX = FrameWidth + Padding;
			tile.TileFrameY = (FrameHeight + Padding) * 3;
			if (tileBelow.TileType == ModContent.TileType<GlimsporeVine>())
				tile.TileFrameY = (short)((FrameHeight + Padding) * ((i + j) % 3));
			if (tile.TileFrameX > FrameWidth + Padding) //for some reason frameX gets set to 255 when leaving/entering worlds, this fixes that
				tile.TileFrameX = FrameWidth + Padding;
			if (Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendTileSquare(-1, i, j, 1);
		}
        public override bool RightClick(int i, int j)
        {
			if (Framing.GetTileSafely(i, j).TileFrameX == FrameWidth + Padding && Framing.GetTileSafely(i, j).TileFrameY < (FrameHeight + Padding) * 4)
			{
				SoundEngine.PlaySound(SoundID.Grass);
				Framing.GetTileSafely(i, j).TileFrameX = 0;
				DoItemDrop(i, j);
				return true;
			}
			return false;
        }
        public override void PlaceInWorld(int i, int j, Item item)
		{
			UpdateFrame(i, j, false);
		}
		public override void RandomUpdate(int i, int j)
		{		
			var BelowTile = Framing.GetTileSafely(i, j + 1);
			if (!BelowTile.HasTile && BelowTile.TileType != ModContent.TileType<GlimsporeVine>())
			{
				BelowTile.HasTile = true;
				BelowTile.TileType = (ushort)ModContent.TileType<GlimsporeVine>();
				UpdateFrame(i, j, true);
				UpdateFrame(i, j + 1, false);
			}
		}
		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}
			var texture = ModContent.Request<Texture2D>("OvermorrowMod/Content/Tiles/UVBiome/GlimsporeVines/GlimsporeGlow").Value;
			if (tile.TileFrameX == FrameWidth + Padding && tile.TileFrameY < (FrameHeight + Padding) * 4)
				Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X - (FrameWidth - 16f) / 2f, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, FrameHeight, FrameWidth), Color.White, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
		}
		public override bool CanPlace(int i, int j)
		{
			var fard = Main.tile[Main.MouseWorld.ToTileCoordinates().X, Main.MouseWorld.ToTileCoordinates().Y - 1].TileType;
			return fard == ModContent.TileType<UVSoil.UVSoil>() || fard == ModContent.TileType<UVSoilOff>();
		}
		public override void NearbyEffects(int i, int j, bool closer)
		{
			OvermorrowModPlayer OMMP = Main.LocalPlayer.GetModPlayer<OvermorrowModPlayer>();
			Tile tile = Framing.GetTileSafely(i, j);
			int offCheckCount = 0;
			for (int h = 0; h < OMMP.UVBubbles.Count; h++)
			{
				float vectorDisX = (OMMP.UVBubbles[h].Position.X - (i * 16));
				float vectorDisY = (OMMP.UVBubbles[h].Position.Y - (j * 16));
				float center = (float)Math.Pow(vectorDisX, 2) + (float)Math.Pow(vectorDisY, 2);
				if (/*(!uVGogglesPplayer.UVEffect) &&*/ !((center <= Math.Pow(OMMP.UVBubbles[h].Radius, 2))))
					offCheckCount++;
			}
			if (offCheckCount == OMMP.UVBubbles.Count)
			{
				UpdateFrame(i, j, false);
				tile.TileFrameY += (FrameHeight + Padding) * 4;
				WorldGen.SquareTileFrame(i, j);
				if (Main.netMode == NetmodeID.Server)
					NetMessage.SendTileSquare(-1, i, j, 1);
				return;
			}
			else
			{
				if (Main.rand.Next(0, 30) == 0)
					Dust.NewDustPerfect(new Vector2(i, j).ToWorldCoordinates(), ModContent.DustType<GlimsporeDust>(), null, 0, default, 1f);
				UpdateFrame(i, j, false);
				WorldGen.SquareTileFrame(i, j);
				if (Main.netMode == NetmodeID.Server)
					NetMessage.SendTileSquare(-1, i, j, 1);
				return;
			}
		}
	}
}