using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs.Debuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Ranged.Ammo
{
    public class FungalArrow : ModProjectile
    {

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fungal Arrow");
        }

        public override void AI()
        {
            projectile.velocity.Y += 0.07f;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.Next(3) == 2)
            {
                target.AddBuff(ModContent.BuffType<FungalInfection>(), 280);
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Main.rand.Next(5) == 3)
            {
                Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<FungalArrowAmmo>());
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