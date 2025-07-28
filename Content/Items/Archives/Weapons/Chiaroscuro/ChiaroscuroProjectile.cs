using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Buffs;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items;
using OvermorrowMod.Core.Particles;
using ReLogic.Content;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Weapons
{
    public class ChiaroscuroProjectile : ModProjectile, IDrawAdditive, IProjectileClassification
    {
        public override string Texture => AssetDirectory.ArchiveProjectiles + Name;

        public int maxTimeLeft;
        public float animationDuration = 0.7f;

        public Player.CompositeArmStretchAmount stretch = Player.CompositeArmStretchAmount.Full;
        public Player Owner => Main.player[Projectile.owner];
        public WeaponType WeaponType => WeaponType.Rapier;
        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.timeLeft = 12;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = (int)(Projectile.timeLeft * (1f / Owner.GetTotalAttackSpeed(DamageClass.Melee)));
            maxTimeLeft = Projectile.timeLeft;

            Projectile.velocity = Vector2.Normalize(Projectile.velocity);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity = Owner.DirectionTo(Main.MouseWorld).RotatedByRandom(0.3f);

            stretch = (Player.CompositeArmStretchAmount)Main.rand.Next(4);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Color(108, 108, 224).ToVector3() * 0.5f);

            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation = 2;

            float normalizedTime = 1f - (Projectile.timeLeft / (float)maxTimeLeft);
            float progress = MathHelper.Clamp(normalizedTime / animationDuration, 0f, 1f);

            stretch = (Player.CompositeArmStretchAmount)(3 * MathF.Sin(progress * MathHelper.Pi));

            Owner.SetCompositeArmFront(true, stretch, Projectile.rotation - MathHelper.Pi);
            Owner.ChangeDir(Projectile.direction);

            Projectile.Center = Owner.MountedCenter;

            float randomScale = Main.rand.NextFloat(0.35f, 0.5f);
            Texture2D sparkTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01", AssetRequestMode.ImmediateLoad).Value;

            var hitboxHeight = 140;
            Vector2 bladeStart = Projectile.Center;
            Vector2 bladeEnd = Projectile.Bottom + new Vector2(-20, -hitboxHeight).RotatedBy(Projectile.rotation);

            for (int i = 0; i < Main.rand.Next(1, 3); i++)
            {
                //if (Main.rand.NextBool(2))
                {
                    randomScale = Main.rand.NextFloat(1f, 1.75f);
                    Vector2 RandomVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(90)) * Main.rand.Next(10, 12);
                    Color color = new Color(108, 108, 224);

                    var lightRay = new Circle(sparkTexture, ModUtils.SecondsToTicks(0.6f), true, false)
                    {
                        endColor = new Color(108, 108, 224),
                        floatUp = false,
                        AnchorEntity = Owner,
                        AnchorOffset = new Vector2(0, -20).RotatedBy(Projectile.rotation)
                    };
                    ParticleManager.CreateParticleDirect(lightRay, Projectile.Center, RandomVelocity, color * 0.5f, 0.5f, randomScale, Projectile.rotation, 0f, useAdditiveBlending: true);
                }
            }

            for (int i = 0; i < Main.rand.Next(1, 3); i++)
            {
                if (Main.rand.NextBool(3))
                {
                    randomScale = Main.rand.NextFloat(1f, 1.75f);
                    Color color = new(108, 108, 224);

                    float randomPosition = Main.rand.NextFloat(0.2f, 0.9f); // Don't spawn too close to ends
                    Vector2 sparkPosition = Vector2.Lerp(bladeStart, bladeEnd, randomPosition);

                    randomScale = Main.rand.NextFloat(0.25f, 0.35f);

                    var lightSpark = new Spark(sparkTexture, 0f, false, 0f);
                    lightSpark.endColor = new Color(108, 108, 224);
                    Vector2 randomVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(90)) * Main.rand.Next(10, 12);
                    randomVelocity = -Vector2.Normalize(Projectile.velocity) * Main.rand.Next(1, 2);
                    ParticleManager.CreateParticleDirect(lightSpark, sparkPosition, randomVelocity, color, 1f, randomScale, MathHelper.Pi, useAdditiveBlending: true);
                }
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.friendly && !target.friendly;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            var hitboxHeight = 140;
            Vector2 swordStart = Projectile.Center;
            Vector2 swordEnd = Projectile.Bottom + new Vector2(0, -hitboxHeight).RotatedBy(Projectile.rotation);

            Vector2 hitPoint = Vector2.Lerp(swordStart, swordEnd,
                MathHelper.Clamp(Vector2.Dot(target.Center - swordStart, swordEnd - swordStart) /
                Vector2.DistanceSquared(swordStart, swordEnd), 0f, 1f));

            Texture2D sparkTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01", AssetRequestMode.ImmediateLoad).Value;
            Texture2D circleTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05", AssetRequestMode.ImmediateLoad).Value;

            for (int i = 0; i < 3; i++)
            {
                float randomScale = Main.rand.NextFloat(0.15f, 0.3f);
                Vector2 RandomVelocity = -Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(9, 11);
                Color color = new Color(149, 149, 239);

                var lightSpark = new Spark(sparkTexture, 0f, false, 0f)
                {
                    endColor = new Color(108, 108, 224)
                };
                ParticleManager.CreateParticleDirect(lightSpark, hitPoint, RandomVelocity, color, 1f, randomScale, MathHelper.Pi, useAdditiveBlending: true);

                var impact = new Circle(circleTexture, ModUtils.SecondsToTicks(0.5f), false, false)
                {
                    endColor = new Color(108, 108, 224)
                };

                randomScale = Main.rand.NextFloat(0.25f, 0.45f);
                ParticleManager.CreateParticleDirect(impact, target.Center, Vector2.Zero, color, 0.5f, randomScale, MathHelper.Pi, useAdditiveBlending: true);
            }

            int shadowCount = Main.projectile.Count(p => p.active && p.type == ModContent.ProjectileType<ChiaroscuroShadow>() && p.owner == Projectile.owner);
            if (shadowCount < 3)
            {
                if (Main.rand.NextBool(4) && !Owner.HasBuff(ModContent.BuffType<Buffs.ChiaroscuroStance>()))
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Vector2.Zero, ModContent.ProjectileType<ChiaroscuroShadow>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }


            if (Owner.HasBuff(ModContent.BuffType<Buffs.ChiaroscuroStance>()))
            {
                // Find a shadow that is invisible and doesn't have an attack target
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<ChiaroscuroShadow>() &&
                        Main.projectile[i].owner == Projectile.owner)
                    {
                        if (Main.projectile[i].ModProjectile is ChiaroscuroShadow shadow && shadow.AIState == (int)ChiaroscuroShadow.AIStates.Invisible && shadow.AttackTarget == null)
                        {
                            shadow.SetAttackTarget(target);
                            //break; // Only set one shadow to attack
                        }
                    }
                }
            }

            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox)) return true;

            var hitboxHeight = 140;
            var hitboxWidth = 18;
            float _ = float.NaN;
            Vector2 endPosition = Projectile.Bottom + new Vector2(0, -hitboxHeight).RotatedBy(Projectile.rotation);

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, endPosition, hitboxWidth * Projectile.scale, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float normalizedTime = 1f - (Projectile.timeLeft / (float)maxTimeLeft);
            float progress = MathHelper.Clamp(normalizedTime / animationDuration, 0f, 1f);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            float swordOffset = MathHelper.Lerp(-50, 10, EasingUtils.EaseOutCirc(progress));
            Vector2 off = new Vector2(swordOffset, -20).RotatedBy(Projectile.rotation - MathHelper.PiOver2);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY) + off, null, Color.White, Projectile.rotation - MathHelper.PiOver2, Vector2.Zero, Projectile.scale, 0, 0);

            /* Color color = new Color(108, 108, 224);
             Main.spriteBatch.Reload(BlendState.Additive);
             Texture2D effect = ModContent.Request<Texture2D>(AssetDirectory.Textures + "slash_01").Value;

             float stabOffset = MathHelper.Lerp(-50, 30, EasingUtils.EaseOutCirc(progress));
             float slashAlpha = normalizedTime > animationDuration ? MathHelper.Lerp(1f, 0f, (normalizedTime - animationDuration) / (1f - animationDuration)) : 1f;

             off = new Vector2(-200 + stabOffset, 12).RotatedBy(Projectile.rotation - MathHelper.PiOver2);
             Main.spriteBatch.Draw(effect, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY) + off, null, color * slashAlpha, Projectile.rotation + MathHelper.Pi, Vector2.Zero, new Vector2(0.05f, 0.8f), 0, 0);
             Main.spriteBatch.End();
             Main.spriteBatch.Begin(default, default, Main.DefaultSamplerState, default, Main.Rasterizer, default, Main.GameViewMatrix.TransformationMatrix);
            */
            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            float normalizedTime = 1f - (Projectile.timeLeft / (float)maxTimeLeft);
            float progress = MathHelper.Clamp(normalizedTime / animationDuration, 0f, 1f);

            //Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            float swordOffset = MathHelper.Lerp(-50, 10, EasingUtils.EaseOutCirc(progress));
            Vector2 off = new Vector2(swordOffset, -20).RotatedBy(Projectile.rotation - MathHelper.PiOver2);

            Texture2D effect = ModContent.Request<Texture2D>(AssetDirectory.Textures + "slash_01").Value;
            Color color = new(108, 108, 224);

            float stabOffset = MathHelper.Lerp(-50, 30, EasingUtils.EaseOutCirc(progress));
            float slashAlpha = normalizedTime > animationDuration ? MathHelper.Lerp(1f, 0f, (normalizedTime - animationDuration) / (1f - animationDuration)) : 1f;

            off = new Vector2(-200 + stabOffset, 12).RotatedBy(Projectile.rotation - MathHelper.PiOver2);
            spriteBatch.Draw(effect, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY) + off, null, color * slashAlpha, Projectile.rotation + MathHelper.Pi, Vector2.Zero, new Vector2(0.05f, 0.8f), 0, 0);
        }
    }
}