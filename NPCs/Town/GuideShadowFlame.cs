using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using System;

namespace OvermorrowMod.NPCs.Town
{
    public class GuideShadowFlame : ModProjectile
    {
        private int KeepAliveTime2 = 1;
        private int Time = 0;
        int Random2 = Main.rand.Next(-10, 11);
        int Random1 = Main.rand.Next(-10, 11);
        public override string Texture => "Terraria/Projectile_" + ProjectileID.ShadowFlame;
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.ShadowFlame);
            aiType = ProjectileID.ShadowFlame;
            projectile.friendly = false;
            projectile.hostile = true;
        }



        public override void AI()
        {
            Time++;
            if(Time == 1)
            {
                Random2 = Main.rand.Next(-7, 8);
                Random1 = Main.rand.Next(-7, 8);
            }
            projectile.velocity.X += Random2 * 0.03f;
            projectile.velocity.Y += Random2 * 0.03f;
        }


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadow Tentacle");
        }
    }
}