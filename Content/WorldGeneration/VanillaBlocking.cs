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
            int PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Sand"));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);

            PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Generate Ice Biome"));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);

            PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Full Desert"));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);

            PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Oasis"));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);

            int DungeonIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));
            if (DungeonIndex != -1) tasks.RemoveAt(DungeonIndex);

            PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Corruption"));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);

            PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle"));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);

            PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle Trees"));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);

            PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Buried Chests"));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);

        }

        private void RemovePass(List<GenPass> tasks, string passName)
        {
            int PassIndex = tasks.FindIndex(genpass => genpass.Name.Equals("passName"));
            if (PassIndex != -1) tasks.RemoveAt(PassIndex);
        }
    }
}