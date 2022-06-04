using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.NPCs.RockCrawler;
using OvermorrowMod.Core;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Underground
{
    public class RockEgg : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.addTile(Type);

            DustType = DustID.Stone;

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Crawler Egg");
            AddMapEntry(new Color(49, 201, 221), name);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            NPC.NewNPC(new EntitySource_TileBreak(i, j), i * 16, j * 16, ModContent.NPCType<RockCrawler>());
            SoundEngine.PlaySound(SoundID.NPCDeath1);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Tiles + "Underground/RockEgg_Glow").Value;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

            spriteBatch.Draw(texture, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = 2;
        }
    }
}