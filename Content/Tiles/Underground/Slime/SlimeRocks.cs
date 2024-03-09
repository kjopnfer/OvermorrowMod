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

namespace OvermorrowMod.Content.Tiles.Underground.Slime
{
    public abstract class SlimeRock : ModTile
    {
        public virtual int Width => 2;
        public virtual int Height => 2;
        public virtual int[] CoordinateHeights => new int[] { 16, 16 };

        public override void SetStaticDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Width = Width;
            TileObjectData.newTile.Height = Height;
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
        public override int Width => 3;
        public override int Height => 2;
    }

    public class SlimeRock2 : SlimeRock
    {
        public override int Width => 3;
        public override int Height => 2;
    }

    public class SlimeRock3 : SlimeRock
    {
        public override int Width => 1;
        public override int Height => 1;
        public override int[] CoordinateHeights => new int[] { 16 };
    }

    public class SlimeRock4 : SlimeRock
    {
        public override int Width => 3;
        public override int Height => 2;
    }

    public class SlimeRock5 : SlimeRock
    {
        public override int Width => 3;
        public override int Height => 2;
    }

    public class SlimeRock6 : SlimeRock
    {
        public override int Width => 4;
        public override int Height => 3;
        public override int[] CoordinateHeights => new int[] { 16, 16, 16 };
    }

    public class SlimeRock7 : SlimeRock
    {
        public override int Width => 3;
        public override int Height => 2;
    }
}