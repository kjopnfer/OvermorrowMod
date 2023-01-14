using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using OvermorrowMod.Quests.Requirements;
using OvermorrowMod.Quests.State;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod.Quests
{
    public class QuestMapLayer : ModMapLayer
    {
        private Asset<Texture2D> questMarker;
        public override void Draw(ref MapOverlayDrawContext context, ref string text)
        {
            if (questMarker == null || questMarker.IsDisposed) questMarker = ModContent.Request<Texture2D>(AssetDirectory.Textures + "QuestMarker");

            var modPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();

            foreach (var (_, req) in Quests.State.GetActiveRequirementsOfType<TravelRequirementState>(modPlayer))
            {
                if (req.IsCompleted) continue;

                var requirement = req.Requirement as TravelRequirement;

                if (context.Draw(questMarker.Value, requirement.Location, Color.White, new SpriteFrame(1, 1, 0, 0), 1f, 1.5f, Alignment.Center).IsMouseOver)
                {
                    text = requirement.ID;
                    if (Main.mouseLeft)
                    {
                        modPlayer.SelectedLocation = requirement.ID;
                        Main.mapFullscreen = false;
                    }
                }
            }
        }
    }
}
