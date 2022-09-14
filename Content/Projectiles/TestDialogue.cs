using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Projectiles
{
    public class TestDialogue : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("DIALOGUE TEST");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 420;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();

            if (Projectile.ai[0]++ == 0)
            {
                Main.NewText("a");
                player.AddDialogue("John Cena", "Do [you] enjoy going through [hell?]", 60, 120, Color.LimeGreen, "34C9EB");
            }
            else if (Projectile.ai[0] == 60)
            {
                Main.NewText("b");
                player.AddDialogue("its ya boi", "[You've harnessed great power, but you wield] it for no one but yourself.", 60, 120, Color.Orange, "34C9EB");
            }
            else if (Projectile.ai[0] == 120)
            {
                Main.NewText("c");
                player.AddDialogue("???", "Once the [dust] has settled and [only] one remains, if it is you, what value will this have had?!", 60, 120, Color.Red, "34C9EB");
            }
            else if (Projectile.ai[0] == 180)
            {
                Main.NewText("d");
                player.AddDialogue("John Cena", "An upstart who recklessly stole [and] killed their way to power. I wonder, who does that remind me of...?", 60, 120, Color.LimeGreen, "34C9EB");
            }
            else if (Projectile.ai[0] == 240)
            {
                Main.NewText("e");
                player.AddDialogue("its ya boi", "You have no stake in this battle. [No] one gave you any say in this matter!", 60, 120, Color.Orange, "34C9EB");
            }
            else if(Projectile.ai[0] == 300)
            {
                player.AddDialogue("its ya boi", "[STOP POSTING ABOUT AMONG US! I'M TIRED OF SEEING IT! MY FRIENDS ON TIKTOK SEND ME MEMES, ON DISCORD IT'S FUCKING MEMES! I was in a server, right? and ALL OF THE CHANNELS were just among us stuff.]", 60, 120, Color.Orange, "FF0000");
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.NewText("im DEAD");
        }
    }
}