using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Buffs.Debuffs;

namespace OvermorrowMod.Projectiles.Ranged
{
    public class PoisonArrow : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.Stinger;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("MushAxeBack");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.Stinger);
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 1;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
        }


        public override void AI()
        {
            projectile.velocity.Y += 0.16f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.Next(6) == 3)
            {
                target.AddBuff(20, 240);
            }
        }
        
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Makes dust projectiled on tile
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Item10, projectile.position);
            return true;
        }
    }
}