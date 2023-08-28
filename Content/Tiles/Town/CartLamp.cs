using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.NPCs.Carts;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Town
{
    public class CartLamp : ModTile
    {
        public override bool CanExplode(int i, int j) => false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;

            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.CoordinateHeights = new int[5] { 16, 16, 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Reaper Buff Station Thing");
            AddMapEntry(Color.Red, name);
        }
    }

    public class CartLampTE : ModTileEntity
    {
        int counter = 0;
        public Vector2 LampPosition => Position.ToWorldCoordinates(16, 16);
        public override void Update()
        {
            int detectionRange = 20 * 16;

            foreach (Player player in Main.player)
            {
                if (Vector2.DistanceSquared(player.Center, LampPosition) < detectionRange * detectionRange)
                {
                    if (counter != 420)
                    {
                        counter++;
                    }
                    break;
                }
            }

            if (counter == 420)
            {
                NPC.NewNPC(null, (int)LampPosition.X - 90, (int)LampPosition.Y, ModContent.NPCType<Cart>());

                counter++;
            }
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.TileType != ModContent.TileType<CartLamp>())
            {
                Kill(Position.X, Position.Y);
            }

            return tile.HasTile && tile.TileType == ModContent.TileType<CartLamp>();
        }
    }
}