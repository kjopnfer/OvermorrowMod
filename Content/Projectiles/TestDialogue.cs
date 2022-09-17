using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Portraits/Rhiannon/Neutral").Value;
            if (Projectile.ai[0]++ == 0)
            {
                Main.NewText("a");
                player.AddDialogue(texture, "John Cena", "Do [you] enjoy going through [hell?]", 60, 120, Color.LimeGreen, "34C9EB");
            }
            else if (Projectile.ai[0] == 60)
            {
                Main.NewText("b");
                player.AddDialogue(texture, "its ya boi", "[You've harnessed great power, but you wield] it for no one but yourself.", 60, 120, Color.Orange, "34C9EB");
            }
            else if (Projectile.ai[0] == 120)
            {
                Main.NewText("c");
                texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Portraits/Rhiannon/Speak").Value;
                player.AddDialogue(texture, "???", "Once the [dust] has settled and [only] one remains, if it is you, what value will this have had?!", 60, 120, Color.Red, "34C9EB");
            }
            else if (Projectile.ai[0] == 180)
            {
                Main.NewText("d");
                player.AddDialogue(texture, "John Cena", "An upstart who recklessly stole [and] killed their way to power. I wonder, who does that remind me of...?", 60, 120, Color.LimeGreen, "34C9EB");
            }
            else if (Projectile.ai[0] == 240)
            {
                Main.NewText("e");
                player.AddDialogue(texture, "its ya boi", "You have no stake in this battle. [No] one gave you any say in this matter!", 60, 120, Color.Orange, "34C9EB");
            }
            else if(Projectile.ai[0] == 300)
            {
                texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Portraits/Rhiannon/Menacing").Value;
                player.AddDialogue(texture, "its ya boi", "[STOP POSTING ABOUT AMONG US! I'M TIRED OF SEEING IT! MY FRIENDS ON TIKTOK SEND ME MEMES, ON DISCORD IT'S FUCKING MEMES! I was in a server, right? and ALL OF THE CHANNELS were just among us stuff.]", 60, 120, Color.Orange, "FF0000");
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.NewText("im DEAD");
        }
    }
}