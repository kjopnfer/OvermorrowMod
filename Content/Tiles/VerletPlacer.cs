using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles
{
    public class VerletPlacer : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<VerletPlacerTE>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.addTile(Type);

            MineResist = 2f;
            AddMapEntry(new Color(92, 64, 51));
        }
    }

    public class VerletPlacerTE : ModTileEntity
    {
        public int BlockID;
        public int PairedBlock;

        public VerletPoint[] points = null;
        public VerletStick[] sticks = null;

        public override void SaveData(TagCompound tag)
        {
            tag["BlockID"] = BlockID;
            tag["PairedBlock"] = PairedBlock;
        }

        public override void LoadData(TagCompound tag)
        {
            BlockID = tag.Get<int>("BlockID");
            PairedBlock = tag.Get<int>("PairedBlock");
        }

        public override void Update()
        {
            if (BlockID != 0 && BlockID % 2 == 1)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
                points = Verlet.SimulateVerlet(points, sticks, new Vector2(0, 1), 0.07f, 10, 100f);
                Verlet.DrawVerlet(points, Main.spriteBatch);

                Main.spriteBatch.End();
            }
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            int id = Place(i, j);
            VerletPlacerTE te = ByID[id] as VerletPlacerTE;

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Chains + "Bones").Value;

            // For each block that is placed, assign a new ID and then add into the global list
            te.BlockID = VerletWorld.VerletCounter++;

            // If the block's ID is 0, then the next block is 1. Their paired block is the previous tunnel, so ID - 1.
            // Therefore, for each even block, make it ID - 1, and then for each odd block make it ID + 1
            te.PairedBlock = VerletWorld.VerletCounter % 2 == 0 ? te.BlockID - 1 : te.BlockID + 1;

            Main.NewText("placed tunnel, my id is:" + te.BlockID + " my pair is:" + te.PairedBlock);

            if (VerletWorld.VerletCounter % 2 == 0)
            {
                // Retrieve the paired block ID, and their associated position
                for (int x = 0; x < ByID.Count; x++)
                {
                    TileEntity entity;
                    if (ByID.TryGetValue(x, out entity))
                    {
                        // Check if it is the matching tile entity
                        if (entity != null && entity is VerletPlacerTE tile && tile.PairedBlock == te.BlockID && te.PairedBlock == tile.BlockID)
                        {
                            Main.NewText("i generated verlet");

                            te.points = Verlet.GenerateVerlet(texture, Main.LocalPlayer.Center, Main.LocalPlayer.Center + new Vector2(200, 200), true, true);
                            //te.points = Verlet.GenerateVerlet(texture, te.Position.ToWorldCoordinates(16, 16), tile.Position.ToWorldCoordinates(16, 16), true, true);
                            te.sticks = Verlet.GetVerletSticks(te.points);
                        }
                    }
                }
            }

            return id;
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.TileType != ModContent.TileType<VerletPlacer>()) Kill(Position.X, Position.Y);

            return tile.HasTile && tile.TileType == ModContent.TileType<VerletPlacer>();
        }
    }

    public class VerletWorld : ModSystem
    {
        public static int VerletCounter;
    }
}