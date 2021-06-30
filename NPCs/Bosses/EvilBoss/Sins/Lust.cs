using System;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;


namespace OvermorrowMod.NPCs.Bosses.EvilBoss.Sins
{
    public class Lust : ModProjectile
    {

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
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
            Projectile.NewProjectile(target.Center.X, target.Center.Y, -projectile.velocity.X, -projectile.velocity.Y, mod.ProjectileType("BloodHealNPC"), 2, 1f, projectile.owner, 0f);
        }


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sin of Lust");
        }
    }
}