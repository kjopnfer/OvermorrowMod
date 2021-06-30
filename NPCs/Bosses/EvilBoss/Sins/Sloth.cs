using System;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;


namespace OvermorrowMod.NPCs.Bosses.EvilBoss.Sins
{
    public class Sloth : ModProjectile
    {

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 100;
            projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            projectile.rotation += 0.5f;
        }



        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(32, MethodHelper.SecondsToTicks(5));

        }


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sin of Sloth");
        }
    }
}