using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Weapons
{
    public class PatronusStar : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        /// <summary>
        /// Maximum speed that the star moves.
        /// </summary>
        private float driftSpeed = 0.25f;

        /// <summary>
        /// Distance that the stars detect eah other.
        /// </summary>
        private float separationDistance = ModUtils.TilesToPixels(6);

        /// <summary>
        /// The strength in which the stars drift away from each other when close.
        /// Higher values create stronger push.
        /// </summary>
        private float separationForce = 5f;

        /// <summary>
        /// How strongly the stars are pulled towards enemies. Reducing gives more influence to separation.
        /// </summary>
        private float enemyAttractionForce = 0.15f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.timeLeft = ModUtils.SecondsToTicks(10);
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = ModUtils.SecondsToTicks(1);
            Projectile.DamageType = DamageClass.Ranged;
        }

        Color starColor = Color.White;
        float starScale = 0.1f;
        public override void OnSpawn(IEntitySource source)
        {
            starColor = Color.Lerp(Color.White, new Color(108, 108, 224), Main.rand.NextFloat(0, 1f));
            starScale = Main.rand.NextFloat(0.025f, 0.075f);
        }

        public override void AI()
        {
            NPC targetEnemy = null;
            float closestDistance = 400f;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy())
                {
                    float distance = Vector2.Distance(Projectile.Center, npc.Center);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        targetEnemy = npc;
                    }
                }
            }

            if (targetEnemy != null)
            {
                Vector2 toEnemy = targetEnemy.Center - Projectile.Center;
                if (toEnemy.Length() > 0)
                {
                    toEnemy.Normalize();
                    Projectile.velocity += toEnemy * enemyAttractionForce;
                }
            }
            else
            {
                // Floating behavior when no targets
                float time = Main.GameUpdateCount * 0.02f + Projectile.whoAmI * 0.5f;
                Vector2 floatDirection = new Vector2(
                    (float)Math.Sin(time) * 0.3f,
                    (float)Math.Cos(time * 0.7f) * 0.2f
                );
                Projectile.velocity += floatDirection;

                if (Main.GameUpdateCount % 60 == 0)
                {
                    Vector2 randomDrift = Main.rand.NextVector2Circular(0.5f, 0.5f);
                    Projectile.velocity += randomDrift;
                }
            }

            Vector2 separationVector = Vector2.Zero;
            int nearbyStars = 0;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];
                if (other.active && other.type == Projectile.type && other.whoAmI != Projectile.whoAmI)
                {
                    float distance = Vector2.Distance(Projectile.Center, other.Center);
                    if (distance < separationDistance && distance > 0)
                    {
                        Vector2 away = Projectile.Center - other.Center;
                        away.Normalize();
                        separationVector += away / distance;
                        nearbyStars++;
                    }
                }
            }

            if (nearbyStars > 0)
            {
                separationVector /= nearbyStars;
                Projectile.velocity += separationVector * separationForce;
            }

            // Speed limiting
            if (Projectile.velocity.Length() > driftSpeed)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= driftSpeed;
            }

            Projectile.rotation += 0.02f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_06").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_02").Value;

            float timeRatio = 1f - (float)Projectile.timeLeft / ModUtils.SecondsToTicks(6);
            float pulseFreq = 3f;
            float pulse = (float)Math.Sin(Main.GlobalTimeWrappedHourly * pulseFreq) * 0.1f + 0.9f;
            float scale = starScale * pulse * (1f - timeRatio * 0.3f);

            float opacity = Projectile.Opacity;
            if (Projectile.timeLeft < 60)
            {
                opacity *= (float)Projectile.timeLeft / 60f;
            }

            float scale2 = scale * 1.5f;
            for (int _ = 0; _ < 3; _++)
            {
                Main.spriteBatch.Draw(texture2, Projectile.Center - Main.screenPosition, null, new Color(108, 108, 224) * opacity * 0.7f, Projectile.rotation * 0.5f, texture2.Size() / 2f, scale2, SpriteEffects.None, 0);

                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, starColor * opacity, Projectile.rotation, texture.Size() / 2f, scale, SpriteEffects.None, 0);
            }

            return false;
        }
    }
}