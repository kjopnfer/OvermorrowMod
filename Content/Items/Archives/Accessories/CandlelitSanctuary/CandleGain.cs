
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Particles;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives
{
    public class CandleGain : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = ModUtils.SecondsToTicks(0.75f);
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);

            Player player = Main.player[Projectile.owner];
            if (!player.active) return;

            float randomScale = Main.rand.NextFloat(10f, 20f);
            float randomRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);

            Color color = new(108, 108, 224);

            Texture2D sparkTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_04", AssetRequestMode.ImmediateLoad).Value;
            for (int i = 0; i < 32; i++)
            {
                randomScale = Main.rand.NextFloat(3f, 6f);
                Vector2 randomVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(9, 12);

                var rotatingEmber = new Spark(sparkTexture, Main.rand.Next(8, 10) * 10, false, -1f)
                {
                    endColor = Color.Purple,
                    AnchorEntity = player
                };

                ParticleManager.CreateParticleDirect(rotatingEmber, Projectile.Center, Vector2.Normalize(randomVelocity), color, 1f, randomScale, 0f, useAdditiveBlending: true);
            }
        }

        public ref float AICounter => ref Projectile.ai[0];

        public override void AI()
        {
            AICounter++;
            base.AI();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}