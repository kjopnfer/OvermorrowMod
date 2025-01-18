using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Common;
using Terraria.ID;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using OvermorrowMod.Content.Biomes;
using Terraria.Localization;
using System;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Common.Particles;
using Terraria.Map;

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

        protected virtual int CastTime => 120;
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue(LocalizationPath.Bestiary + Name)),
            });
        }

        public ref float AIState => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];
        public enum AICase
        {
            Hidden = -2,
            Fall = -1,
            Fly = 0,
            Cast = 1
        }

        protected int distanceFromGround = 180;
        protected int aggroDelayTime = 60;
        protected int tileAttackDistance = 24;
        protected Vector2 targetPosition;
        public override void OnSpawn(IEntitySource source)
        {
            AIState = (int)AICase.Fall;
            distanceFromGround = Main.rand.Next(16, 19) * 8;
            aggroDelayTime = Main.rand.Next(10, 20) * 10;
            tileAttackDistance = Main.rand.Next(16, 32) * 16;

            aggroDelay = aggroDelayTime;
            targetPosition = NPC.Center + new Vector2(14 * 16 * NPC.direction, 0).RotatedByRandom(MathHelper.PiOver2);

            NPC.netUpdate = true;
        }

        float flySpeedX = 2;
        float flySpeedY = 0;
        float aggroDelay = 60;
        public sealed override void AI()
        {
            //Dust.NewDust(targetPosition, 1, 1, DustID.Torch);
            //Lighting.AddLight(NPC.Center, Color.White.ToVector3() * 0.2f);
            switch ((AICase)AIState)
            {
                case AICase.Fall:
                    NPC.noGravity = false;

                    if (AICounter++ >= 36)
                    {
                        NPC.noGravity = true;
                        AIState = (int)AICase.Fly;
                        AICounter = 0;
                    }
                    break;
                case AICase.Fly:
                    NPC.TargetClosest();

                    HandleHorizontalMovement();
                    HandleVerticalMovement();
                    HandleGroundProximity();
                    HandleObstacleAvoidance();

                    if (AttackCondition())
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

                    if (AICounter++ > CastTime)
                    {
                        distanceFromGround = Main.rand.Next(16, 19) * 8;
                        aggroDelayTime = Main.rand.Next(10, 20) * 10;
                        tileAttackDistance = Main.rand.Next(16, 32) * 16;

                        AIState = (int)AICase.Fly;
                        AICounter = 0;
                        NPC.localAI[0] = 0;
                        aggroDelay = aggroDelayTime;
                    }
                    break;
            }

            NPC.rotation = NPC.velocity.Y * 0.08f;

            base.AI();
        }

        public abstract bool AttackCondition();
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

            if (NPC.Center.Y <= Player.Center.Y - verticalBuffer)
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
                    if (NPC.frameCounter++ % 4 == 0)
                    {
                        yFrame++;
                        if (yFrameWing >= 8) yFrameWing = 8;
                    }
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
                    yFrame = AICounter <= 6 || AICounter >= CastTime - 6 ? 9 : 10;

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

        protected virtual void DrawCastEffect(SpriteBatch spriteBatch) { }

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

        public override void DrawBehindOvermorrowNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if ((AICase)AIState == AICase.Cast)
                DrawCastEffect(spriteBatch);
        }

        public override bool DrawOvermorrowNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
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
            if ((AICase)AIState != AICase.Fall)
                spriteBatch.Draw(wingTexture, NPC.Center + drawOffset - Main.screenPosition, new Rectangle(0, wingTextureHeight * yFrameWing, wingTexture.Width, wingTextureHeight), wingColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            return false;
        }
    }
}