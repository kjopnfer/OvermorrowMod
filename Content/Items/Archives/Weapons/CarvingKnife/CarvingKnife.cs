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
    public class CarvingKnife : ModDagger<CarvingKnifeSlash, CarvingKnifeThrown>
    {
        public override string Texture => AssetDirectory.ArchiveItems + "CarvingKnife";

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

    public class CarvingKnifeSlash : HeldDagger
    {
        protected override Color BaseSlashColor => Color.LightBlue;

        protected override string GetDaggerTexture()
        {
            return AssetDirectory.ArchiveItems + "CarvingKnife";
        }
    }

    public class CarvingKnifeThrown : ThrownDagger
    {
        public override string Texture => AssetDirectory.ArchiveItems + "CarvingKnife";

        public override Color IdleColor => Color.White;
        public override Color TrailColor => Color.White;
        public override bool CanImpale => true;
        public override SoundStyle? HitSound => SoundID.Dig;

        protected override float BaseBackTime => 15f;
        protected override float BaseForwardTime => 4f;
        protected override float BaseHoldTime => 4f;

        private bool hasHitNPC = false;
        protected override void OnDaggerHit(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hasHitNPC = true;
        }

        protected override void CreateThrownHitEffects(Vector2 strikePoint)
        {
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

                var lightSpark = new Spark(sparkTexture, maxTime: 20, true, 0f)
                {
                    endColor = TrailColor,
                    rotationOffset = MathHelper.PiOver2
                };

                ParticleManager.CreateParticleDirect(lightSpark, strikePoint, particleVelocity, TrailColor, 1f, randomScale, 0f, ParticleDrawLayer.BehindProjectiles, useAdditiveBlending: true);
            }
        }

        protected override void OnDaggerPickup()
        {
        }

        protected override void OnDaggerImpale(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }

        protected override void OnImpaleDamage(NPC target, int damage)
        {
        }
    }
}