using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Items.Consumable;
using OvermorrowMod.Content.Items.Misc;
using OvermorrowMod.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Underground
{
    public class TrollToll : ModTile
    {
        public override bool CanExplode(int i, int j) => false;
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;

            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TETrollToll>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.newTile.Width = 11;
            TileObjectData.newTile.Height = 7;
            TileObjectData.newTile.CoordinateHeights = new int[]
            {
            16,
            16,
            16,
            16,
            16,
            16,
            16
            };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);

            //animationFrameHeight = 126;
            minPick = 1;

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Troll Toll");
            AddMapEntry(new Color(24, 23, 23), name);
        }

        public static TETrollToll FindTE(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int left = i - tile.frameX / 18;
            int top = j - tile.frameY / 18;

            int index = ModContent.GetInstance<TETrollToll>().Find(left, top);
            if (index == -1)
            {
                return null;
            }

            TETrollToll alter = (TETrollToll)TileEntity.ByID[index];
            return alter;

        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.player[Main.myPlayer];
            player.noThrow = 2;
            player.showItemIcon = true;
            player.showItemIcon2 = ModContent.ItemType<MonkeyStone_ShowItem>();
        }

        public override bool NewRightClick(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            TETrollToll tunnel = FindTE(i, j);

            if (tunnel != null) tunnel.Interact();

            return base.NewRightClick(i, j);
        }

        /*public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 8)
            {
                frameCounter = 0;
                frame++;

                if (frame > 4)
                {
                    frame = 0;
                }
            }
        }*/

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height)
        {
            offsetY = 2;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            TETrollToll tunnel = FindTE(i, j);

            // PreDraw works by going through each tile and then drawing that portion at that area
            // If try to draw the texture once it will draw an additional 77 times for each tile leading to weird overlap shenanigans
            // Therefore, we'll only draw the texture once if it is the origin tile
            if (tile.frameX == 0 && tile.frameY == 0)
            {
                Texture2D texture = ModContent.GetTexture(AssetDirectory.Tiles + "Underground/TrollToll_New");
                Texture2D glow = ModContent.GetTexture(AssetDirectory.Tiles + "Underground/TrollToll_Glow");
                Texture2D lamp = ModContent.GetTexture(AssetDirectory.Tiles + "Underground/TrollToll_Lamp");
                Texture2D face = ModContent.GetTexture(AssetDirectory.Tiles + "Underground/TrollFace");

                Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;

                Rectangle drawRectangle = new Rectangle(0, texture.Height / 3 * tunnel.LightFrame, texture.Width, texture.Height / 3);
                Rectangle faceRectangle = new Rectangle(0, face.Height / 5 * tunnel.TunnelFrame, face.Width, face.Height / 5);
                Rectangle miscRectangle = new Rectangle(0, lamp.Height, lamp.Width, lamp.Height);

                spriteBatch.Draw(texture, drawPos, drawRectangle, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(glow, drawPos, drawRectangle, Color.White * 0.25f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

                if (tunnel.LampOn)
                {
                    spriteBatch.Draw(lamp, drawPos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                }

                spriteBatch.Draw(face, drawPos + new Vector2((texture.Width / 2) - (face.Width / 2), 114 / 2 + 17), faceRectangle, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }

            return false;
        }
    }

    public class TETrollToll : ModTileEntity
    {
        public int TunnelID;
        public int PairedTunnel;

        public bool CanTeleport = false;
        public bool LampOn = false;

        // Face sprite
        private int FrameCounter = 0;
        public int TunnelFrame = 0; // Goes from frame 0 to 4

        // Light sprite
        private int LightCounter = 0;
        public int LightFrame = 0;

        public override TagCompound Save()
        {
            // Save the Tunnel's ID into this, and the paired Tunnel
            return new TagCompound()
            {
                ["TunnelID"] = TunnelID,
                ["PairedTunnel"] = PairedTunnel
            };
        }

        public override void Load(TagCompound tag)
        {
            // Load the Tunnel's ID and the paired Tunnel
            TunnelID = tag.Get<int>("TunnelID");
            PairedTunnel = tag.Get<int>("PairedTunnel");
        }

        public void Interact()
        {
            // Retrieve the paired tunnel ID, and their associated position
            for (int i = 0; i < ByID.Count; i++)
            {
                TileEntity entity;
                if (ByID.TryGetValue(i, out entity))
                {
                    // Check if the tile entity is the tunnel and that their pair ID is equal to this one
                    if (entity != null && entity is TETrollToll tunnel && tunnel.PairedTunnel == TunnelID && PairedTunnel == tunnel.TunnelID)
                    {
                        // Teleport the player to that position, and then remove a stone from their inventory
                        foreach (Item playerItem in Main.LocalPlayer.inventory)
                        {
                            if (playerItem.type == ModContent.ItemType<MonkeyStone>() && playerItem.stack > 0)
                            {
                                playerItem.stack--;
                                CanTeleport = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public Vector2 TunnelPosition => Position.ToWorldCoordinates(16, 16);
        public override void Update()
        {
            int detectionRange = 20 * 16;

            LampOn = false;
            if (!LampOn)
            {
                foreach (Player player in Main.player)
                {
                    if (Vector2.DistanceSquared(player.Center, TunnelPosition + new Vector2(84, 57)) < detectionRange * detectionRange)
                    {
                        LampOn = true;
                        NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
                        break;
                    }
                }
            }

            if (LampOn)
            {
                Lighting.AddLight(TunnelPosition + new Vector2(82, 82), new Color(201, 59, 8).ToVector3());

                if (LightCounter++ > 8)
                {
                    LightCounter = 0;
                    LightFrame++;

                    if (LightFrame > 1)
                    {
                        LightFrame = 0;
                    }
                }
            }
            else
            {
                LightFrame = 2;
            }



            if (CanTeleport)
            {
                FrameCounter++;
                if (FrameCounter > 8)
                {
                    FrameCounter = 0;
                    TunnelFrame++;

                    if (TunnelFrame > 4)
                    {
                        for (int i = 0; i < ByID.Count; i++)
                        {
                            TileEntity entity;
                            if (ByID.TryGetValue(i, out entity))
                            {
                                // Check if the tile entity is the tunnel and that their pair ID is equal to this one
                                if (entity != null && entity is TETrollToll tunnel && tunnel.PairedTunnel == TunnelID && PairedTunnel == tunnel.TunnelID)
                                {
                                    Main.LocalPlayer.Teleport(tunnel.Position.ToWorldCoordinates(16, 16), -1);
                                }
                            }
                        }

                        FrameCounter = 0;
                        TunnelFrame = 0;
                        CanTeleport = false;
                    }
                }
            }
        }

        public override bool ValidTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            if (!tile.active() || tile.type != ModContent.TileType<TrollToll>())
            {
                Kill(Position.X, Position.Y);
            }

            return tile.active() && tile.type == ModContent.TileType<TrollToll>();
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
        {
            int id = Place(i, j);
            TETrollToll te = TileEntity.ByID[id] as TETrollToll;

            // For each tunnel that is placed, assign a new ID and then add into the global list
            te.TunnelID = TrollWorld.TunnelCounter++;

            // If the tunnel's ID is 0, then the next tunnel is 1. Their paired tunnel is the previous tunnel, so ID - 1.
            // Therefore, for each even tunnel, make it ID - 1, and then for each odd tunnel make it ID + 1
            te.PairedTunnel = TrollWorld.TunnelCounter % 2 == 0 ? te.TunnelID - 1 : te.TunnelID + 1;

            Main.NewText("placed tunnel, my id is:" + te.TunnelID + " my pair is:" + te.PairedTunnel);

            return id;
        }
    }

    public class TrollWorld : ModWorld
    {
        public static int TunnelCounter;
    }
}