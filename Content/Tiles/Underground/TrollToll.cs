using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.NPCs.RockCrawler;
using OvermorrowMod.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
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

            animationFrameHeight = 126;
            minPick = 999;

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Troll Toll");
            AddMapEntry(new Color(24, 23, 23), name);
        }

        public override void MouseOver(int i, int j)
        {
            base.MouseOver(i, j);
        }

        public override bool NewRightClick(int i, int j)
        {
            return base.NewRightClick(i, j);
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
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
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height)
        {
            offsetY = 2;
        }
    }
}