using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Town.Sojourn
{
    public class Feyden_Bound : ModNPC
    {
        public override bool NeedSaving() => true;
        public override bool UsesPartyHat() => false;
        public override bool CheckActive() => false;
        public override bool CanChat() => true;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new(0)
            {
                Hide = true
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.width = 28;
            NPC.height = 32;
            NPC.aiStyle = -1;
            NPC.defense = 0;
            NPC.lifeMax = 250;
            NPC.npcSlots = 0;
            NPC.dontTakeDamage = true;
            NPC.dontTakeDamageFromHostiles = true;

            NPC.townNPC = true;
            TownNPCStayingHomeless = true;
        }

        public override void AI()
        {
            NPC.homeless = true;
            NPC.life = NPC.lifeMax;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs) => false;

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "get up NOW";

            base.SetChatButtons(ref button, ref button2);
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                Main.NewText("transform here");
            }

            base.OnChatButtonClicked(firstButton, ref shopName);
        }

        public override string GetChat()
        {
            return "placeholder";
        }
    }
}