using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvermorrowMod.Common.Netcode
{
    internal class NetworkMessageHandler
    {
        public static void HandlePacket(BinaryReader reader, int fromWho)
        {
            switch (reader.ReadByte()) {
                case (byte)PacketType.QuestPacket:
                    break;
                default:
                    // No known packet type if this happens.
                    break;
            }
        }
    }
}
