using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Particles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class FloorCandles : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;
        public override bool CanExplode(int i, int j) => false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;

            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = [16, 16];

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, 1);

            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 6;

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.addTile(Type);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.9f;
            g = 0.675f;
            b = 0f;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.gamePaused) return;

            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameY != 0) return;

            float scale = 0.075f;
            Vector2 velocity = -Vector2.UnitY * 0.4f;

            switch (tile.TileFrameX)
            {
                case 0:
                    WaxCandleholder.CreateEmberParticle(new Vector2(i + 0.3f, j + 1.5f) * 16, velocity, scale);
                    WaxCandleholder.CreateEmberParticle(new Vector2(i + 0.65f, j + 0.8f) * 16, velocity, scale);
                    WaxCandleholder.CreateEmberParticle(new Vector2(i + 0.8f, j + 1.7f) * 16, velocity, scale);
                    break;
                case 18:
                    WaxCandleholder.CreateEmberParticle(new Vector2(i + 0.3f, j + 1.6f) * 16, velocity, scale);
                    WaxCandleholder.CreateEmberParticle(new Vector2(i + 0.6f, j + 1.4f) * 16, velocity, scale);
                    break;
                case 36:
                    WaxCandleholder.CreateEmberParticle(new Vector2(i + 0.2f, j + 1.5f) * 16, velocity, scale);
                    WaxCandleholder.CreateEmberParticle(new Vector2(i + 0.65f, j + 1.6f) * 16, velocity, scale);
                    break;
                case 54:
                    WaxCandleholder.CreateEmberParticle(new Vector2(i + 0.35f, j + 1.3f) * 16, velocity, scale);
                    break;
                case 72:
                    WaxCandleholder.CreateEmberParticle(new Vector2(i + 0.3f, j + 1.4f) * 16, velocity, scale);
                    WaxCandleholder.CreateEmberParticle(new Vector2(i + 0.8f, j + 1.3f) * 16, velocity, scale);
                    break;
                case 90:
                    WaxCandleholder.CreateEmberParticle(new Vector2(i + 0.15f, j + 1.6f) * 16, velocity, scale);
                    WaxCandleholder.CreateEmberParticle(new Vector2(i + 0.45f, j + 1.4f) * 16, velocity, scale);
                    WaxCandleholder.CreateEmberParticle(new Vector2(i + 0.75f, j + 1.6f) * 16, velocity, scale);
                    break;
            }


            base.NearbyEffects(i, j, closer);
        }
    }
}