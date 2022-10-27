using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Base;
using OvermorrowMod.Content.NPCs.Shades;
using OvermorrowMod.Content.Tiles.Ambient;
using OvermorrowMod.Content.Tiles.Underground;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Content.WorldGeneration
{
    public class VanillaBlocking : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int BiomeIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Sand"));
            if (BiomeIndex != -1) tasks.RemoveAt(BiomeIndex);

            BiomeIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Slush Check"));
            if (BiomeIndex != -1) tasks.RemoveAt(BiomeIndex);


            BiomeIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Slush"));
            if (BiomeIndex != -1) tasks.RemoveAt(BiomeIndex);

            BiomeIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Ice"));
            if (BiomeIndex != -1) tasks.RemoveAt(BiomeIndex);


            BiomeIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Full Desert"));
            if (BiomeIndex != -1) tasks.RemoveAt(BiomeIndex);

            int DungeonIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));
            if (DungeonIndex != -1) tasks.RemoveAt(DungeonIndex);

            BiomeIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Corruption"));
            if (BiomeIndex != -1) tasks.RemoveAt(BiomeIndex);

            BiomeIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle"));
            if (BiomeIndex != -1) tasks.RemoveAt(BiomeIndex);

            BiomeIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle Trees"));
            if (BiomeIndex != -1) tasks.RemoveAt(BiomeIndex);

            BiomeIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Buried Chests"));
            if (BiomeIndex != -1) tasks.RemoveAt(BiomeIndex);

        }
    }
}