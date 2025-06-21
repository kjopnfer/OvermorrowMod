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
            Projectile.width = Projectile.height = 16;
            Projectile.damage = 5;
            Projectile.timeLeft = ModUtils.SecondsToTicks(4);
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            // Apply gravity
            float gravity = 0.15f; // Adjust this value to change fall speed
            Projectile.velocity.Y += gravity;

            // Optional: apply slight horizontal damping to make it feel heavier
            Projectile.velocity.X *= 0.995f;

            // Spawn particle effects
            int version = Main.rand.Next(1, 4);
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "flame_0" + version, AssetRequestMode.ImmediateLoad).Value;
            float scale = 0.1f;

            Vector2 velocity = -Projectile.velocity.SafeNormalize(Vector2.UnitY) * 2f;
            //velocity = velocity.RotatedBy(Main.rand.NextFloat(MathHelper.ToRadians(-5), MathHelper.ToRadians(5)));

            var flameParticle = new Circle(texture, 0f, useSineFade: false);
            //flameParticle.fadeIn = false;
            //flameParticle.AnchorEntity = Projectile;

            if (Projectile.timeLeft % 2 == 0)
                ParticleManager.CreateParticleDirect(
                    flameParticle,
                    Projectile.Center,
                    -Vector2.UnitY,
                    Color.DarkOrange,
                    1f,
                    scale,
                    Main.rand.NextFloat(0f, MathHelper.TwoPi),
                    ParticleDrawLayer.BehindNPCs
                );

            flameParticle = new Circle(ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value, 0f, useSineFade: false);
            //flameParticle.fadeIn = false;
            ParticleManager.CreateParticleDirect(
                flameParticle,
                Projectile.Center,
                -Vector2.UnitY,
                Color.Gold * 0.1f,
                1f,
                scale: 0.45f,
                Main.rand.NextFloat(0f, MathHelper.TwoPi),
                ParticleDrawLayer.BehindNPCs
            );
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


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity.X = 0;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(BlendState.Additive);

            Texture2D circle = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value;

            Vector2 center = Projectile.Center - Main.screenPosition;
            Vector2 origin = circle.Size() / 2f;

            // Define glow layers: (scale, alpha)
            (float scale, float alpha, Color color)[] glowLayers = new (float, float, Color)[]
    {
        (0.75f, 0.15f, new Color(255, 80, 0)),      // outer - deep orange
        (0.55f, 0.25f, new Color(255, 120, 20)),    // orange
        (0.4f,  0.35f, new Color(255, 170, 40)),    // orange-yellow
        (0.25f, 0.45f, new Color(255, 220, 60)),    // bright gold
        (0.15f, 0.6f,  new Color(255, 255, 120)),   // light yellow
        (0.08f, 0.9f,  new Color(255, 255, 200))    // center - bright white-yellow
    };

            foreach (var (scale, alpha, color) in glowLayers)
            {
                Main.spriteBatch.Draw(circle, center, null, color * alpha, 0f, origin, scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

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
            Projectile.ai[0]++; // Increment internal timer
            Lighting.AddLight(Projectile.Center, 1f, 0.4f, 0.1f);

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

            if (nearestPlayer == null)
                return;

            Vector2 shootDirection = (nearestPlayer.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
            Projectile.rotation = shootDirection.ToRotation();

            if ((int)Projectile.ai[0] % 6 == 0)
            {
                float spread = MathHelper.ToRadians(5f);
                float randomAngle = Main.rand.NextFloat(-spread, spread);
                Vector2 flameVelocity = shootDirection.RotatedBy(randomAngle - MathHelper.PiOver4) * 3f;

                Projectile.NewProjectileDirect(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    flameVelocity,
                    ModContent.ProjectileType<Flame>(),
                    5,
                    0f,
                    Projectile.owner
                );
            }
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