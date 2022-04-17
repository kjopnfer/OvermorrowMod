using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace OvermorrowMod.Content.Tiles.Ores
{
    public class WaterCaveOre : ModTile
    {
        public override void SetStaticDefaults()
        {
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Lacusite");
            Main.tileSolid[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileMerge[Type][TileID.Stone] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            SoundType = SoundID.Tink;
            SoundStyle = 1;
            MinPick = 55;
            MineResist = 2f;
            ItemDrop = ModContent.ItemType<WaterOre>();
            AddMapEntry(new Color(102, 255, 255), name);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Player player = Main.LocalPlayer;
            int distance = (int)Vector2.Distance(new Vector2(i * 16, j * 16), player.Center);
            if (distance < 54)
            {
                SoundEngine.PlaySound(new Terraria.Audio.LegacySoundStyle(21, 1));
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }
            int height = tile.TileFrameY == 36 ? 18 : 16;
            //Main.spriteBatch.Draw(mod.GetTexture("Tiles/WaterCaveOre"), new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + zero,
            //    new Rectangle(tile.frameX, tile.frameY, 16, height), new Color(100, 100, 100), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.0f;
            g = 1f;
            b = 1f;
        }
    }
}