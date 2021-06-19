using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Tiles.Ambient;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Tiles
{
    public class GlowBlock : ModTile
    {

        int Random = Main.rand.Next(1, 5);


        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            drop = ModContent.ItemType<OvermorrowMod.Items.Placeable.Tiles.GlowBlock>();
            AddMapEntry(new Color(0, 25, 25));

            /*TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            //TileObjectData.newTile.Origin = new Point16(0, 0);
            //TileObjectData.newTile.StyleHorizontal = true;
            //TileObjectData.newTile.StyleMultiplier = 5;
            //TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.addAlternate(0);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TEShenanigans>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.addTile(Type);*/
        }

        public static bool PlaceObject(int x, int y, int type, bool mute = false, int style = 0, int alternate = 0, int random = -1, int direction = -1)
        {
            TileObject toBePlaced;
            if (!TileObject.CanPlace(x, y, type, style, direction, out toBePlaced, false))
            {
                return false;
            }
            toBePlaced.random = random;
            if (TileObject.Place(toBePlaced) && !mute)
            {
                WorldGen.SquareTileFrame(x, y, true);
            }
            return false;
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            Tile tileLeft = Framing.GetTileSafely(i - 1, j);
            Tile tileRight = Framing.GetTileSafely(i + 1, j);
            Random = Main.rand.Next(1, 50);

            if(Random == 4)
            {
                WorldGen.PlaceTile(i, j + 1, 1);
            }

            if (Random == 14)
            {
                WorldGen.PlaceTile(i, j - 1, 1);
            }

            if (Random == 20)
            {
                WorldGen.PlaceTile(i - 1, j, 1);
            }

            if (Random == 7)
            {
                WorldGen.PlaceTile(i + 1, j, 1);
            }

            // Grow plants
            if (WorldGen.genRand.NextBool(2) && !tileAbove.active() && !tileBelow.lava())
            {
                if (!tile.bottomSlope() && !tile.topSlope() && !tile.halfBrick())
                {
                    tileAbove.type = (ushort)ModContent.TileType<GlowPlants>();
                    tileAbove.active(true);
                    tileAbove.frameY = 0;
                    tileAbove.frameX = (short)(WorldGen.genRand.Next(7) * 18); // 7 is the amount of plants in the sprite
                    WorldGen.SquareTileFrame(i, j - 1, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                    }
                }
            }

            // Grow vines
            if (WorldGen.genRand.NextBool(2) && !tileBelow.active() && !tileBelow.lava())
            {
                if (!tile.bottomSlope())
                {
                    tileBelow.type = (ushort)ModContent.TileType<GlowWorms>();
                    tileBelow.active(true);
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                    }
                }
            }
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref Color drawColor, ref int nextSpecialDrawIndex)
        {
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            if (!tileAbove.active() && tileAbove.liquid > 0)
            {
                if (Main.rand.NextFloat() < 0.0001f)
                {
                    Dust dust;
                    // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
                    Vector2 position = new Vector2(i * 16, j * 16);
                    dust = Terraria.Dust.NewDustPerfect(position, 92, new Vector2(0f, -5.53f), 0, new Color(255, 255, 255), 1f);
                    dust.noGravity = true;
                }
            }

            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            Tile tileLeft = Framing.GetTileSafely(i - 1, j);
            Tile tileRight = Framing.GetTileSafely(i + 1, j);
            if (tileBelow.active() && tileAbove.active() && tileLeft.active() && tileRight.active() &&
                tileBelow.type == Type && tileAbove.type == Type && tileLeft.type == Type && tileRight.type == Type)
            {
                if (Main.rand.NextFloat() < 0.0001f)
                {
                    Dust dust;
                    // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
                    Vector2 position = new Vector2(i * 16, j * 16);
                    dust = Main.dust[Terraria.Dust.NewDust(position, 30, 30, 41, 0f, 0f, 0, new Color(0, 255, 242), 0.9210526f)];
                    dust.shader = GameShaders.Armor.GetSecondaryShader(25, Main.LocalPlayer);
                }


            }
        }
    }
}