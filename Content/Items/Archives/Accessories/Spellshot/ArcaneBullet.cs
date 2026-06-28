using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class ArcaneBullet : ModProjectile, ITrailEntity
    {
        public override string Texture => AssetDirectory.Empty;

        private static readonly Color CorePurple = new Color(200, 120, 255);
        private static readonly Color EdgePink = new Color(255, 120, 220);

        private bool exploded;

        public IEnumerable<TrailConfig> TrailConfigurations()
        {
            return new List<TrailConfig>
            {
                new TrailConfig(typeof(LaserTrail), progress => EdgePink * MathHelper.SmoothStep(0, 1, progress), progress => 12f, null),
                new TrailConfig(typeof(LaserTrail), progress => Color.White * MathHelper.SmoothStep(0, 1, progress), progress => 5f, null)
            };
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, CorePurple.ToVector3());
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Detonate();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Detonate();
        }

        private void Detonate()
        {
            if (exploded) return;
            exploded = true;

            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<ArcaneBurst>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            Projectile.Kill();
        }
    }
}
