using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModTile;
using static Terraria.ModLoader.ModContent;
using static Terraria.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using OvermorrowMod.Common.Players;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Content.Tiles.UVBiome
{
    //bad code (bad code) using UVGlovalTile for the same shits
    public abstract class baseUVtile : ModTile
    {
        public int TileOn;
        public int TileOff;

        //"RadiantShadows/Tiles/TestTileBlock";
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileMerge[Type][TileID.Stone] = true;
            Main.tileMerge[Type][TileID.Sand] = true;
            Main.tileMerge[Type][TileID.Mud] = true;
            MinPick = 0;
            /*ModTranslation name = CreateMapEntryName();
            name.SetDefault("Base UV tile");
            AddMapEntry(new Color(172, 23, 99), name);*/
        }

        //if ever decided to change code here remember to do so in glimsporevine too
        //ffs why couldn't you just use GlobalTile frank smh
        public override void NearbyEffects(int i, int j, bool closer)
        {
            OvermorrowModPlayer OMMP = Main.LocalPlayer.GetModPlayer<OvermorrowModPlayer>();       
            int offCheckCount = 0;
            //Main.NewText("offCheckCount: " + offCheckCount);
            for (int h = 0; h < OMMP.UVBubbles.Count; h++)
            {
                float vectorDisX = (OMMP.UVBubbles[h].Position.X - (i * 16));
                float vectorDisY = (OMMP.UVBubbles[h].Position.Y - (j * 16));
                float center = (float)Math.Pow(vectorDisX, 2) + (float)Math.Pow(vectorDisY, 2);
                if (/*(!uVGogglesPplayer.UVEffect) &&*/ !((center <= Math.Pow(OMMP.UVBubbles[h].Radius, 2))))
                {
                    offCheckCount++;
                }
            }
            if (offCheckCount == OMMP.UVBubbles.Count)
            {
                Main.tile[i, j].TileType = (ushort)TileOff;
                WorldGen.SquareTileFrame(i, j);
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendTileSquare(-1, i, j, 1);
                return;
            }
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.24f;
            g = 0.06f;
            b = 0.24f;
        }
    }

    public abstract class baseUVtileOff : baseUVtile
    {
        public int TileOn;
        public bool isOn;

        public override void NearbyEffects(int i, int j, bool closer)
        {
            OvermorrowModPlayer OOMP = Main.LocalPlayer.GetModPlayer<OvermorrowModPlayer>();

            isOn = false;
            for (int h = 0; h < OOMP.UVBubbles.Count; h++)
            {
                float vectorDisX = (OOMP.UVBubbles[h].Position.X - (i * 16));
                float vectorDisY = (OOMP.UVBubbles[h].Position.Y - (j * 16));
                float center = (float)Math.Pow(vectorDisX, 2) + (float)Math.Pow(vectorDisY, 2);
                if ((center <= Math.Pow(OOMP.UVBubbles[h].Radius, 2)))
                    isOn = true;
            }
            if (isOn)
            {
                Main.tile[i, j].TileType = (ushort)TileOn;
                WorldGen.SquareTileFrame(i, j);
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendTileSquare(-1, i, j, 1);
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0f;
            g = 0f;
            b = 0f;
        }
    }
}