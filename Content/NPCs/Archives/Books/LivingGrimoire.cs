using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using Terraria.ID;
using Terraria.GameContent.Bestiary;
using Terraria.UI;
using Terraria.DataStructures;
using OvermorrowMod.Content.Buffs;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Biomes;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Microsoft.VisualBasic;
using System;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public abstract class LivingGrimoire : OvermorrowNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + Name;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CheckActive() => false;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Position = new Vector2(8, 8),
                PortraitPositionXOverride = 8,
                PortraitPositionYOverride = -6,
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 44;
            NPC.lifeMax = 100;
            NPC.defense = 8;
            NPC.damage = 12;
            NPC.knockBackResist = 0.5f;
            NPC.noGravity = true;
            NPC.value = Item.buyPrice(0, 0, silver: 2, copper: 20);

            SpawnModBiomes = [ModContent.GetInstance<GrandArchives>().Type];
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue(LocalizationPath.Bestiary + Name)),
            });
        }

        public ref float AIState => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];
        public ref Player player => ref Main.player[NPC.target];
        public enum AICase
        {
            Fall = 0,
            Fly = 1,
            Cast = 2
        }

        private int distanceFromGround = 180;
        private int aggroDelayTime = 60;
        private int tileAttackDistance = 24;
        private Vector2 targetPosition;
        public override void OnSpawn(IEntitySource source)
        {
            AIState = (int)AICase.Fly;
            distanceFromGround = Main.rand.Next(16, 19) * 8;
            aggroDelayTime = Main.rand.Next(10, 20) * 10;
            tileAttackDistance = Main.rand.Next(16, 32) * 16;

            aggroDelay = aggroDelayTime;
            targetPosition = NPC.Center + new Vector2(14 * 16 * NPC.direction, 0).RotatedByRandom(MathHelper.PiOver2);
        }

        float flySpeedX = 2;
        float flySpeedY = 0;
        float aggroDelay = 60;
        public sealed override void AI()
        {
            NPC.TargetClosest();

            //Dust.NewDust(targetPosition, 1, 1, DustID.Torch);

            switch ((AICase)AIState)
            {
                case AICase.Fall:
                    break;
                case AICase.Fly:
                    HandleHorizontalMovement();
                    HandleVerticalMovement();
                    HandleGroundProximity();
                    HandleObstacleAvoidance();

                    float xDistance = Math.Abs(NPC.Center.X - player.Center.X);

                    bool xDistanceCheck = xDistance <= tileAttackDistance * 18;
                    bool yDistanceCheck = Math.Abs(NPC.Center.Y - player.Center.Y) < 100;

                    if (xDistanceCheck && yDistanceCheck && Collision.CanHitLine(player.Center, 1, 1, NPC.Center, 1, 1))
                    {
                        aggroDelay--;
                        if (aggroDelay <= 0)
                        {
                            AIState = (int)AICase.Cast;
                            AICounter = 0;
                        }
                    }

                    if (AICounter++ > 120)
                    {
                        AIState = (int)AICase.Fly;
                        AICounter = 0;
                        targetPosition = NPC.Center + new Vector2(26 * 16 * NPC.direction, 0).RotatedByRandom(MathHelper.PiOver4);
                    }
                    break;
                case AICase.Cast:
                    NPC.velocity.X /= 2f;

                    HandleVerticalMovement();
                    HandleGroundProximity();

                    CastSpell();

                    if (AICounter++ > 120)
                    {
                        distanceFromGround = Main.rand.Next(16, 19) * 8;
                        aggroDelayTime = Main.rand.Next(10, 20) * 10;
                        tileAttackDistance = Main.rand.Next(16, 32) * 16;

                        AIState = (int)AICase.Fly;
                        AICounter = 0;
                        aggroDelay = aggroDelayTime;
                    }
                    break;
            }

            NPC.rotation = NPC.velocity.Y * 0.08f;

            base.AI();
        }

        public virtual void CastSpell() { }

        private void HandleHorizontalMovement()
        {
            float targetSpeed = 2f;

            if (NPC.Center.X >= targetPosition.X)
            {
                NPC.velocity.X = Math.Max(NPC.velocity.X - 0.05f, -targetSpeed);
                flySpeedX = Math.Max(flySpeedX - 0.1f, -targetSpeed);
            }
            else if (NPC.Center.X <= targetPosition.X)
            {
                NPC.velocity.X = Math.Min(NPC.velocity.X + 0.05f, targetSpeed);
                flySpeedX = Math.Min(flySpeedX + 0.1f, targetSpeed);
            }
        }

        private void HandleVerticalMovement()
        {
            float verticalBuffer = 16 * 5;
            float targetSpeed = 2f;

            if (NPC.Center.Y <= player.Center.Y - verticalBuffer)
            {
                NPC.velocity.Y = Math.Min(NPC.velocity.Y + 0.1f, targetSpeed);

                // Add randomness to avoid straight-line movement
                if (Main.rand.NextBool(3))
                    NPC.velocity.Y += 0.05f;

                flySpeedY = Math.Min(flySpeedY + 0.1f, targetSpeed);
            }
        }

        private void HandleGroundProximity()
        {
            float groundBuffer = distanceFromGround;

            if (RayTracing.CastTileCollisionLength(NPC.Center, Vector2.UnitY, groundBuffer) < groundBuffer)
            {
                NPC.velocity.Y -= 0.1f;
                flySpeedY = Math.Max(flySpeedY - 0.1f, -2f);
            }
        }

        private void HandleObstacleAvoidance()
        {
            float obstacleBuffer = 45f;

            if (RayTracing.CastTileCollisionLength(NPC.Center, Vector2.UnitX * NPC.direction, obstacleBuffer) < obstacleBuffer)
            {
                NPC.velocity.X -= 0.25f * NPC.direction;
                flySpeedX -= 0.25f * NPC.direction;

                NPC.velocity.Y -= 0.5f;
                flySpeedY = Math.Max(flySpeedY - 0.5f, -2f);
            }
        }

        private void SetFrame()
        {
            if (NPC.IsABestiaryIconDummy) AIState = (int)AICase.Fly;

            switch ((AICase)AIState)
            {
                case AICase.Fall:
                    yFrame = 1;
                    break;
                case AICase.Fly:
                    yFrame = 8;

                    if (NPC.frameCounter++ % 3 == 0)
                    {
                        yFrameWing++;
                        if (yFrameWing >= 3) yFrameWing = 0;
                    }
                    break;
                case AICase.Cast:
                    yFrame = AICounter <= 6 || AICounter >= 114 ? 9 : 10;

                    if (NPC.frameCounter++ % 3 == 0)
                    {
                        yFrameWing++;
                        if (yFrameWing >= 3) yFrameWing = 0;
                    }
                    break;
            }
        }

        int xFrame = 0;
        int yFrame = 0;
        int yFrameWing = 0;
        public override void FindFrame(int frameHeight)
        {
            SetFrame();

            NPC.spriteDirection = NPC.direction;
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width;
            NPC.frame.Height = TextureAssets.Npc[NPC.type].Value.Height / 11;

            NPC.frame.X = NPC.frame.Width * xFrame;
            NPC.frame.Y = NPC.frame.Height * yFrame;
        }

        protected override void DrawNPCBestiary(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D wingTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "BookWings").Value;

            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            int wingTextureHeight = (int)(wingTexture.Height / 3f);
            Vector2 bestiaryDrawOffset = new Vector2(-6, -28);
            spriteBatch.Draw(wingTexture, NPC.Center + bestiaryDrawOffset, new Rectangle(0, wingTextureHeight * yFrameWing, wingTexture.Width, wingTextureHeight), drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
            spriteBatch.Draw(texture, NPC.Center + new Vector2(-8, 0), NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
        }

        public override bool DrawNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D wingTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "BookWings").Value;

            int wingTextureHeight = (int)(wingTexture.Height / 3f);
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            /*if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(Type, out NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers))
           {
               drawModifiers.Position = new Vector2(8, 8);
               drawModifiers.PortraitPositionXOverride = 8;
               drawModifiers.PortraitPositionYOverride = -6;

               // Replace the existing NPCBestiaryDrawModifiers with our new one with an adjusted rotation
               NPCID.Sets.NPCBestiaryDrawOffset.Remove(Type);
               NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
           }*/

            int xOffset = NPC.direction == -1 ? 4 : -10;
            Vector2 drawOffset = new Vector2(xOffset, -28);

            var lightAverage = (drawColor.R / 255f + drawColor.G / 255f + drawColor.B / 255f) / 3;
            if (Main.LocalPlayer.HasBuff(BuffID.Hunter))
            {
                drawColor = Color.Lerp(new Color(255, 50, 50), drawColor, lightAverage);
            }

            Color wingColor = Color.Lerp(drawColor, Color.White, 0.7f);
            spriteBatch.Draw(wingTexture, NPC.Center + drawOffset - Main.screenPosition, new Rectangle(0, wingTextureHeight * yFrameWing, wingTexture.Width, wingTextureHeight), wingColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            return false;
        }

        private void AI_114_Dragonflies()
        {
            if (NPC.localAI[0] == 0f && Main.netMode != 1)
            {
                NPC.localAI[0] = 1f;
                Vector2 center = NPC.Center;
                NPC.ai[2] = center.X;
                NPC.ai[3] = center.Y;
                NPC.velocity = (Main.rand.NextVector2Circular(5f, 3f) + Main.rand.NextVector2CircularEdge(5f, 3f)) * 0.4f;
                NPC.ai[1] = 0f;
                NPC.ai[0] = 1f;
                NPC.netUpdate = true;
            }
            switch ((int)NPC.ai[0])
            {
                case 0:
                    NPC.velocity *= 0.94f;
                    if (Main.netMode != 1 && (NPC.ai[1] += 1f) >= (float)(60 + Main.rand.Next(60)))
                    {
                        Vector2 vector = new Vector2(NPC.ai[2], NPC.ai[3]);
                        if (NPC.Distance(vector) > 96f)
                        {
                            NPC.velocity = NPC.DirectionTo(vector) * 3f;
                        }
                        else if (NPC.Distance(vector) > 16f)
                        {
                            NPC.velocity = NPC.DirectionTo(vector) * 1f + Main.rand.NextVector2Circular(1f, 0.5f);
                        }
                        else
                        {
                            NPC.velocity = (Main.rand.NextVector2Circular(5f, 3f) + Main.rand.NextVector2CircularEdge(5f, 3f)) * 0.4f;
                        }
                        NPC.ai[1] = 0f;
                        NPC.ai[0] = 1f;
                        NPC.netUpdate = true;
                    }
                    break;
                case 1:
                    {
                        int num = 4;
                        Vector2 other = new Vector2(NPC.ai[2], NPC.ai[3]);
                        if (NPC.Distance(other) > 112f)
                        {
                            num = 200;
                        }
                        if ((NPC.ai[1] += 1f) >= (float)num)
                        {
                            NPC.ai[1] = 0f;
                            NPC.ai[0] = 0f;
                            NPC.netUpdate = true;
                        }
                        int num2 = (int)NPC.Center.X / 16;
                        int num3 = (int)NPC.Center.Y / 16;
                        int num4 = 3;
                        for (int i = num3; i < num3 + num4; i++)
                        {
                            if (Main.tile[num2, i] != null && ((Main.tile[num2, i].HasUnactuatedTile && Main.tileSolid[Main.tile[num2, i].TileType]) || Main.tile[num2, i].LiquidAmount > 0))
                            {
                                if (NPC.velocity.Y > 0f)
                                {
                                    NPC.velocity.Y *= 0.9f;
                                }
                                NPC.velocity.Y -= 0.2f;
                            }
                        }
                        if (!(NPC.velocity.Y < 0f))
                        {
                            break;
                        }
                        int num5 = 30;
                        bool flag = false;
                        for (int j = num3; j < num3 + num5; j++)
                        {
                            if (Main.tile[num2, j] != null && Main.tile[num2, j].HasUnactuatedTile && Main.tileSolid[Main.tile[num2, j].TileType])
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag && NPC.velocity.Y < 0f)
                        {
                            NPC.velocity.Y *= 0.9f;
                        }
                        break;
                    }
            }
            if (NPC.velocity.X != 0f)
            {
                NPC.direction = ((NPC.velocity.X > 0f) ? 1 : (-1));
            }
            if (NPC.wet)
            {
                NPC.velocity.Y = -3f;
            }
            if (NPC.localAI[1] > 0f)
            {
                NPC.localAI[1] -= 1f;
                return;
            }
            NPC.localAI[1] = 15f;
            float num6 = 0f;
            Vector2 zero = Vector2.Zero;
            for (int k = 0; k < 200; k++)
            {
                NPC nPC = Main.npc[k];
                if (nPC.active && nPC.damage > 0 && !nPC.friendly && nPC.Hitbox.Distance(NPC.Center) <= 100f)
                {
                    num6 += 1f;
                    zero += NPC.DirectionFrom(nPC.Center);
                }
            }
            for (int l = 0; l < 255; l++)
            {
                Player player = Main.player[l];
                if (player.active && player.Hitbox.Distance(NPC.Center) <= 150f)
                {
                    num6 += 1f;
                    zero += NPC.DirectionFrom(player.Center);
                }
            }
            if (num6 > 0f)
            {
                float num7 = 2f;
                zero /= num6;
                zero *= num7;
                NPC.velocity += zero;
                if (NPC.velocity.Length() > 16f)
                {
                    NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero) * 16f;
                }
                Vector2 vector2 = NPC.Center + zero * 10f;
                NPC.ai[1] = -10f;
                NPC.ai[0] = 1f;
                NPC.ai[2] = vector2.X;
                NPC.ai[3] = vector2.Y;
                NPC.netUpdate = true;
            }
            else
            {
                if (Main.netMode == 1 || !((new Vector2(NPC.ai[2], NPC.ai[3]) - NPC.Center).Length() < 16f))
                {
                    return;
                }
                int maxValue = 30;
                if (Main.tile[(int)NPC.ai[2] / 16, (int)NPC.ai[3] / 16].TileType != 519)
                {
                    maxValue = 4;
                }
                if (Main.rand.Next(maxValue) != 0)
                {
                    return;
                }
                int cattailX = (int)NPC.ai[2];
                int cattailY = (int)NPC.ai[2];
                if (NPC.FindCattailTop((int)NPC.ai[2] / 16, (int)NPC.ai[3] / 16, out cattailX, out cattailY))
                {
                    NPC.ai[2] = cattailX * 16;
                    NPC.ai[3] = cattailY * 16;
                    NPC.netUpdate = true;
                    return;
                }
                int num8 = (int)(NPC.Center.X / 16f);
                int m;
                for (m = (int)(NPC.Center.Y / 16f); !WorldGen.SolidTile(num8, m) && (double)m < Main.worldSurface; m++)
                {
                }
                m -= Main.rand.Next(3, 6);
                NPC.ai[2] = num8 * 16;
                NPC.ai[3] = m * 16;
                NPC.netUpdate = true;
            }
        }
    }
}