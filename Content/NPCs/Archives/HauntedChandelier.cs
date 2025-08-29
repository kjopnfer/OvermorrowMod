using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.Particles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class HauntedChandelier : ModNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + Name;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 104;
            NPC.height = 60;
            NPC.lifeMax = 1000;
            NPC.immortal = true;
            NPC.damage = 40;
            //NPC.hide = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.ShowNameOnHover = false;
        }

        public override void DrawBehind(int index)
        {

            //Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        }

        public ref float AIState => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];

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

            //var i2 = NPC.TopLeft.X - 12;
            //var j = NPC.TopLeft.Y - 10;
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
                if (nearestPlayer != null && nearestPlayer.Center.Y > NPC.Center.Y &&
                    nearestPlayer.Center.X >= NPC.Hitbox.Left && nearestPlayer.Center.X <= NPC.Hitbox.Right)
                {
                    AIState = 1;
                    AICounter = 0;
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

                NPC.velocity.Y = currentFallSpeed;
                Point bottomCenter = new Point((int)(NPC.Center.X / 16), (int)((NPC.Bottom.Y + currentFallSpeed) / 16));
                Tile tile = Framing.GetTileSafely(bottomCenter.X, bottomCenter.Y);
                if (tile.HasTile && Main.tileSolid[tile.TileType])
                {
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
                }
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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
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