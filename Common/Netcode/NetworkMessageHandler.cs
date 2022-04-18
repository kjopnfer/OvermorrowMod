using OvermorrowMod.Quests;
using System.IO;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Common.Netcode
{
    internal class NetworkMessageHandler
    {
        public static QuestPacketHandler Quests = new QuestPacketHandler();

        public static void HandlePacket(BinaryReader reader, int fromWho)
        {
            var type = reader.ReadByte();
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                fromWho = reader.ReadInt32();
            }

            switch (type)
            {
                case (byte)PacketType.QuestPacket:
                    Quests.HandlePacket(reader, fromWho);
                    break;
                default:
                    // No known packet type if this happens.
                    break;
            }
        }
    }
}
