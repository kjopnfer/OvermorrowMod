using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ID;
using OvermorrowMod.Common.Players;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Content.Tiles.UVBiome
{
    public class UVMultiGlobalTile : GlobalTile
    {
        public static List<int[]> UVMultiTiles = new List<int[]>();
        public override void NearbyEffects(int i, int j, int type, bool closer)
        {
            bool isOn = true;
            base.NearbyEffects(i, j, type, closer);
            OvermorrowModPlayer OMMP = Main.LocalPlayer.GetModPlayer<OvermorrowModPlayer>();
            for (int poopshid = 0; UVMultiTiles.Count > poopshid; poopshid++)
            {
                if (UVMultiTiles[poopshid][0] == type)
                {
                    //Main.NewText($"{i} {j} {Framing.GetTileSafely(i,j).frameX} {Framing.GetTileSafely(i, j).frameY}");
                    isOn = false;
                    for (int h = 0; h < OMMP.UVBubbles.Count; h++)
                    {
                        float vectorDisX = (OMMP.UVBubbles[h].Position.X - (i * 16));
                        float vectorDisY = (OMMP.UVBubbles[h].Position.Y - (j * 16));
                        float center = (float)Math.Pow(vectorDisX, 2) + (float)Math.Pow(vectorDisY, 2);
                        if (/*uVGogglesPplayer.UVRevealing*/ true)
                        {
                            if ((center <= Math.Pow(OMMP.UVBubbles[h].Radius, 2)))
                                isOn = true;
                        }
                    }
                    if (isOn /*&& !wasOn*/)
                    {
                        if (Framing.GetTileSafely(i, j).TileFrameY - (short)(UVMultiTiles[poopshid][1] * UVMultiTiles[poopshid][2]) >= 0)
                            Framing.GetTileSafely(i, j).TileFrameY -= (short)(UVMultiTiles[poopshid][1] * UVMultiTiles[poopshid][2]);
                    }
                    if (!isOn /*&& wasOn*/)
                    {
                        if (Framing.GetTileSafely(i, j).TileFrameY < (short)(UVMultiTiles[poopshid][1] * UVMultiTiles[poopshid][2]))
                            Framing.GetTileSafely(i, j).TileFrameY += (short)(UVMultiTiles[poopshid][1] * UVMultiTiles[poopshid][2]);
                    }
                }
            }
        }
    }
}