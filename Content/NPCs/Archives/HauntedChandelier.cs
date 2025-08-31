using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.NPCs;
using OvermorrowMod.Core.Particles;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class HauntedChandelier : OvermorrowNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + Name;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SafeSetDefaults()
        {
            NPC.width = 104;
            NPC.height = 60;
            NPC.lifeMax = 1000;
            NPC.immortal = true;
            NPC.damage = 75;
            //NPC.hide = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.ShowNameOnHover = false;
        }

        public override NPCTargetingConfig TargetingConfig()
        {
            NPCTargetingConfig config = new NPCTargetingConfig()
            {
                DisplayAggroIndicator = false,
                AlertRange = null
            };

            return config;
        }

        public override void DrawBehind(int index)
        {

            //Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        }

        public ref float AIState => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];
        public ref float Cooldown => ref NPC.ai[2];


        private Vector2 originalPosition;
        public override void OnSpawn(IEntitySource source)
        {
            originalPosition = NPC.Center;
        }

        private float maxFallSpeed = 16f;
        private float gravity = 0.8f;
        private float retractSpeed = 4f;
        private float maxLeanAngle = MathHelper.ToRadians(15f);
        private float currentFallSpeed = 0f;
        private float targetRotation = 0f;

        private int detectionRange = 200;
        public override bool CanHitNPC(NPC target)
        {
            return AIState != 1 || AIState != 4;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return AIState != 1 || AIState != 4;
        }

        public override void AI()
        {
            NPC.ShowNameOnHover = false;
            AICounter++;

            Player nearestPlayer = null;
            float closestDistance = float.MaxValue;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead)
                {
                    float distance = Vector2.Distance(player.Center, NPC.Center);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        nearestPlayer = player;
                    }
                }
            }

            float scale = 0.1f;
            Vector2 velocity = -Vector2.UnitY * 0.5f;

            int interpolationSteps = Math.Max(1, (int)Math.Abs(NPC.velocity.Y));
            Vector2 stepVelocity = interpolationSteps > 1 ? NPC.velocity / interpolationSteps : Vector2.Zero;

            for (int step = 0; step <= interpolationSteps; step++)
            {
                Vector2 interpolatedPosition = NPC.position + (stepVelocity * step);
                if (AIState == 4)
                    interpolatedPosition = NPC.position - (stepVelocity * step);

                var i2 = interpolatedPosition.X - 12;
                var j = interpolatedPosition.Y - 10;

                float yOffset = (AIState == 2) ? 3.2f : 2.45f;
                CreateEmberParticle(new Vector2(i2 + 1.15f * 16, j + yOffset * 16), velocity, scale);
                yOffset = (AIState == 2) ? 3.0f : 2.25f;
                CreateEmberParticle(new Vector2(i2 + 2.35f * 16, j + yOffset * 16), velocity, scale);
                yOffset = (AIState == 2) ? 3.25f : 2.5f;
                CreateEmberParticle(new Vector2(i2 + 3.0f * 16, j + yOffset * 16), velocity, scale);
                yOffset = (AIState == 2) ? 3.0f : 2.25f;
                CreateEmberParticle(new Vector2(i2 + 3.45f * 16, j + yOffset * 16), velocity, scale);

                yOffset = (AIState == 2) ? 3.0f : 2.25f;
                CreateEmberParticle(new Vector2(i2 + 4.55f * 16, j + yOffset * 16), velocity, scale);
                yOffset = (AIState == 2) ? 3.2f : 2.45f;
                CreateEmberParticle(new Vector2(i2 + 4.9f * 16, j + yOffset * 16), velocity, scale);
                yOffset = (AIState == 2) ? 2.95f : 2.2f;
                CreateEmberParticle(new Vector2(i2 + 5.7f * 16, j + yOffset * 16), velocity, scale);
                yOffset = (AIState == 2) ? 3.25f : 2.5f;
                CreateEmberParticle(new Vector2(i2 + 6.8f * 16, j + yOffset * 16), velocity, scale);
            }

            if (AIState == 0 || AIState == 4)
            {
                if (AIState == 0)
                    Lighting.AddLight(NPC.Center, 0.9f, 0.675f, 0f);
            }
            else
            {
                Lighting.AddLight(NPC.Center, new Vector3(0.1f, 0.4f, 0.7f) * 2f);
            }

            if (AIState == 0)
            {
                if (Cooldown > 0)
                {
                    Cooldown--;
                }
                else
                {
                    float leadingDistance = 48;
                    if (nearestPlayer != null && nearestPlayer.Center.Y > NPC.Center.Y &&
                        nearestPlayer.Center.X >= NPC.Hitbox.Left - leadingDistance && nearestPlayer.Center.X <= NPC.Hitbox.Right + leadingDistance)
                    {
                        AIState = 1;
                        AICounter = 0;
                    }
                }
            }
            else if (AIState == 1)
            {
                float wiggleIntensity = 2f;
                float wiggleX = (float)Math.Sin(AICounter * 0.5f) * wiggleIntensity;
                NPC.Center = originalPosition + new Vector2(wiggleX, 0);

                if (AICounter >= 15)
                {
                    AIState = 2;
                    AICounter = 0;
                }
            }
            else if (AIState == 2)
            {
                currentFallSpeed += gravity;
                if (currentFallSpeed > maxFallSpeed)
                    currentFallSpeed = maxFallSpeed;

                if (Main.rand.NextBool(2))
                {
                    float randomScale = Main.rand.NextFloat(0.025f, 0.075f);
                    Color color = new Color(149, 149, 239);
                    Texture2D sparkTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01", AssetRequestMode.ImmediateLoad).Value;

                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 randomVelocity = -Vector2.UnitY * Main.rand.Next(1, 4);

                        var lightSpark = new Spark(sparkTexture, maxTime: Main.rand.Next(60, 80), true, 0f)
                        {
                            endColor = new Color(108, 108, 224),
                            rotationOffset = MathHelper.PiOver2
                        };

                        Vector2 position = NPC.Center + new Vector2(Main.rand.Next(-3, 3) * 16, 0);
                        ParticleManager.CreateParticleDirect(lightSpark, position, randomVelocity, color, 1f, randomScale, MathHelper.PiOver2, ParticleDrawLayer.BehindProjectiles, useAdditiveBlending: true);
                    }
                }

                NPC.velocity.Y = currentFallSpeed;
                Point bottomCenter = new Point((int)(NPC.Center.X / 16), (int)((NPC.Bottom.Y + currentFallSpeed) / 16));
                Tile tile = Framing.GetTileSafely(bottomCenter.X, bottomCenter.Y);
                if (tile.HasTile && Main.tileSolid[tile.TileType])
                {
                    SpawnImpactParticles();
                    NPC.Bottom = new Vector2(NPC.Center.X, bottomCenter.Y * 16);
                    AIState = 3;
                    AICounter = 0;
                    NPC.velocity.Y = 0;
                    currentFallSpeed = 0f;
                }
            }
            else if (AIState == 3)
            {
                if (AICounter >= 120)
                {
                    AIState = 4;
                    AICounter = 0;
                }
            }
            else if (AIState == 4)
            {
                if (Vector2.Distance(NPC.Center, originalPosition) > 10f)
                {
                    Vector2 direction = Vector2.Normalize(originalPosition - NPC.Center);
                    NPC.velocity = direction * retractSpeed;
                }
                else
                {
                    NPC.Center = originalPosition;
                    NPC.velocity = Vector2.Zero;
                    NPC.rotation = 0f;
                    AIState = 0;
                    AICounter = 0;
                    Cooldown = 120f;
                }
            }
        }

        private void SpawnImpactParticles()
        {
            float randomScale = Main.rand.NextFloat(10f, 20f);
            Color color = new Color(149, 149, 239);
            Texture2D sparkTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01", AssetRequestMode.ImmediateLoad).Value;
            for (int i = 0; i < 24; i++)
            {
                randomScale = Main.rand.NextFloat(2f, 7f);
                float angle = Main.rand.NextFloat(MathHelper.ToRadians(-15), MathHelper.ToRadians(15));
                if (Main.rand.NextBool())
                    angle += MathHelper.Pi;
                Vector2 randomVelocity = Vector2.UnitX.RotatedBy(angle) * Main.rand.Next(2, 12);
                var lightSpark = new Spark(sparkTexture, maxTime: Main.rand.Next(15, 30), true, 0f)
                {
                    endColor = new Color(108, 108, 224)
                };
                ParticleManager.CreateParticleDirect(lightSpark, NPC.Bottom, randomVelocity, color, 1f, randomScale, 0f, ParticleDrawLayer.BehindProjectiles, useAdditiveBlending: true);
            }
        }

        private void CreateEmberParticle(Vector2 position, Vector2 velocity, float scale)
        {
            if (AIState == 4)
                return;

            Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.StardustTowerMark).Value;

            var emberParticle = new Circle(texture, 0f, useSineFade: true)
            {
                endColor = Color.DarkRed
            };

            velocity = velocity.RotatedBy(Main.rand.NextFloat(MathHelper.ToRadians(-5), MathHelper.ToRadians(5)));
            //velocity = velocity.RotatedBy(MathHelper.ToRadians(-5));

            Color color = Color.DarkOrange;
            if (AIState == 1 || AIState == 2 || AIState == 3)
            {
                if (AIState != 3)
                    emberParticle = new Circle(texture, 15f, useSineFade: true)
                    {
                        endColor = Color.DarkRed
                    };

                emberParticle.useSineFade = false;
                color = Color.DarkBlue;
                scale *= 1.5f;
            }

            if (AIState == 4)
            {
                emberParticle = new Circle(texture, 15f, useSineFade: true)
                {
                    endColor = Color.DarkRed
                };
            }

            ParticleManager.CreateParticleDirect(emberParticle, position, velocity, color, 1f, scale, 0f, ParticleDrawLayer.BehindProjectiles, useAdditiveBlending: true);
            ParticleManager.CreateParticleDirect(emberParticle, position, velocity, Color.White * 0.45f, 1f, scale, 0f, ParticleDrawLayer.BehindProjectiles, useAdditiveBlending: true);
        }

        public override bool DrawOvermorrowNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D chainTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "WaxheadChain").Value;

            Vector2 chainStart = originalPosition - new Vector2(0, 24) - Main.screenPosition;
            Vector2 chainEnd = NPC.Center - Main.screenPosition;
            Vector2 chainVector = chainEnd - chainStart;
            float chainLength = chainVector.Length();
            Vector2 chainDirection = Vector2.Normalize(chainVector);

            float chainScale = 0.6f;
            float scaledChainHeight = chainTexture.Height * chainScale;
            int chainSegments = (int)(chainLength / scaledChainHeight);
            for (int i = 0; i < chainSegments; i++)
            {
                Vector2 segmentPosition = chainStart + (chainDirection * scaledChainHeight * i);
                Vector2 worldPosition = segmentPosition + Main.screenPosition;
                Color segmentLighting = Lighting.GetColor((int)(worldPosition.X / 16), (int)(worldPosition.Y / 16));
                float rotation = chainVector.ToRotation() + MathHelper.PiOver2;
                spriteBatch.Draw(chainTexture, segmentPosition, null, segmentLighting * NPC.Opacity, rotation,
                    new Vector2(chainTexture.Width / 2, 0), chainScale, SpriteEffects.None, 0);
            }

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}