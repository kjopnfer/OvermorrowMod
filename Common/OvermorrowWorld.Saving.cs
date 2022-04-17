using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using OvermorrowMod.Content.Items.Weapons.Melee.GraniteChomper;
using OvermorrowMod.Content.Tiles;
using OvermorrowMod.Content.Tiles.Ambient.WaterCave;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using OvermorrowMod.Content.NPCs.Shades;

namespace OvermorrowMod.Common
{
    // I legit didn't know this already existed but here is an example of how to make one I guess, lol
    /*public class VectorSerializer : TagSerializer<Vector2, TagCompound>
    {
        public override TagCompound Serialize(Vector2 value)
        {
            return new TagCompound
            {
                ["x"] = value.X,
                ["y"] = value.Y
            };
        }

        public override Vector2 Deserialize(TagCompound tag)
        {
            return new Vector2(tag.GetInt("x"), tag.GetInt("y"));
        }
    }*/

    public partial class OvermorrowWorld : ModSystem
    {
        // Bosses
        public static bool downedDarude;
        public static bool downedTree;
        public static bool downedDrippler;
        public static bool downedDrake;
        public static bool downedLady;
        public static bool downedKnight;

        // NPC persisting
        public static List<int> SavedShades = new List<int>();
        public static List<Vector2> ShadePositions = new List<Vector2>();

        public override void OnWorldLoad()
        {
            #region Boss Downed Flags
            downedTree = false;
            downedDarude = false;
            downedDrippler = false;
            downedDrake = false;
            #endregion

            // Loop through the saved NPC list and then spawn then into the world
            // Also retrieve the concurrent index of that NPC's position
            for (int i = 0; i < SavedShades.Count; i++)
            {
                Vector2 SpawnPosition = ShadePositions[i];
                NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), (int)SpawnPosition.X, (int)SpawnPosition.Y, SavedShades[i]);
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            #region Boss Downed Flags
            var downed = new List<string>();
            if (downedTree)
            {
                downed.Add("Iorich");
            }

            if (downedDarude)
            {
                downed.Add("Dharuud");
            }

            if (downedDrippler)
            {
                downed.Add("Dripplord");
            }

            if (downedDrake)
            {
                downed.Add("Storm Drake");
            }
            #endregion

            #region NPC Persistence
            // Clear the previous list in order to update it
            SavedShades.Clear();
            ShadePositions.Clear();

            // Save active NPCs and their current positions into the list
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc != null && npc.ModNPC is ShadeOrb)
                {
                    SavedShades.Add(npc.type);
                    ShadePositions.Add(npc.position);
                }
            }
            #endregion

            tag["downed"] = downed;
            tag["SavedShades"] = SavedShades;
            tag["ShadePositions"] = ShadePositions;

            base.SaveWorldData(tag);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            #region Boss Downed Flags
            var downed = tag.GetList<string>("downed");
            downedTree = downed.Contains("Iorich");
            downedDarude = downed.Contains("Dharuud");
            downedDrippler = downed.Contains("Dripplord");
            downedDrake = downed.Contains("Storm Drake");
            #endregion

            SavedShades = tag.Get<List<int>>("SavedShades");
            ShadePositions = tag.Get<List<Vector2>>("ShadePositions");
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = downedTree;
            flags[1] = downedDarude;
            flags[2] = downedDrippler;
            flags[3] = downedDrake;

            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedTree = flags[0];
            downedDarude = flags[1];
            downedDrippler = flags[2];
            downedDrake = flags[3];
        }
    }
}