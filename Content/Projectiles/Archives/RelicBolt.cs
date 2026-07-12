using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Projectiles.Archives
{
    public class RelicBolt : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        private readonly List<Vector2> trail = new();
        private float orbitDir = 1f;
        private float orbitTilt;
        private Color colorCenter = Color.White;
        private Color colorOuter = Color.White;
        private Color colorTailA = Color.White;
        private Color colorTailB = Color.White;
        private Color colorTailC = Color.White;
        private float speedMult = 1f;
        private bool launched;
        private Vector2 launchTarget;
        private int launchTimer;
        private Vector2 lastMovement;
        private int paletteIndex;

        private static int spawnCounter;

        public bool IsLaunched => launched;

        private Player Owner => Main.player[Projectile.owner];
        private ref float Phase => ref Projectile.ai[0];
        private ref float Counter => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.timeLeft = ModUtils.SecondsToTicks(8);
        }

        public override void OnSpawn(IEntitySource source)
        {
            orbitDir = Main.rand.NextBool() ? 1f : -1f;
            speedMult = Main.rand.NextFloat(0.6f, 1.5f);

            int id = spawnCounter++;
            Projectile.localAI[1] = id;
            paletteIndex = id % 2;

            if (id % 2 == 0)
            {
                colorCenter = new Color(239, 245, 255);
                colorOuter = new Color(26, 117, 255);
                colorTailA = new Color(31, 44, 255);
                colorTailB = new Color(26, 117, 255);
                colorTailC = new Color(13, 7, 153);
            }
            else
            {
                colorCenter = new Color(254, 254, 255);
                colorOuter = new Color(189, 74, 210);
                colorTailA = new Color(104, 5, 164);
                colorTailB = new Color(204, 47, 123);
                colorTailC = new Color(255, 82, 119);
            }

            EnforceLimit();
        }

        private void EnforceLimit()
        {
            const int max = 5;
            int count = 0;
            Projectile oldest = null;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.type == Type && p.owner == Projectile.owner && p.ModProjectile is RelicBolt rb && !rb.launched)
                {
                    count++;
                    if (oldest == null || p.localAI[1] < oldest.localAI[1]) oldest = p;
                }
            }

            if (count > max) oldest?.Kill();
        }

        public void Launch(Vector2 target, int damage)
        {
            launched = true;
            launchTarget = target;
            Projectile.damage = damage;
            Projectile.friendly = false;
            Projectile.hide = false;
            launchTimer = 100;

            Vector2 fallback = (target - Projectile.Center).SafeNormalize(Vector2.UnitX);
            Projectile.velocity = lastMovement.SafeNormalize(fallback) * 16f;
        }

        public override void AI()
        {
            Player player = Owner;
            if (!player.active || player.dead)
            {
                Projectile.Kill();
                return;
            }

            if (launched)
            {
                LaunchAI();
                return;
            }

            Projectile.timeLeft = 2;

            Projectile.hide = true;

            float radiusX = 70f;
            float radiusY = 24f;
            float orbitSpeed = 0.1f * speedMult;
            int trailLength = 26;

            int total = 0, rank = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.type == Type && p.owner == Projectile.owner && p.ModProjectile is RelicBolt rb && !rb.launched)
                {
                    total++;
                    if (p.localAI[1] < Projectile.localAI[1]) rank++;
                }
            }
            float targetTilt = total > 0 ? (rank + 0.5f) * MathHelper.Pi / total : 0f;
            orbitTilt = MathHelper.Lerp(orbitTilt, targetTilt, 0.12f);

            Counter += 1f;
            float theta = Phase + orbitDir * Counter * orbitSpeed;
            float depth = (float)Math.Cos(theta);
            Vector2 offset = new Vector2((float)Math.Sin(theta) * radiusX, depth * radiusY).RotatedBy(orbitTilt);

            Vector2 last = Projectile.Center;
            Vector2 target = player.MountedCenter + offset;
            Projectile.Center = target;
            Projectile.velocity = Vector2.Zero;
            lastMovement = target - last;
            Projectile.rotation = lastMovement.ToRotation();

            Projectile.localAI[0] = depth;
            Projectile.scale = MathHelper.Lerp(0.1625f, 0.2875f, (depth + 1f) / 2f);

            trail.Insert(0, Projectile.Center);
            while (trail.Count > trailLength) trail.RemoveAt(trail.Count - 1);

            Lighting.AddLight(Projectile.Center, 0.3f, 0.4f, 0.55f);
        }

        private void LaunchAI()
        {
            Projectile.timeLeft = 10;
            Projectile.scale = 0.3f;

            Vector2 toTarget = launchTarget - Projectile.Center;
            float dist = toTarget.Length();

            float closeness = MathHelper.Clamp(1f - dist / 160f, 0f, 1f);
            float turnRate = MathHelper.Lerp(0.14f, 0.9f, closeness);
            float speed = 16f;

            if (dist <= speed || --launchTimer <= 0)
            {
                Detonate();
                return;
            }

            float newAngle = Projectile.velocity.ToRotation().AngleLerp(toTarget.ToRotation(), turnRate);
            Projectile.velocity = newAngle.ToRotationVector2() * speed;
            Projectile.rotation = Projectile.velocity.ToRotation();

            trail.Insert(0, Projectile.Center);
            while (trail.Count > 26) trail.RemoveAt(trail.Count - 1);

            Lighting.AddLight(Projectile.Center, colorOuter.ToVector3() * 0.6f);
        }

        private void Detonate()
        {
            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), launchTarget, Vector2.Zero, ModContent.ProjectileType<RelicExplosion>(), Projectile.damage, 0f, Projectile.owner, 0f, paletteIndex);

            Projectile.Kill();
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (launched) return;
            if (Projectile.localAI[0] < 0f)
                behindProjectiles.Add(index);
            else
                overPlayers.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D blob = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value;
            Vector2 origin = blob.Size() / 2f;
            float s = Projectile.scale;

            Main.spriteBatch.Reload(BlendState.Additive);

            for (int i = trail.Count - 1; i >= 0; i--)
            {
                float frac = i / (float)trail.Count;
                Color color = DrawUtils.ColorLerp3(colorTailA, colorTailB, colorTailC, frac) * (1f - frac) * 0.6f;
                float scale = MathHelper.Lerp(0.45f, 0.05f, frac) * s;
                Main.spriteBatch.Draw(blob, trail[i] - Main.screenPosition, null, color, 0f, origin, scale, SpriteEffects.None, 0f);
            }

            Vector2 center = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(blob, center, null, colorOuter * 0.7f, 0f, origin, 0.55f * s, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(blob, center, null, colorCenter, 0f, origin, 0.28f * s, SpriteEffects.None, 0f);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
    }
}
