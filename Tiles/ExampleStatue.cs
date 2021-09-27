using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.World.Generation;
using OvermorrowMod.Projectiles.Melee;
namespace OvermorrowMod.Tiles
{
	public class ExampleStatue : ModTile
	{
		public override void SetDefaults() {
			Main.tileFrameImportant[Type] = true;
			Main.tileObsidianKill[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Statue");
			AddMapEntry(new Color(144, 148, 144), name);
			dustType = DustID.Silver;
			disableSmartCursor = true;
		}


		public override void HitWire(int i, int j) 
		{
			// Find the coordinates of top left tile square through math
			int y = j - Main.tile[i, j].frameY / 18;
			int x = i - Main.tile[i, j].frameX / 18;

			Wiring.SkipWire(x, y);
			Wiring.SkipWire(x, y + 1);
			Wiring.SkipWire(x, y - 1);

			// We add 16 to x to spawn right between the 2 tiles. We also want to right on the ground in the y direction.
			int spawnX = x * 16;
			int spawnY = y * 16 + 16;

			// This example shows both item spawning code and npc spawning code, you can use whichever code suits your mod
				// If you want to make an NPC spawning statue, see below.
				// 30 is the time before it can be used again. NPC.MechSpawn checks nearby for other spawns to prevent too many spawns. 3 in immediate vicinity, 6 nearby, 10 in world.
				if (Wiring.CheckMech(x, y, 30)) 
				{
                    int proj = Projectile.NewProjectile(spawnX + 8, spawnY + 10, 0, 5, ProjectileID.Blizzard, 25, 3f, Main.myPlayer);
                    Main.projectile[proj].hostile = true;
				}
		}
 /*       public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile Above = Framing.GetTileSafely(i, j - 1);
            if (!Above.active())
            {
                Above.KillTile(i, j);
            }

            return true;
        }
	}*/
	}
}
