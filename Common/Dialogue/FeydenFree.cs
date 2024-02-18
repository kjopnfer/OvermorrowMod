using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Dialogue
{
    public class FeydenFree : DialogueWindow
    {
        public FeydenFree() : base()
        {
            Dialogue = new[]
            {
                new DialogueNode(
                    "start",
                    new Text[]
                    {
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Feyden").Value,
                            "Alright, you're not as useless as I thought. Thanks for the help. Just... don't expect me to write you a hero's ballad or anything.", 150),
                    },
                    new DialogueChoice[]
                    {
                        new DialogueChoice("Can't let a damsel in distress handle slimes alone, right?", "feyden_1")
                    }
                ),

                new DialogueNode(
                    "feyden_1",
                    new Text[]
                    {
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Feyden").Value,
                            "Damsel? Please. I've dealt with worse. But, I'll give you credit for showing up when I needed a hand.", 100),
                    },
                    new DialogueChoice[]
                    {
                        new DialogueChoice("Foraging in a slime den? You've got some guts.", "")
                    }
                ),

                new DialogueNode(
                    "feyden_2",
                    new Text[]
                    {
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Feyden").Value,
                            "Yeah I know, Enza would want me to stick to the safer parts of the forest, but safe is for the meek.", 100),
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Feyden").Value,
                            "We need some real grub, not the usual bland stuff. Plus, mushrooms from here are rare and extra tasty.", 100),
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Feyden").Value,
                            "Our village is in a sorry state, you know. Bandits, wild creatures, you name it. And the tavern's running low on supplies. Figured I'd brave the cave for something to spice up our meals.", 180),
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Feyden").Value,
                            "Anyways, I should head back before someone notices I'm gone.", 60),
                        new Text(
                            ModContent.Request<Texture2D>(AssetDirectory.Portrait + "Feyden").Value,
                            "You might as well tag along, at the very least I owe you a drink for your help.", 80),
                    },
                    new DialogueChoice[]
                    {
                        new DialogueChoice("That works for me.")
                    }
                ),
            };

        }
    }
}