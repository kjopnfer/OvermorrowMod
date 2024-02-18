using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Dialogue
{
    public class GuideCamp : DialogueWindow
    {
        public GuideCamp() : base()
        {
            Dialogue = new[]
            {
                new DialogueNode(
                    "start",
                    new Text[]
                    {
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "GuideWave").Value,
                            "Came across you unconscious so I took care of you for a bit, no big deal.", 60),
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "GuideWave").Value,
                            "RIP, looks like my fire got put out, wanna help me start it again?", 60)
                    },
                    new DialogueChoice[]
                    {
                        new DialogueChoice("Fine")
                    }
                ),
            };

        }
    }
}