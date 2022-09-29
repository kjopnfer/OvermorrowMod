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
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Portraits/Guide/GuideSmug").Value;
            /*if (Projectile.ai[0]++ == 0)
            {
                Main.NewText("a");
                player.AddDialogue(texture, "John Cena", "Do you enjoy going through hell? ", 60, 120, Color.LimeGreen, new Color(52, 201, 235));
            }
            else if (Projectile.ai[0] == 60)
            {
                Main.NewText("b");
                player.AddDialogue(texture, "its ya boi", "You've harnessed great power, but you wield it for no one but yourself.", 60, 120, Color.Orange, new Color(52, 201, 235));
            }
            else if (Projectile.ai[0] == 120)
            {
                Main.NewText("c");
                texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Portraits/Rhiannon/Speak").Value;
                player.AddDialogue(texture, "???", "Once the dust has settled and only one remains, if it is you, what value will this have had?!", 60, 120, Color.Red, new Color(52, 201, 235));
            }
            else if (Projectile.ai[0] == 180)
            {
                Main.NewText("d");
                player.AddDialogue(texture, "John Cena", "An upstart who recklessly stole and killed their way to power. I wonder, who does that remind me of...?", 60, 120, Color.LimeGreen, new Color(52, 201, 235));
            }
            else if (Projectile.ai[0] == 240)
            {
                Main.NewText("e");
                player.AddDialogue(texture, "its ya boi", "You have no stake in this battle. No one gave you any say in this matter!", 60, 120, Color.Orange, new Color(52, 201, 235));
            }
            else if(Projectile.ai[0] == 300)
            {
                texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Portraits/Rhiannon/Menacing").Value;
                player.AddDialogue(texture, "its ya boi", "STOP POSTING ABOUT AMONG US! I'M TIRED OF SEEING IT! MY FRIENDS ON TIKTOK SEND ME MEMES, ON DISCORD IT'S FUCKING MEMES! I was in a server, right? and ALL OF THE CHANNELS were just among us stuff. ", 60, 120, Color.Orange, "FF0000");
            }*/

            if (Projectile.ai[0]++ == 0)
            {
                player.AddPopup(texture, "Do [you] enjoy going through [hell?]", 60, 120, new Color(52, 201, 235), true, false);
            }
            else if (Projectile.ai[0] == 60)
            {
                player.AddPopup(texture, "[You've harnessed great power, but you wield] it for no one but yourself.", 60, 120, new Color(52, 201, 235), false, false);
            }
            else if (Projectile.ai[0] == 120)
            {
                player.AddPopup(texture, "Once the [dust] has settled and [only] one remains, if it is you, what value will this have had?!", 60, 120, new Color(52, 201, 235), false, false);
            }
            else if (Projectile.ai[0] == 180)
            {
                player.AddPopup(texture, "An upstart who recklessly stole [and] killed their way to power. I wonder, who does that remind me of...?", 60, 120, new Color(52, 201, 235), false, false);
            }
            else if (Projectile.ai[0] == 240)
            {
                player.AddPopup(texture, "You have no stake in this battle. [No] one gave you any say in this matter!", 60, 120, new Color(52, 201, 235), false, true);
            }  
        }

        public override void Kill(int timeLeft)
        {
            Main.NewText("im DEAD");
        }
    }
}