﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles
{
    // This class shows off many things common to Lamp tiles in Terraria. The process for creating this example is detailed in: https://github.com/tModLoader/tModLoader/wiki/Advanced-Vanilla-Code-Adaption#examplelamp-tile
    // If you can't figure out how to recreate a vanilla tile, see that guide for instructions on how to figure it out yourself.
    internal class HerosAltar : ModTile
    {
        public override void SetStaticDefaults()
        {
            // Main.tileFlame[Type] = true; This breaks it.
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileWaterDeath[Type] = false;
            Main.tileLavaDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            MinPick = 45;
            // name.SetDefault("Hero's Altar");
            AddMapEntry(new Color(253, 221, 3), name);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
        }
    }
}
