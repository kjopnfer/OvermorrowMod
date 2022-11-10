using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Metadata;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.Content.Tiles.Town
{
    public class TownBell : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.Width = 2;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(207, 167, 87));
        }
    }
}