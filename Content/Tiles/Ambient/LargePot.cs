using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Ambient
{
    public class LargePot : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.RandomStyleRange = 4;
            TileObjectData.addTile(Type);

            TileID.Sets.DisableSmartCursor[Type] = true;

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Large Pot");
            AddMapEntry(new Color(151, 79, 80), name);
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 2;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            // TODO: Put shit here
            SoundEngine.PlaySound(SoundID.Item27);
            for (int k = 0; k < 8; k++)
            {
                Dust.NewDust(new Vector2(i * 16, j * 16 - 10), 54, 16, DustID.Dirt, 0.0f, -1, 0, new Color(), 0.5f);
                Dust.NewDust(new Vector2(i * 16, j * 16 - 10), 75, 16, DustID.Dirt, 0.0f, 0, 0, new Color(), 0.5f);
                Gore.NewGore(null, new Vector2(i * 16 + Main.rand.Next(-10, 10), j * 16 + Main.rand.Next(-10, 10)), new Vector2(-1, 1), Mod.Find<ModGore>("Terraria/Images/Gore_" + Main.rand.Next(698, 704)).Type, 1f);
            }
        }
    }
}
