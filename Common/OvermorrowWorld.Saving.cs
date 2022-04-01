using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using OvermorrowMod.Content.Items.Weapons.Melee.GraniteChomper;
using OvermorrowMod.Content.Tiles;
using OvermorrowMod.Content.Tiles.Ambient.WaterCave;
using OvermorrowMod.Content.Tiles.TrapOre;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;
using OvermorrowMod.Content.Items.Weapons.Magic.WarpRocket;
using OvermorrowMod.Content.Tiles.DesertTemple;
using OvermorrowMod.Content.Tiles.Ores;

namespace OvermorrowMod.Common
{
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

    public partial class OvermorrowWorld : ModWorld
    {
        // Bosses
        public static bool downedDarude;
        public static bool downedTree;
        public static bool downedDrippler;
        public static bool downedDrake;
        public static bool downedLady;
        public static bool downedKnight;

        public override void Initialize()
        {
            downedTree = false;
            downedDarude = false;
            downedDrippler = false;
            downedDrake = false;
        }

        public override TagCompound Save()
        {
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


            return new TagCompound
            {
                ["downed"] = downed
            };

        }

        public override void Load(TagCompound tag)
        {
            var downed = tag.GetList<string>("downed");
            downedTree = downed.Contains("Iorich");
            downedDarude = downed.Contains("Dharuud");
            downedDrippler = downed.Contains("Dripplord");
            downedDrake = downed.Contains("Storm Drake");
        }

        public override void LoadLegacy(BinaryReader reader)
        {
            int loadVersion = reader.ReadInt32();
            if (loadVersion == 0)
            {
                BitsByte flags = reader.ReadByte();
                downedTree = flags[0];
                downedDarude = flags[1];
                downedDrippler = flags[2];
                downedDrake = flags[3];
            }
            else
            {
                mod.Logger.WarnFormat("Overmorrow: Unknown loadVersion: {0}", loadVersion);
            }
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