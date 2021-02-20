using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod
{
    public static class XPPacket
    {
        public static void Read(BinaryReader reader)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                WardenDamagePlayer player = Main.LocalPlayer.GetModPlayer<WardenDamagePlayer>();
                player.AddSoul((int)reader.ReadInt32());
            }
        }

        public static bool Write(int soulEssence, int target)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = OvermorrowModFile.Mod.GetPacket();
                packet.Write((byte)Message.AddSoul);
                packet.Write(soulEssence);
                packet.Send();
                return true;
            }

            return false;
        }
    }
}