using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class WaxSconce : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;
        public override bool CanExplode(int i, int j) => false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;

            TileObjectData.newTile.AnchorWall = true;

            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, TileObjectData.newTile.Height).ToArray();

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, 1);

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.addTile(Type);
            AddMapEntry(new Color(215, 186, 87));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.9f;
            g = 0.675f;
            b = 0f;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX == 0 && tile.TileFrameY == 0 && !Main.gamePaused)
            {
                float scale = 0.1f;
                Vector2 velocity = -Vector2.UnitY * 0.3f;

                WaxCandleholder.CreateEmberParticle(new Vector2(i + 0.95f, j + 0.4f) * 16, velocity, scale);
                WaxCandleholder.CreateEmberParticle(new Vector2(i + 2.05f, j + 0.4f) * 16, velocity, scale);
            }
        }
    }
}