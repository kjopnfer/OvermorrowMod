using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace OvermorrowMod.Projectiles.Magic
{
    public class Spliter : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 28;
            projectile.tileCollide = true;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.timeLeft = 150;
            projectile.penetrate = 1;
        }
        public override void AI()
        {
            projectile.rotation += 0.25f; 
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        
            int Hie = (int)Math.Round(target.width / 1.5f);
            int Wid = (int)Math.Round(target.height / 1.5f);
			if(target.GetGlobalNPC<OvermorrowGlobalNPC>().split < 1)
            {

                target.lifeMax /= 2;
                target.life /= 2;
                target.scale /= 1.5f;
                target.defense /= 2;
                target.width = Hie;
                target.height = Wid;
                target.GetGlobalNPC<OvermorrowGlobalNPC>().split = 1;
                int npc = NPC.NewNPC((int)target.Center.X - target.width - 2, (int)target.Center.Y, target.type);
                Main.npc[npc].scale /= 1.5f;
                Main.npc[npc].lifeMax /= 2;
                Main.npc[npc].life /= 2;
                Main.npc[npc].defense /= 2;      
                Main.npc[npc].GetGlobalNPC<OvermorrowGlobalNPC>().split = 1;    
                Main.npc[npc].width = Hie;
                Main.npc[npc].height = Wid;
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}