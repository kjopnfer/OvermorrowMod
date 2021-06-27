using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Tiles.TrapGems
{
    public class TrapAmethyst : ModTile
    {
        public override void SetDefaults()
        {
            TileID.Sets.Ore[Type] = false;
            Main.tileSpelunker[Type] = false;
            Main.tileMerge[TileID.Stone][Type] = true;
            Main.tileMerge[Type][TileID.Stone] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 975;
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;


            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Amethyst");
            AddMapEntry(new Color(0, 0, 0), name);
            //dustType = 9;
            soundType = SoundID.Tink;
            soundStyle = 1;
            mineResist = 2f;
            minPick = 1;
        }
        public override bool Drop(int i, int j)
        {
            for (int x = 0; x < Main.maxPlayers; x++)
            {
                float distance = Vector2.Distance(new Vector2(i * 16, j * 16), Main.player[i].Center);
                if (distance <= 350)
                {
                    Main.player[i].AddBuff(BuffID.Obstructed, 240);
                }
            }

            /*int style = 0;
            if (style == 0) // It can be useful to share a single tile with multiple styles. This code will let you drop the appropriate bar if you had multiple.
            {
                Projectile.NewProjectile(i * 16, j * 16, 0, 0, mod.ProjectileType("FakeGold"), 60, 0f, Main.myPlayer);
            }*/
            return base.Drop(i, j);
        }
    }
}