using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Items.Daggers;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Weapons
{
    public class CarvingKnifeNew : ModDagger<CarvingKnifeSlashNew, CarvingKnifeThrownNew>
    {
        public override string Texture => AssetDirectory.ArchiveItems + "CarvingKnife";

        // Override default settings from the base template
        protected override int MaxComboCount => 3;
        protected override bool HasCrossSlash => true;
        protected override int MaxThrownDaggers => 2;

        public override void SafeSetDefaults()
        {
            Item.damage = 13;
            Item.knockBack = 2;
            Item.useAnimation = 8;
            Item.useTime = 8;
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 10f;
        }
    }

    public class CarvingKnifeSlashNew : HeldDagger
    {
        public override string Texture => AssetDirectory.ArchiveItems + "CarvingKnife";

        // Customize the slash behavior
        public override Color SlashColor => Color.Orange;
        public override int SlashDuration => 20;
        public override float SlashRange => 80f;

        protected override void OnDaggerHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Add custom hit effects or behaviors here
            // Example: Apply debuffs, special damage calculations, etc.
        }

        protected override void CreateSlashHitEffects(Vector2 hitPosition)
        {
            // Default CarvingKnife slash particle effects
            Texture2D sparkTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01").Value;

            for (int i = 0; i < 6; i++)
            {
                float randomScale = Main.rand.NextFloat(0.1f, 0.25f);
                Vector2 particleVelocity = Main.rand.NextVector2Circular(8f, 8f);

                var spark = new Spark(sparkTexture, maxTime: 20, false, 0f)
                {
                    endColor = SlashColor
                };

                ParticleManager.CreateParticleDirect(spark, hitPosition, particleVelocity, SlashColor, 1f, randomScale, 0f, ParticleDrawLayer.BehindProjectiles, useAdditiveBlending: true);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Custom drawing logic for the slash effect
            // You can implement visual slash effects here
            return false; // Return false to prevent default drawing
        }
    }

    public class CarvingKnifeThrownNew : ThrownDagger
    {
        public override string Texture => AssetDirectory.ArchiveItems + "CarvingKnife";

        // Customize throwing behavior
        public override Color IdleColor => Color.White;
        public override Color TrailColor => Color.Orange;
        public override bool CanImpale => true;
        public override SoundStyle? HitSound => SoundID.Dig;

        // Customize animation timings
        protected override float BaseBackTime => 15f;
        protected override float BaseForwardTime => 4f;
        protected override float BaseHoldTime => 4f;

        protected override void OnDaggerHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Custom hit behavior
            // Example: Special effects, damage over time, etc.
        }

        protected override void CreateThrownHitEffects(Vector2 strikePoint)
        {
            // Default CarvingKnife thrown hit particle effects
            Texture2D sparkTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01").Value;

            Vector2 knifeDirection = Vector2.Normalize(Projectile.velocity);
            Vector2 oppositeDirection = -knifeDirection;
            float baseAngle = oppositeDirection.ToRotation();
            float spreadAngle = MathHelper.ToRadians(135);

            for (int i = 0; i < 8; i++)
            {
                float randomScale = Main.rand.NextFloat(0.1f, 0.25f);
                float randomAngleOffset = Main.rand.NextFloat(-spreadAngle * 0.5f, spreadAngle * 0.5f);
                float finalAngle = baseAngle + randomAngleOffset;

                Vector2 particleDirection = new Vector2((float)System.Math.Cos(finalAngle), (float)System.Math.Sin(finalAngle));
                Vector2 particleVelocity = particleDirection * Main.rand.Next(3, 9);

                var lightSpark = new Spark(sparkTexture, maxTime: 20, false, 0f)
                {
                    endColor = TrailColor
                };

                ParticleManager.CreateParticleDirect(lightSpark, strikePoint, particleVelocity, TrailColor, 1f, randomScale, 0f, ParticleDrawLayer.BehindProjectiles, useAdditiveBlending: true);
            }
        }

        protected override void OnDaggerImpale(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Custom impaling behavior
            // Example: Apply bleeding debuff, special visual effects
            target.AddBuff(BuffID.Bleeding, 300);
        }

        protected override void OnImpaleDamage(NPC target, int damage)
        {
            // Called every time the impaled dagger deals damage over time
            // Example: Heal player, create particles, etc.
        }
    }
}