using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.WaterCave
{
    public class GlowWorms : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileNoFail[Type] = true;
            HitSound = SoundID.Grass;
            AddMapEntry(new Color(0, 200, 200));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.0f;
            g = 0.5f;
            b = 0.5f;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            // This recursively kills all the tiles below it
            Tile tile = Framing.GetTileSafely(i, j + 1);
            if (tile.HasTile && tile.TileType == Type)
            {
                WorldGen.KillTile(i, j + 1);
            }
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            int type = -1;

            // Get the tile that this is grown on
            if (tileAbove.HasTile && !tileAbove.BottomSlope)
            {
                type = tileAbove.TileType;
            }

            // Check the that tile above is either the block it can grow on or itself
            if (type == ModContent.TileType<GlowBlock>() || type == ModContent.TileType<CaveMud>() || type == Type)
            {
                return true;
            }

            // Otherwise, kill the tile
            WorldGen.KillTile(i, j);

            return true;
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);

            if (WorldGen.genRand.NextBool(2) && !tileBelow.HasTile && tileBelow.LiquidType != LiquidID.Lava)
            {
                bool placeWorm = false;
                int yTest = j;

                while (yTest > j - 10)
                {
                    Tile testTile = Framing.GetTileSafely(i, yTest);
                    if (testTile.BottomSlope)
                    {
                        break;
                    }
                    else if (!testTile.HasTile || testTile.TileType != ModContent.TileType<GlowBlock>())
                    {
                        yTest--;
                        continue;
                    }
                    placeWorm = true;
                    break;
                }

                if (placeWorm)
                {
                    tileBelow.TileType = Type;
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                    }
                }
            }
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            if (!tileBelow.HasTile)
            {
                if (Main.rand.NextFloat() < 0.001f)
                {
                    Dust dust;
                    // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
                    Vector2 position = new Vector2(i * 16, j * 16);
                    dust = Terraria.Dust.NewDustPerfect(position, 92, new Vector2(0f, 0.5263162f), 0, new Color(255, 255, 255), 0.65f);
                    //dust.fadeIn = 0.7105263f;
                }

            }
        }


        // Glowmask
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            base.PostDraw(i, j, spriteBatch);
        }
    }
}