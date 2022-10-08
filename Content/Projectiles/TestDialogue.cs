using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Core;
using System;
using System.Xml;
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
            if (Projectile.ai[0]++ == 0)
            {
                DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();
                
                XmlDocument doc2 = ModUtils.GetXML(AssetDirectory.Popup + "Misery.xml");
                player.AddPopup(doc2);

                XmlDocument doc3 = ModUtils.GetXML(AssetDirectory.Popup + "CPR.xml");
                player.AddPopup(doc3);

                XmlDocument doc = ModUtils.GetXML(AssetDirectory.Popup + "Reeses.xml");
                player.AddPopup(doc);
            }
        }
    }
}