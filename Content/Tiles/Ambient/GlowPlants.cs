using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Tiles.WaterCave;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.Ambient
{
    public class GlowPlants : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            DustType = DustID.Blood;
            HitSound = SoundID.Grass;
            AddMapEntry(new Color(0, 200, 200));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.0f;
            g = 0.5f;
            b = 0.5f;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 2;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 2;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);

            if (!tileBelow.HasTile || tileBelow.IsHalfBlock || tileBelow.TopSlope || tileBelow.TileType != ModContent.TileType<GlowBlock>())
            {
                WorldGen.KillTile(i, j);
            }

            return true;
        }
    }
}