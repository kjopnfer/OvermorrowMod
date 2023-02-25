/*using OvermorrowMod.Common.NPCs;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Arbiter
{
    internal class ArbiterHead : Arbiter
    {
        public override string Texture => "OvermorrowMod/Content/NPCs/Arbiter/Arbiter_Head";
        public override void SetDefaults()
        {
            // Head is 10 defence, body 20, tail 30.
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.aiStyle = -1;
            NPC.width = 62;
            NPC.height = 78;
            NPC.townNPC = true;
        }

        public override void Init()
        {
            base.Init();
            head = true;
            flies = true;
        }

        private int attackCounter;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(attackCounter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            attackCounter = reader.ReadInt32();
        }

        public override void CustomBehavior()
        {

        }

        public override string GetChat()
        {
            List<string> dialogue = new List<string>
            {
                "devourer of gods sucks",
                "im not a worm boss lol",
                "catboy",
                "fire/fire/fire",
                "have u heard of the high elves?",
                "if u meet iorich ignore his dad jokes",
            };

            return Main.rand.Next(dialogue);
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }

    }

    internal class ArbiterBody : Arbiter
    {
        public override string Texture => "OvermorrowMod/Content/NPCs/Arbiter/Arbiter_Body";
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerBody);
            NPC.aiStyle = -1;
            NPC.width = 60;
            NPC.height = 68;
        }
    }

    internal class ArbiterTail : Arbiter
    {
        public override string Texture => "OvermorrowMod/Content/NPCs/Arbiter/Arbiter_Body";

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerTail);
            NPC.aiStyle = -1;
            NPC.width = 60;
            NPC.height = 68;
        }

        public override void Init()
        {
            base.Init();
            tail = true;
        }
    }

    // I made this 2nd base class to limit code repetition.
    public abstract class Arbiter : Worm
    {
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanBeHitByItem(Player player, Item item) => false;
        public override bool? CanBeHitByProjectile(Projectile projectile) => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arbiter");
        }

        public override void Init()
        {
            minLength = 150;
            maxLength = 151;
            tailType = ModContent.NPCType<ArbiterTail>();
            bodyType = ModContent.NPCType<ArbiterBody>();
            headType = ModContent.NPCType<ArbiterHead>();
            speed = 10.5f;
            turnSpeed = 0.045f;
        }
    }
}*/