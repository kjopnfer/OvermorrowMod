using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.Ores
{
    public class EruditeTile : ModTile
    {
        public override void SetDefaults()
        {
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Erudite");
            Main.tileSolid[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileMerge[Type][TileID.Stone] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            soundType = SoundID.Tink;
            soundStyle = 1;
            minPick = 40;
            drop = ModContent.ItemType<EruditeOre>();
            AddMapEntry(new Color(51, 204, 51), name);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Player player = Main.LocalPlayer;
            int distance = (int)Vector2.Distance(new Vector2(i * 16, j * 16), player.Center);
            if (distance < 54)
            {
                Main.PlaySound(new Terraria.Audio.LegacySoundStyle(21, 1));
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
            int height = tile.frameY == 36 ? 18 : 16;
            Main.spriteBatch.Draw(ModContent.GetTexture(AssetDirectory.Tiles + "Ores/EruditeTile_Glow"), new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + zero,
                new Rectangle(tile.frameX, tile.frameY, 16, height), new Color(100, 100, 100), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0f;
            g = 0f;
            b = 0f;
        }
    }
}