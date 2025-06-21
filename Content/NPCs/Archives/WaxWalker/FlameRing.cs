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
            Projectile.timeLeft = ModUtils.SecondsToTicks(10);
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1f, 0.4f, 0.1f);

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
            flameParticle.endColor = Color.Red;
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

            var glowParticle = new Circle(ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value, 0f, useSineFade: false);
            //flameParticle.fadeIn = false;
            ParticleManager.CreateParticleDirect(
                glowParticle,
                Projectile.Center,
                -Vector2.UnitY,
                Color.Red * 0.15f,
                1f,
                scale: 0.45f,
                Main.rand.NextFloat(0f, MathHelper.TwoPi),
                ParticleDrawLayer.BehindNPCs
            );
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, ModUtils.SecondsToTicks(5));
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, ModUtils.SecondsToTicks(5));
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

    public class HomingFlame : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.width = Projectile.height = 16;
            Projectile.damage = 5;
            Projectile.timeLeft = ModUtils.SecondsToTicks(12);
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            // Spawn particle effects
            int version = Main.rand.Next(1, 4);
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "flame_0" + version, AssetRequestMode.ImmediateLoad).Value;
            float scale = 0.1f;

            Vector2 velocity = -Projectile.velocity * 2f;
            //velocity = velocity.RotatedBy(Main.rand.NextFloat(MathHelper.ToRadians(-5), MathHelper.ToRadians(5)));

            var flameParticle = new Circle(texture, 0f, useSineFade: false);
            flameParticle.endColor = new Color(202, 188, 255);
            flameParticle.floatUp = false;
            //flameParticle.fadeIn = false;
            //flameParticle.AnchorEntity = Projectile;

            if (Projectile.timeLeft % 2 == 0)
                ParticleManager.CreateParticleDirect(
                    flameParticle,
                    Projectile.Center,
                    -Vector2.UnitY,
                    new Color(108, 108, 224),
                    1f,
                    scale,
                    Main.rand.NextFloat(0f, MathHelper.TwoPi),
                    ParticleDrawLayer.BehindNPCs
                );

            var glowParticle = new Circle(ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value, 0f, useSineFade: false);
            //flameParticle.fadeIn = false;
            glowParticle.floatUp = false;
            ParticleManager.CreateParticleDirect(
                glowParticle,
                Projectile.Center,
                -Vector2.UnitY,
                new Color(108, 108, 224) * 0.2f,
                1f,
                scale: 0.45f,
                Main.rand.NextFloat(0f, MathHelper.TwoPi),
                ParticleDrawLayer.BehindNPCs
            );

            float homingSpeed = 0.15f;      // How sharply it turns
            float maxSpeed = 3.5f;            // Max velocity magnitude

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
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, ModUtils.SecondsToTicks(5));
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, ModUtils.SecondsToTicks(5));
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

            (float scale, float alpha, Color color)[] glowLayers = new (float, float, Color)[]
            {
                (0.75f, 0.15f, new Color(100, 80, 140)),   // muted violet
                (0.55f, 0.25f, new Color(140, 110, 180)),  // dusty purple
                (0.4f,  0.35f, new Color(170, 140, 210)),  // faded lavender
                (0.25f, 0.45f, new Color(200, 170, 230)),  // light pastel purple
                (0.15f, 0.6f,  new Color(225, 200, 240)),  // very pale lilac
                (0.08f, 0.9f,  new Color(245, 230, 250))   // soft white-lavender center
            };

            foreach (var (scale, alpha, color) in glowLayers)
            {
                Main.spriteBatch.Draw(circle, center, null, color * alpha, 0f, origin, scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            return base.PreDraw(ref lightColor);
        }
    }

    public class FlameOrbiter : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.width = Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.timeLeft = ModUtils.SecondsToTicks(8);
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.damage = 5;

            if (!Main.projectile.IndexInRange((int)Projectile.ai[1]))
            {
                Projectile.Kill();
                return;
            }

            Projectile parent = Main.projectile[(int)Projectile.ai[1]];
            if (!parent.active || parent.type != ModContent.ProjectileType<FlameRing>())
            {
                Projectile.Kill();
                return;
            }

            int index = (int)Projectile.ai[0]; // orbital index
            Projectile.ai[2]++;

            int delay = ModUtils.SecondsToTicks(0.25f);
            float progress = Utils.Clamp((Projectile.ai[2] - delay) / ModUtils.SecondsToTicks(2), 0f, 1f);
            float orbitRadius = 2;
            if (Projectile.ai[2] > delay)
                orbitRadius = MathHelper.Lerp(2f, 72f, progress);

            float orbitSpeed = 0.03f;
            float angle = Main.GameUpdateCount * orbitSpeed + index * MathHelper.TwoPi / 3f;

            Vector2 offset = orbitRadius * angle.ToRotationVector2();
            Projectile.Center = parent.Center + offset;

            Lighting.AddLight(Projectile.Center, 1f, 0.4f, 0.1f);

            // Particle effect
            int version = Main.rand.Next(1, 4);
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "flame_0" + version, AssetRequestMode.ImmediateLoad).Value;
            float scale = 0.1f;

            var flameParticle = new Circle(texture, 0f, useSineFade: false)
            {
                endColor = Color.Red,
                floatUp = false
            };

            if (Projectile.timeLeft % 2 == 0)
            {
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
            }

            var glowParticle = new Circle(ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value, 0f, useSineFade: false);
            glowParticle.floatUp = false;
            ParticleManager.CreateParticleDirect(
                glowParticle,
                Projectile.Center,
                -Vector2.UnitY,
                Color.Red * 0.15f,
                1f,
                scale: 0.45f,
                Main.rand.NextFloat(0f, MathHelper.TwoPi),
                ParticleDrawLayer.BehindNPCs
            );
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, ModUtils.SecondsToTicks(5));
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, ModUtils.SecondsToTicks(5));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(BlendState.Additive);

            Texture2D circle = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value;
            Vector2 center = Projectile.Center - Main.screenPosition;
            Vector2 origin = circle.Size() / 2f;

            (float scale, float alpha, Color color)[] glowLayers = new (float, float, Color)[]
            {
            (0.75f, 0.15f, new Color(255, 80, 0)),
            (0.55f, 0.25f, new Color(255, 120, 20)),
            (0.4f,  0.35f, new Color(255, 170, 40)),
            (0.25f, 0.45f, new Color(255, 220, 60)),
            (0.15f, 0.6f,  new Color(255, 255, 120)),
            (0.08f, 0.9f,  new Color(255, 255, 200))
            };

            foreach (var (scale, alpha, color) in glowLayers)
            {
                Main.spriteBatch.Draw(circle, center, null, color * alpha, 0f, origin, scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
    }

    public class FlameRing : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        private const int FlameCount = 6;

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = ModUtils.SecondsToTicks(8);
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

            Projectile.rotation += 0.01f;
        }

        private void SpawnOrbitingFlames()
        {
            for (int i = 0; i < 3; i++)
            {
                int index = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<FlameOrbiter>(),
                    Projectile.damage,
                    0f,
                    Projectile.owner,
                    ai0: i,
                    ai1: Projectile.whoAmI
                );

                if (Main.projectile.IndexInRange(index))
                    Main.projectile[index].netUpdate = true;
            }
        }
    }
}