using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Players;
using System;
using System.Collections.Generic;
using Terraria.ID;

namespace OvermorrowMod.Content.Tiles.UVBiome
{
    public class UVGlobalTile : GlobalTile
    {
        public static List<int[]> UVTiles = new List<int[]>();
        public int UVTileOn;
        public int UVTileOff;
        bool isOn;

        public override void NearbyEffects(int i, int j, int type, bool closer)
        {
            OvermorrowModPlayer OMMP = Main.LocalPlayer.GetModPlayer<OvermorrowModPlayer>();
            for (int poopshid = 0; UVTiles.Count > poopshid; poopshid++)
            {
                if (UVTiles[poopshid][0] == type)
                {
                    int offCheckCount = 0;
                    for (int h = 0; h < OMMP.UVBubbles.Count; h++)
                    {
                        float vectorDisX = (OMMP.UVBubbles[h].Position.X - (i * 16));
                        float vectorDisY = (OMMP.UVBubbles[h].Position.Y - (j * 16));
                        float center = (float)Math.Pow(vectorDisX, 2) + (float)Math.Pow(vectorDisY, 2);
                        if (/*(!uVGogglesPplayer.UVEffect) &&*/ !((center <= Math.Pow(OMMP.UVBubbles[h].Radius, 2))))
                            offCheckCount++;
                    }
                    if (offCheckCount == OMMP.UVBubbles.Count)
                    {
                        Main.tile[i, j].TileType = (ushort)UVTiles[poopshid][1];
                        WorldGen.SquareTileFrame(i, j);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendTileSquare(-1, i, j, 1);
                        return;
                    }
                }
                if (UVTiles[poopshid][1] == type)
                {
                    isOn = false;
                    for (int h = 0; h < OMMP.UVBubbles.Count; h++)
                    {
                        float vectorDisX = (OMMP.UVBubbles[h].Position.X - (i * 16));
                        float vectorDisY = (OMMP.UVBubbles[h].Position.Y - (j * 16));
                        float center = (float)Math.Pow(vectorDisX, 2) + (float)Math.Pow(vectorDisY, 2);
                        if ((center <= Math.Pow(OMMP.UVBubbles[h].Radius, 2)))
                            isOn = true;
                        
                    }
                    if (isOn)
                    {
                        Main.tile[i, j].TileType = (ushort)UVTiles[poopshid][0];
                        WorldGen.SquareTileFrame(i, j);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendTileSquare(-1, i, j, 1);
                    }
                }
            }
        }
    }
}