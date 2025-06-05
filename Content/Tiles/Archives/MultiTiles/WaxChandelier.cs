using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Particles;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class WaxChandelier : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;
        public override bool CanExplode(int i, int j) => false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;

            TileObjectData.newTile.Width = 8;
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, TileObjectData.newTile.Height).ToArray();

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(3, 0);

            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 3);

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
                Vector2 velocity = -Vector2.UnitY * 0.5f;

                WaxCandleholder.CreateEmberParticle(new Vector2(i + 1.15f, j + 2.45f) * 16, velocity, scale);
                WaxCandleholder.CreateEmberParticle(new Vector2(i + 2.35f, j + 2.25f) * 16, velocity, scale);
                WaxCandleholder.CreateEmberParticle(new Vector2(i + 3.0f, j + 2.5f) * 16, velocity, scale);
                WaxCandleholder.CreateEmberParticle(new Vector2(i + 3.45f, j + 2.25f) * 16, velocity, scale);

                WaxCandleholder.CreateEmberParticle(new Vector2(i + 4.55f, j + 2.25f) * 16, velocity, scale);
                WaxCandleholder.CreateEmberParticle(new Vector2(i + 4.9f, j + 2.45f) * 16, velocity, scale);
                WaxCandleholder.CreateEmberParticle(new Vector2(i + 5.7f, j + 2.2f) * 16, velocity, scale);
                WaxCandleholder.CreateEmberParticle(new Vector2(i + 6.8f, j + 2.5f) * 16, velocity, scale);
            }
        }
    }
}