using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Items;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class WizardStatue : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;

        public override bool CanExplode(int i, int j) => false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.Width = 10;
            TileObjectData.newTile.Height = 10;
            TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, TileObjectData.newTile.Height).ToArray();

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, TileObjectData.newTile.Height - 1);

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(24, 21, 18), name);
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            base.NearbyEffects(i, j, closer);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            return base.PreDraw(i, j, spriteBatch);
        }
    }

    public class NormalWizardStatue : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;

        public override bool CanExplode(int i, int j) => false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;

            TileObjectData.newTile.Width = 8;
            TileObjectData.newTile.Height = 10;
            TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, TileObjectData.newTile.Height).ToArray();

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, TileObjectData.newTile.Height - 1);

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(24, 21, 18), name);
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
                Vector2 velocity = -Vector2.UnitY * 0.4f;

                WaxCandleholder.CreateEmberParticle(new Vector2(i + 1.2f, j + 7.2f) * 16, velocity, scale);
                WaxCandleholder.CreateEmberParticle(new Vector2(i + 0.93f, j + 8f) * 16, velocity, scale);
                WaxCandleholder.CreateEmberParticle(new Vector2(i + 1.5f, j + 8.1f) * 16, velocity, scale);
                WaxCandleholder.CreateEmberParticle(new Vector2(i + 1.2f, j + 9.3f) * 16, velocity, scale);

                WaxCandleholder.CreateEmberParticle(new Vector2(i + 2.45f, j + 8.3f) * 16, velocity, scale);
                WaxCandleholder.CreateEmberParticle(new Vector2(i + 5.4f, j + 8.3f) * 16, velocity, scale);
                WaxCandleholder.CreateEmberParticle(new Vector2(i + 5.9f, j + 8.1f) * 16, velocity, scale);
                WaxCandleholder.CreateEmberParticle(new Vector2(i + 6.6f, j + 8.1f) * 16, velocity, scale);
                WaxCandleholder.CreateEmberParticle(new Vector2(i + 6.9f, j + 7.5f) * 16, velocity, scale * 0.8f);
                WaxCandleholder.CreateEmberParticle(new Vector2(i + 7.1f, j + 8.2f) * 16, velocity, scale * 0.7f);
                WaxCandleholder.CreateEmberParticle(new Vector2(i + 7f, j + 9.2f) * 16, velocity, scale);
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            return base.PreDraw(i, j, spriteBatch);
        }
    }
}