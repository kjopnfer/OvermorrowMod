using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Common.Pathfinding;
using OvermorrowMod.Content.Projectiles;
using OvermorrowMod.Content.UI.SpeechBubble;
using OvermorrowMod.Content.WorldGeneration;
using OvermorrowMod.Core;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Content.NPCs.Town.Sojourn
{
    public partial class Feyden : ModNPC
    {
        DialoguePlayer dialoguePlayer => Main.LocalPlayer.GetModPlayer<DialoguePlayer>();
        private void HandleNPCDialogue()
        {
            // TODO: Probably make these stored within individual trigger classes or something
            if (OvermorrowWorld.savedFeyden)
            {
                if (!dialoguePlayer.feydenExitCave &&
                    NPC.Center.X >= GuideCamp.SlimeCaveEntrance.X - (150 * 16) &&
                    NPC.Center.Y <= GuideCamp.SlimeCaveEntrance.Y - (65 * 16))
                {
                    dialoguePlayer.feydenExitCave = true;
                    dialoguePlayer.AddNPCPopup(NPCID.Guide, ModUtils.GetXML(AssetDirectory.Popups + "FeydenEscort.xml"), "EXIT_CAVE");
                }

                if (!dialoguePlayer.feydenSojournClose &&
                    NPC.Center.X >= TownGeneration.SojournLocation.X - (400 * 16))
                {
                    dialoguePlayer.feydenSojournClose = true;
                    dialoguePlayer.AddNPCPopup(NPCID.Guide, ModUtils.GetXML(AssetDirectory.Popups + "FeydenEscort.xml"), "CLOSE_SOJOURN");
                }
            }
        }
    }
}