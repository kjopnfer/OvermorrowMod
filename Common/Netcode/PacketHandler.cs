using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Netcode
{
    internal abstract class PacketHandler
    {
        public PacketType Type { get; set; }
        public abstract void HandlePacket(BinaryReader reader, int fromWho);

        protected PacketHandler(PacketType type)
        {
            Type = type;
        }

        protected ModPacket GetPacket(byte packetType, int fromWho)
        {
            var p = OvermorrowModFile.Instance.GetPacket();
            p.Write((byte)Type);
            if (Main.netMode == NetmodeID.Server)
            {
                p.Write(fromWho);
            }
            p.Write(packetType);
            return p;
        }
    }
}
