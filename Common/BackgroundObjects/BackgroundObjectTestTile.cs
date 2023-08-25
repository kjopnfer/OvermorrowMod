using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Tiles.Town.BackgroundObjects;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Common.BackgroundObjects
{
	public class BackgroundObjectTestTile : ModTile
	{
        public override string Texture => "OvermorrowMod/Common/BackgroundObjects/BackgroundObjectTestTile";
        public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<BackgroundHouse2>().Hook_AfterPlacement, -1, 0, false);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.addTile(Type); 

			 AddMapEntry(new Color(200, 200, 200));
		}
	}

	public class BackgroundObjectTestTile2 : ModTile
	{
		public override string Texture => "OvermorrowMod/Common/BackgroundObjects/BackgroundObjectTestTile";

		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<BackgroundHouse2>().Hook_AfterPlacement, -1, 0, false);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(200, 200, 200));
		}
	}

	public class BackgroundObjectTestTile3 : ModTile
	{
		public override string Texture => "OvermorrowMod/Common/BackgroundObjects/BackgroundObjectTestTile";

		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<BackgroundHouse3>().Hook_AfterPlacement, -1, 0, false);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(200, 200, 200));
		}
	}
	public class BackgroundObjectTestTile4 : ModTile
	{
		public override string Texture => "OvermorrowMod/Common/BackgroundObjects/BackgroundObjectTestTile";

		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<BackgroundHouse4>().Hook_AfterPlacement, -1, 0, false);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(200, 200, 200));
		}
	}

}