using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace OvermorrowMod.Items.Weapons.PreHardmode.JungleBomber
{
    public class JungleEXP : ModProjectile
    {
        int Anitimer = 0;
        public override void SetDefaults()
        {
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.width = 156;
            projectile.height = 200;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 24; //The amount of time the projectile is alive for
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Atomic Boom");
            Main.projFrames[base.projectile.type] = 12;
        }

        public override void AI()
        {

            Anitimer++;
            if(Anitimer == 1)
            {
                projectile.frame += 1;
                Anitimer = 0;
            }
        }
    }
}
