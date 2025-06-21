using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Particles;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class Flame : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.damage = 5;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.damage = 5;

            Lighting.AddLight(Projectile.Center, 0.7f, 0.25f, 0.1f);

            int index = (int)Projectile.ai[0]; // Ring index
            int parentID = (int)Projectile.ai[1];

            Projectile parent = Main.projectile[parentID];
            if (!parent.active || parent.type != ModContent.ProjectileType<FlameRing>())
            {
                Projectile.Kill();
                return;
            }

            float orbitAngle = MathHelper.TwoPi * index / 3f + parent.ai[0]; // sync with parent spin
            Vector2 offset = new Vector2(24f, 0f).RotatedBy(orbitAngle);     // same OrbitRadius as parent
            Projectile.Center = parent.Center + offset;

            // Optional: rotate to face outward
            Projectile.rotation = offset.ToRotation() + MathHelper.PiOver2;

            // Particles
            int version = Main.rand.Next(1, 4);
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "flame_0" + version, AssetRequestMode.ImmediateLoad).Value;
            float scale = 0.1f;

            float angleOffset = MathHelper.ToRadians(45f);
            // or -45f to rotate the other way
            //Vector2 velocity = -offset.SafeNormalize(Vector2.UnitY * 5)
            //                          .RotatedBy(angleOffset) * 1.5f;
            //velocity = velocity.RotatedBy(Main.rand.NextFloat(MathHelper.ToRadians(-5), MathHelper.ToRadians(5)));

            // Rotate the emission angle by 45 degrees
            Vector2 velocity = offset.SafeNormalize(new Vector2(25, 25)) // or -offset if you want inward
                .RotatedBy(MathHelper.ToRadians(45)) * 2f;

            // Add some slight randomization (optional)
            velocity = velocity.RotatedBy(Main.rand.NextFloat(MathHelper.ToRadians(-5), MathHelper.ToRadians(5)));

            var flameParticle = new Circle(texture, 0f, useSineFade: true);
            //flameParticle.AnchorEntity = Projectile;

            ParticleManager.CreateParticleDirect(flameParticle, Projectile.Center, Vector2.Zero, Color.DarkOrange, 1f, scale, Main.rand.NextFloat(0f, MathHelper.TwoPi), ParticleDrawLayer.BehindNPCs);
            return;
            /*Lighting.AddLight(Projectile.Center, 0.7f, 0.25f, 0.1f);

            float homingSpeed = 0.15f;      // How sharply it turns
            float maxSpeed = 2.5f;            // Max velocity magnitude

            Player nearestPlayer = null;
            float closestDist = float.MaxValue;

            foreach (Player player in Main.player)
            {
                if (!player.active || player.dead)
                    continue;

                float dist = Vector2.Distance(Projectile.Center, player.Center);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    nearestPlayer = player;
                }
            }

            if (nearestPlayer != null)
            {
                Vector2 desiredVelocity = (nearestPlayer.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * maxSpeed;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, homingSpeed);
            }

            int version = Main.rand.Next(1, 4);
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "flame_0" + version, AssetRequestMode.ImmediateLoad).Value;
            float scale = 0.1f;

            Vector2 velocity = -Vector2.UnitY * 2;
            //Vector2 velocity = -Projectile.velocity;

            var flameParticle = new Circle(texture, 0f, useSineFade: true);
            velocity = velocity.RotatedBy(Main.rand.NextFloat(MathHelper.ToRadians(-5), MathHelper.ToRadians(5)));
            flameParticle.AnchorEntity = Projectile;

            // Optional offset (above projectile center)
            //particle.AnchorOffset = new Vector2(0f, -10f);
            ParticleManager.CreateParticleDirect(flameParticle, Projectile.Center, velocity, Color.DarkOrange, 1f, scale, Main.rand.NextFloat(0f, MathHelper.TwoPi), ParticleDrawLayer.BehindNPCs);

            base.AI();*/
        }

        public override bool PreDraw(ref Color lightColor)
        {
           
            return base.PreDraw(ref lightColor);
        }
    }

    public class FlameRing : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        private const int FlameCount = 3;
        private const float OrbitRadius = 24f;
        private const float RotationSpeed = 0.05f;

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                SpawnOrbitingFlames();
                Projectile.localAI[0] = 1f;
            }

            // Spin around
            Projectile.ai[0] += RotationSpeed;

            Lighting.AddLight(Projectile.Center, 0.7f, 0.25f, 0.1f);
        }

        private void SpawnOrbitingFlames()
        {
            for (int i = 0; i < FlameCount; i++)
            {
                float angle = MathHelper.TwoPi * i / FlameCount;

                int index = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero, // velocity will be handled in flame
                    ModContent.ProjectileType<Flame>(),
                    Projectile.damage,
                    0f,
                    Main.myPlayer,
                    ai0: i,                // store index in ring
                    ai1: Projectile.whoAmI // store parent ID
                );

                if (Main.projectile.IndexInRange(index))
                {
                    Main.projectile[index].netUpdate = true;
                }
            }
        }
    }
}