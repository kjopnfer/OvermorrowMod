using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.BackgroundObjects;
using OvermorrowMod.Core;
using Terraria;

namespace OvermorrowMod.Content.Tiles.Town.BackgroundObjects
{
    public class BackgroundHouse : BaseBackgroundObject
    {
        public override string Texture => "OvermorrowMod/Content/Tiles/Town/BackgroundObjects/BackgroundHouse";
        public override Vector2 DrawOffset => new Vector2(0, -68);
        public override bool BackgroundObjectTileCheck(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile;
        }

        public override (float, float) Size() => new(542, 378);
    }

    public class BackgroundHouse2 : BaseBackgroundObject
    {
        public override string Texture => "OvermorrowMod/Content/Tiles/Town/BackgroundObjects/BackgroundHouse2"; 
        public override Vector2 DrawOffset => new Vector2(64, -48);

        public override bool BackgroundObjectTileCheck(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile;
        }

        public override (float, float) Size() => new(682, 506);
    }

    public class BackgroundHouse3 : BaseBackgroundObject
    {
        public override string Texture => "OvermorrowMod/Content/Tiles/Town/BackgroundObjects/BackgroundHouse3"; 
        public override Vector2 DrawOffset => new Vector2(0, -48);

        public override bool BackgroundObjectTileCheck(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile;
        }

        public override (float, float) Size() => new(722, 510);
    }

    public class BackgroundHouse4 : BaseBackgroundObject
    {
        public override string Texture => "OvermorrowMod/Content/Tiles/Town/BackgroundObjects/BackgroundHouse4"; 
        public override Vector2 DrawOffset => new Vector2(0, -32);

        public override bool BackgroundObjectTileCheck(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile;
        }

        public override (float, float) Size() => new(546, 464);
    }
}