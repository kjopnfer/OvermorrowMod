/*using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    public class DarkTest : ModProjectile
    {
        public override bool PreDraw(ref Color lightColor) => false;
        public override bool ShouldUpdatePosition() => false;
        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 90;
            Projectile.tileCollide = false;
        }
        
        public ref float ParentID => ref Projectile.ai[0];
        public override void AI()
        {
            NPC npc = Main.npc[(int)ParentID];
            if (npc.active && npc.type == NPCID.EyeofCthulhu)
            {
                Projectile.timeLeft = 5;

                //if (Projectile.timeLeft > 80)
                //    Projectile.ai[1] += 0.1f;
                //else if (Projectile.timeLeft < 50 && Projectile.timeLeft >= 40)
                //    Projectile.ai[1] -= 0.1f;
            }
        }
    }
}*/