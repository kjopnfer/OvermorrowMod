using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Buffs;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Core.Globals
{
    public class BuffNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        /// <summary>
        /// 60 is fully stealthed and 0 is stealthed.
        /// </summary>
        public int StealthCounter { get; private set; } = 0;

        /// <summary>
        /// Amount of time left before the NPC can gain the Stealth buff again.
        /// </summary>
        public int StealthDelay = 0;

        public override bool? DrawHealthBar(NPC npc, byte hbPosition, ref float scale, ref Vector2 position)
        {
            if (npc.HasBuff<Stealth>()) return false;

            return base.DrawHealthBar(npc, hbPosition, ref scale, ref position);
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                binaryWriter.Write(StealthCounter);
                binaryWriter.Write(StealthDelay);
            }

            base.SendExtraAI(npc, bitWriter, binaryWriter);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                StealthCounter = binaryReader.ReadInt32();
                StealthDelay = binaryReader.ReadInt32();
            }

            base.ReceiveExtraAI(npc, bitReader, binaryReader);
        }

        public override bool PreAI(NPC npc)
        {
            if (npc.HasBuff<Stealth>())
            {
                if (StealthCounter < 60) StealthCounter++;
            }
            else
            {
                if (StealthCounter > 0) StealthCounter--;
            }

            if (!npc.HasBuff<Stealth>() && StealthDelay > 0) StealthDelay--;
            npc.chaseable = !npc.HasBuff<Stealth>();
            npc.ShowNameOnHover = !npc.HasBuff<Stealth>();

            return base.PreAI(npc);
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            npc.Opacity = MathHelper.Lerp(1f, 0.25f, StealthCounter / 60f);

            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            base.UpdateLifeRegen(npc, ref damage);
        }
    }
}