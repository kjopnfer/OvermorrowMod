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
                player.AddDialogue("here", 60, 120);
            }
            else if (Projectile.ai[0] == 60)
            {
                Main.NewText("b");
                player.AddDialogue("come", 60, 120);
            }
            else if (Projectile.ai[0] == 120)
            {
                Main.NewText("c");
                player.AddDialogue("the", 60, 120);
            }
            else if (Projectile.ai[0] == 180)
            {
                Main.NewText("d");
                player.AddDialogue("big", 60, 120);
            }
            else if (Projectile.ai[0] == 240)
            {
                Main.NewText("e");
                player.AddDialogue("ass", 60, 120);
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.NewText("im DEAD");
        }
    }
}