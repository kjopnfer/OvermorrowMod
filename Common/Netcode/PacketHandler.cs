using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			p.Write(packetType);
			if (Main.netMode == NetmodeID.Server)
			{
				p.Write((byte)fromWho);
			}
			return p;
		}
	}
}
