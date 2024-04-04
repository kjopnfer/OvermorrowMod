using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Underground
{
    public abstract class SlimeRock : ModTile
    {
        public virtual int Width => 2;
        public virtual int Height => 2;
        public virtual int[] CoordinateHeights => new int[] { 16, 16 };
        public virtual Point16 Origin => new Point16(0, 0);
        public override void SetStaticDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Width = Width;
            TileObjectData.newTile.Height = Height;

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = Origin;

            TileObjectData.newTile.CoordinateHeights = CoordinateHeights;
            TileObjectData.addTile(Type);

            DustType = DustID.Stone;

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(49, 201, 221), name);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1);
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 2;
        }
    }

    public class SlimeRock1 : SlimeRock
    {
        public override string Texture => AssetDirectory.Content + "Tiles/Underground/Slime/SlimeRock1";

        public override int Width => 3;
        public override int Height => 2;
        public override Point16 Origin => new Point16(0, 1);
    }

    public class SlimeRock2 : SlimeRock
    {
        public override string Texture => AssetDirectory.Content + "Tiles/Underground/Slime/SlimeRock2";
        public override int Width => 3;
        public override int Height => 2;
        public override Point16 Origin => new Point16(0, 1);

    }

    public class SlimeRock3 : SlimeRock
    {
        public override string Texture => AssetDirectory.Content + "Tiles/Underground/Slime/SlimeRock3";
        public override int Width => 1;
        public override int Height => 1;
        public override int[] CoordinateHeights => new int[] { 16 };
    }

    public class SlimeRock4 : SlimeRock
    {
        public override string Texture => AssetDirectory.Content + "Tiles/Underground/Slime/SlimeRock4";
        public override int Width => 3;
        public override int Height => 2;
        public override Point16 Origin => new Point16(0, 1);
    }

    public class SlimeRock5 : SlimeRock
    {
        public override string Texture => AssetDirectory.Content + "Tiles/Underground/Slime/SlimeRock5";
        public override int Width => 3;
        public override int Height => 2;
        public override Point16 Origin => new Point16(0, 1);

    }

    public class SlimeRock6 : SlimeRock
    {
        public override string Texture => AssetDirectory.Content + "Tiles/Underground/Slime/SlimeRock6";
        public override int Width => 4;
        public override int Height => 3;
        public override int[] CoordinateHeights => new int[] { 16, 16, 16 };
        public override Point16 Origin => new Point16(0, 2);
    }

    public class SlimeRock7 : SlimeRock
    {
        public override string Texture => AssetDirectory.Content + "Tiles/Underground/Slime/SlimeRock7";
        public override int Width => 3;
        public override int Height => 2;
        public override Point16 Origin => new Point16(0, 1);

    }
}