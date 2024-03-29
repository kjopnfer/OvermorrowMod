/*using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    public class EyeStalk : ModNPC
    {
        public float IdleDistance;
        public float LerpTime;

        public int StalkID;
        public int ParentIndex;

        private float InitialRotation;

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seer");
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 24;
            NPC.aiStyle = -1;
            NPC.lifeMax = 300;
            NPC.defense = 16;
            NPC.knockBackResist = 1.25f;
            NPC.friendly = false;
            NPC.noTileCollide = true;
        }

        public ref float AICase => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];
        public ref float MiscCounter => ref NPC.ai[2];
        public override void AI()
        {
            NPC parent = Main.npc[ParentIndex];
            if (!parent.active)
            {
                NPC.active = false;
                return;
            }

            if (AICounter++ == 0)
            {
                IdleDistance = Main.rand.Next(7, 11);
                LerpTime = Main.rand.Next(8, 12) * 5;
            }

            Vector2 StalkPosition = new Vector2(-80, 25);
            switch (StalkID)
            {
                case 2:
                    StalkPosition = new Vector2(-50, 75);
                    break;
                case 3:
                    StalkPosition = new Vector2(50, 75);
                    break;
                case 4:
                    StalkPosition = new Vector2(80, 25);
                    break;
            }

            float LerpCounter = (float)Math.Sin(MiscCounter++ / LerpTime);
            NPC.Center = parent.Center + Vector2.Lerp(StalkPosition.RotatedBy(parent.rotation), (StalkPosition + Vector2.UnitY * IdleDistance).RotatedBy(parent.rotation), LerpCounter);
            NPC.rotation = NPC.DirectionTo(Main.player[parent.target].Center).ToRotation() + MathHelper.PiOver2;

            switch (parent.ai[0])
            {
                case (float)EyeOfCthulhu.AIStates.Portal:
                    if (parent.ai[1] >= 120)
                    {
                        if (MiscCounter == 0)
                        {
                            InitialRotation = NPC.DirectionTo(Main.player[parent.target].Center).ToRotation() + MathHelper.PiOver2;
                        }

                        Main.NewText("rotating small");
                        float ToRotation = NPC.DirectionTo(NPC.Center + Vector2.UnitY * 50).ToRotation() + MathHelper.PiOver2;
                        NPC.rotation = MathHelper.Lerp(InitialRotation, ToRotation, Utils.Clamp(MiscCounter++, 0, 60) / 60f);

                        if (AICounter % 10 == 0 && parent.ai[2] > 60)
                        {
                            Main.NewText("bruh");
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitY * 5, ProjectileID.GreenLaser, NPC.damage, 2f, Main.myPlayer);
                        }
                    }
                    break;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC parent = Main.npc[ParentIndex];

            Vector2 mountedCenter = parent.Center;

            var drawPosition = NPC.Center;
            var remainingVectorToParent = mountedCenter - drawPosition;
            float rotation = remainingVectorToParent.ToRotation() - MathHelper.PiOver2;

            float CHAIN_LENGTH = 5;
            float distance = Vector2.Distance(parent.Center, NPC.Center);
            float iterations = distance / CHAIN_LENGTH;


            // Right side stalks
            Vector2 midPoint1 = parent.Center + new Vector2(50, -100).RotatedBy(NPC.DirectionTo(parent.Center).ToRotation() + MathHelper.PiOver2);
            Vector2 midPoint2 = NPC.Center + new Vector2(25, 25).RotatedBy(NPC.DirectionTo(parent.Center).ToRotation() + MathHelper.PiOver2);
            switch (StalkID)
            {
                // Left side stalks
                case 1:
                case 2:
                    midPoint1 = parent.Center + new Vector2(-50, -100).RotatedBy(NPC.DirectionTo(parent.Center).ToRotation() + MathHelper.PiOver2);
                    midPoint2 = NPC.Center + new Vector2(-25, 25).RotatedBy(NPC.DirectionTo(parent.Center).ToRotation() + MathHelper.PiOver2);
                    break;
            }

            for (int i = 0; i < iterations; i++)
            {
                Texture2D chainTexture = i % 2 == 0 ? ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/StalkBody").Value : ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/StalkBodyAlt").Value;
                if (i == 5)
                {
                    chainTexture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/StalkBodyBulb").Value;
                }

                float progress = i / iterations;
                Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));

                Vector2 position = ModUtils.Bezier(parent.Center, NPC.Center, midPoint1, midPoint2, progress);
                Main.EntitySpriteDraw(chainTexture, position - Main.screenPosition, null, color, rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }

            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/EyeStalk_Glow").Value;
            spriteBatch.Draw(texture, NPC.Center - screenPos, null, Color.White, NPC.rotation, texture.Size() / 2, NPC.scale, SpriteEffects.None, 0);
        }

        public override void OnKill()
        {
            if (Main.expertMode)
            {
                if (Main.rand.NextBool())
                {
                    int index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Seer.SeerHead>());

                    if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: index);
                    }
                }
            }

            base.OnKill();
        }
    }
}*/