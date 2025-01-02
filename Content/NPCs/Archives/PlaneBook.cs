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
    public class PlaneBook : OvermorrowNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + Name;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
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
            NPC.damage = 23;
            NPC.knockBackResist = 0.5f;
            NPC.noGravity = true;
            NPC.value = Item.buyPrice(0, 0, silver: 2, copper: 20);

            SpawnModBiomes = [ModContent.GetInstance<GrandArchives>().Type, ModContent.GetInstance<Inkwell>().Type];
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue(LocalizatonPath.Bestiary + Name)),
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

        public override void OnSpawn(IEntitySource source)
        {
            AIState = (int)AICase.Fly;
        }

        float flySpeedX = 2;
        float flySpeedY = 0;
        float aggroDelay = 60;
        public override void AI()
        {
            NPC.TargetClosest();

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
                    bool xDistanceCheck = xDistance < 200;
                    bool yDistanceCheck = Math.Abs(NPC.Center.Y - player.Center.Y) < 100;

                    if (xDistanceCheck && yDistanceCheck && Collision.CanHitLine(player.Center, 1, 1, NPC.Center, 1, 1))
                    {
                        aggroDelay--;
                        if (aggroDelay <= 0)
                        {
                            AIState = (int)AICase.Cast;
                        }
                    }
                    break;
                case AICase.Cast:
                    NPC.velocity.X /= 2f;

                    HandleVerticalMovement();
                    HandleGroundProximity();

                    if (AICounter++ > 120)
                    {
                        AIState = (int)AICase.Fly;
                        AICounter = 0;
                        aggroDelay = 60;
                    }
                    break;
            }

            NPC.rotation = NPC.velocity.Y * 0.08f;

            base.AI();
        }

        private void HandleHorizontalMovement()
        {
            float targetSpeed = 2f;

            if (NPC.Center.X >= player.Center.X)
            {
                NPC.velocity.X = Math.Max(NPC.velocity.X - 0.05f, -targetSpeed);
                flySpeedX = Math.Max(flySpeedX - 0.1f, -targetSpeed);
            }
            else if (NPC.Center.X <= player.Center.X)
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
            float groundBuffer = 128f;

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

            spriteBatch.Draw(wingTexture, NPC.Center + drawOffset - Main.screenPosition, new Rectangle(0, wingTextureHeight * yFrameWing, wingTexture.Width, wingTextureHeight), drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            return false;
        }
    }
}