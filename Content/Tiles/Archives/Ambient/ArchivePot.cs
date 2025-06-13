using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class ArchivePot : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;
        public override void SetStaticDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.addTile(Type);

            TileID.Sets.DisableSmartCursor[Type] = true;

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(215, 186, 87), name);
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 2;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            SoundEngine.PlaySound(SoundID.Item27);
            for (int k = 0; k < 8; k++)
            {
                Dust.NewDust(new Vector2(i * 16, j * 16 - 10), 54, 16, DustID.Dirt, 0.0f, -1, 0, new Color(), 0.5f);
                Dust.NewDust(new Vector2(i * 16, j * 16 - 10), 75, 16, DustID.Dirt, 0.0f, 0, 0, new Color(), 0.5f);
            }
        }
    }

    public class ArchivePotSmall : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;
        public override void SetStaticDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
            TileObjectData.addTile(Type);

            TileID.Sets.DisableSmartCursor[Type] = true;

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(215, 186, 87), name);
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 2;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            SoundEngine.PlaySound(SoundID.Item27);
            for (int k = 0; k < 8; k++)
            {
                Dust.NewDust(new Vector2(i * 16, j * 16 - 10), 54, 16, DustID.Dirt, 0.0f, -1, 0, new Color(), 0.5f);
                Dust.NewDust(new Vector2(i * 16, j * 16 - 10), 75, 16, DustID.Dirt, 0.0f, 0, 0, new Color(), 0.5f);
            }
        }
    }

    public class ArchivePotBig : ArchivePot
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;
        public override void SetStaticDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.addTile(Type);

            TileID.Sets.DisableSmartCursor[Type] = true;

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(215, 186, 87), name);
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 2;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            SoundEngine.PlaySound(SoundID.Item27);
            for (int k = 0; k < 8; k++)
            {
                Dust.NewDust(new Vector2(i * 16, j * 16 - 10), 54, 16, DustID.Dirt, 0.0f, -1, 0, new Color(), 0.5f);
                Dust.NewDust(new Vector2(i * 16, j * 16 - 10), 75, 16, DustID.Dirt, 0.0f, 0, 0, new Color(), 0.5f);
            }
        }
    }
}