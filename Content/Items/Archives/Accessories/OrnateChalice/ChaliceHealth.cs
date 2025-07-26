using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Particles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class ChaliceHealth : ModProjectile
    {
        public override string Texture => AssetDirectory.ArchiveProjectiles + Name;
        public override void SetStaticDefaults()
        {
            Projectile.width = Projectile.height = 28;
            Projectile.timeLeft = ModUtils.SecondsToTicks(5);
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

        private float baseScale;
        int homingDelay = 12;
        public override void OnSpawn(IEntitySource source)
        {
            //baseScale = Main.rand.NextFloat(f, 2f);
            baseScale = Main.rand.NextFloat(0.2f, 0.6f);
            Projectile.scale = baseScale;
        }

        public ref float AICounter => ref Projectile.ai[0];

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active)
                Projectile.Kill();

            Projectile.tileCollide = false;
            if (AICounter++ > homingDelay)
            {
                Vector2 toPlayer = player.Center - Projectile.Center;
                Vector2 directionToPlayer = Vector2.Normalize(toPlayer);

                // Turn resistance
                Vector2 currentDirection = Vector2.Normalize(Projectile.velocity);

                // Reduce turn resistance when closer to player
                float distanceToPlayer = toPlayer.Length();
                float baseTurnRate = 0.1f;
                float closeTurnRate = 0.3f;
                float closeDistance = 200f; // Distance at which it becomes more responsive

                // Lerp rates based on distance
                float turnRate = MathHelper.Lerp( 0.3f, baseTurnRate, Math.Min(distanceToPlayer / closeDistance, 1f));

                Vector2 newDirection = Vector2.Lerp(currentDirection, directionToPlayer, turnRate);

                float baseSpeed = 4f;
                float timeActive = AICounter - homingDelay;
                float speedIncrease = timeActive * 0.05f; // Increase by 0.05 per tick
                float speed = baseSpeed + speedIncrease;

                speed = Math.Min(speed, 12f);

                Projectile.velocity = newDirection * speed;

                if (Projectile.Hitbox.Intersects(player.Hitbox))
                    Projectile.Kill();
            }

            DrawParticles();
            Projectile.rotation -= 0.02f;
        }

        public virtual void DrawParticles()
        {
            float scale = Main.rand.NextFloat(0.2f, 0.5f);
            var outlineParticle = new OutlineParticle(AssetDirectory.ArchiveProjectiles + Name, 16, 16)
            {
                ShouldDrawOutline = true,
                OutlineColor = new Color(66, 57, 44),
                FillColor = new Color(177, 18, 24),
                MaxLifetime = ModUtils.SecondsToTicks(1)
            };
            ParticleManager.CreateParticleDirect(outlineParticle, Projectile.Center, -Vector2.Normalize(Projectile.velocity), Color.White, 1f, scale);

            outlineParticle = new OutlineParticle(AssetDirectory.ArchiveProjectiles + Name, 16, 16)
            {
                ShouldDrawOutline = true,
                OutlineColor = new Color(220, 20, 26),
                FillColor = new Color(255, 107, 114),
                MaxLifetime = ModUtils.SecondsToTicks(1)
            };
            ParticleManager.CreateParticleDirect(outlineParticle, Projectile.Center, -Vector2.Normalize(Projectile.velocity), Color.White, 1f, scale * 0.4f);

        }

        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }
    }

    public class ChaliceMana : ChaliceHealth
    {
        public override string Texture => AssetDirectory.ArchiveProjectiles + "ChaliceHealth";

        public override void DrawParticles()
        {
            float scale = Main.rand.NextFloat(0.2f, 0.5f);
            var outlineParticle = new OutlineParticle(AssetDirectory.ArchiveProjectiles + "ChaliceHealth", 16, 16)
            {
                ShouldDrawOutline = true,
                OutlineColor = new Color(11, 17, 62),
                FillColor = new Color(62, 85, 246),
                MaxLifetime = ModUtils.SecondsToTicks(1)
            };
            ParticleManager.CreateParticleDirect(outlineParticle, Projectile.Center, -Vector2.Normalize(Projectile.velocity), Color.White, 1f, scale);

            var innerOutlineParticle = new OutlineParticle(AssetDirectory.ArchiveProjectiles + "ChaliceHealth", 16, 16)
            {
                ShouldDrawOutline = true,
                OutlineColor = new Color(105, 136, 255),
                FillColor = new Color(180, 207, 255),
                MaxLifetime = ModUtils.SecondsToTicks(1)
            };
            ParticleManager.CreateParticleDirect(innerOutlineParticle, Projectile.Center, -Vector2.Normalize(Projectile.velocity), Color.White, 1f, scale * 0.4f);
        }
    }
}