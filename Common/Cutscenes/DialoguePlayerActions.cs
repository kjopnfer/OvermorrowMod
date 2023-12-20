using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using System.Xml;
using OvermorrowMod.Core;
using OvermorrowMod.Quests;
using System.Linq;
using OvermorrowMod.Quests.ModQuests;
using OvermorrowMod.Content.NPCs.Town.Sojourn;
using Terraria.ID;

namespace OvermorrowMod.Common.Cutscenes
{
    public partial class DialoguePlayer : ModPlayer
    {
        public bool pickupWood = false;
        public bool outDistanceDialogue = false;
        public bool guideGreeting = false;
        public bool kittFirst = true;

        public bool unlockedGuideCampfire = false;

        private int greetCounter = 0;
        private int guideCampfireCounter = 0;

        private bool feydenHitSlime = false;

        QuestPlayer questPlayer => Player.GetModPlayer<QuestPlayer>();
        DialoguePlayer dialoguePlayer => Player.GetModPlayer<DialoguePlayer>();

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //FeydenHitSlimeDialogue(target);
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            FeydenHitSlimeDialogue(target);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (proj.owner == Player.whoAmI) FeydenHitSlimeDialogue(target);
        }

        private void FeydenHitSlimeDialogue(NPC target)
        {
            if (!feydenHitSlime && questPlayer.IsDoingQuest<FeydenRescue>() && target.type == NPCID.BlueSlime)
            {
                feydenHitSlime = true;
                dialoguePlayer.AddNPCPopup(ModContent.NPCType<Feyden>(), ModUtils.GetXML(AssetDirectory.Popups + "FeydenCave.xml"), "DAMAGE_SLIME");
            }
        }
    }
}
