using OvermorrowMod.Common.Netcode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Quests
{
    internal enum QuestPacketType
    {
        TakeQuest,
        CompleteQuest,
        ResetQuests
    }

    internal class QuestPacketHandler : PacketHandler
    {
        public QuestPacketHandler() : base(PacketType.QuestPacket) { }


        public void TakeQuest(int toWho, int fromWho, string questId)
        {
            var packet = GetPacket((byte)QuestPacketType.TakeQuest, fromWho);
            packet.Write(questId);
            packet.Send(toWho, fromWho);
        }


        private void RecTakeQuest(BinaryReader reader, int fromWho)
        {
            var questId = reader.ReadString();
            if (Main.netMode == NetmodeID.Server)
            {
                TakeQuest(-1, fromWho, questId);
            }
            Main.player[fromWho].GetModPlayer<QuestPlayer>().AddQuest(Quests.QuestList[questId]);
        }

        public void CompleteQuest(int toWho, int fromWho, string questId)
        {
            var packet = GetPacket((byte)QuestPacketType.CompleteQuest, fromWho);
            packet.Write(questId);
            packet.Send(toWho, fromWho);
        }

        private void RecCompleteQuest(BinaryReader reader, int fromWho)
        {
            var questId = reader.ReadString();
            if (Main.netMode == NetmodeID.Server)
            {
                CompleteQuest(-1, fromWho, questId);
            }
            Main.player[fromWho].GetModPlayer<QuestPlayer>().CompleteQuest(questId);
        }

        public void ResetQuest(int toWho, int fromWho)
        {
            var packet = GetPacket((byte)QuestPacketType.ResetQuests, fromWho);
            packet.Send(toWho, fromWho);
        }

        private void RecResetQuest(int fromWho)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ResetQuest(-1, fromWho);
            }
            Quests.ClearAllCompletedQuests();
        }


        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            switch (reader.ReadByte())
            {
                case (byte)QuestPacketType.TakeQuest:
                    RecTakeQuest(reader, fromWho);
                    break;
                case (byte)QuestPacketType.CompleteQuest:
                    RecCompleteQuest(reader, fromWho);
                    break;
                case (byte)QuestPacketType.ResetQuests:
                    RecResetQuest(fromWho);
                    break;
            }
        }
    }
}
