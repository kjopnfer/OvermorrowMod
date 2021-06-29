using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using System;

namespace OvermorrowMod.NPCs.Bosses.EvilBoss
{
    public class BloodyMeatBall : ModProjectile
    {
        private int KeepAliveTime2 = 1; 
        private int KeepAliveTime = 1;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.TheMeatball;
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.FlamingScythe);
            aiType = ProjectileID.FlamingScythe;
            projectile.tileCollide = false;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 50;
            projectile.width = 34;
            projectile.height = 34;
            projectile.penetrate = -1;
        }
    }
}