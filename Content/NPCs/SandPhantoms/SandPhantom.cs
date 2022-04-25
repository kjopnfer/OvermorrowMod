using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.SandPhantoms
{
    public class SandPhantom : ModNPC
    {
        public enum Attacks { Swing = 1, Throw = 2 }

        protected int MoveDirection;
        protected Vector2 InitialPosition;
        protected bool SlashPlayer;

        public bool ThrewSword = true;

        private int frame = 0;
        private int frameTimer = 0;

        public override bool CheckActive() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand Phantom");
            Main.npcFrameCount[NPC.type] = 18;
            NPCID.Sets.TrailCacheLength[NPC.type] = 7;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 26;
            NPC.defense = 9;
            NPC.lifeMax = 200;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath53;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                writer.Write(frame);
                writer.Write(frameTimer);
                writer.Write(MoveDirection);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                frame = reader.ReadInt32();
                frameTimer = reader.ReadInt32();
                MoveDirection = reader.ReadInt32();
            }
        }

        public enum AIStates
        {
            Float = 0,
            Swing = 1,
            Throw = 2
        }

        public ref float AICase => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];
        public ref float MiscCounter => ref NPC.ai[2];

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return frame > 15 && frame < 17;
        }

        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];

            switch (AICase)
            {
                case (int)AIStates.Float:

                    SlashPlayer = false;
                    if (NPC.Hitbox.Intersects(player.Hitbox))
                    {
                        SlashPlayer = true;
                    }


                    if (!SlashPlayer)
                    {
                        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, (player.Center.X > NPC.Center.X ? 1 : -1) * 2, 0.05f);
                        NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, player.Center.Y > NPC.Center.Y ? 2.5f : -2.5f, 0.02f);
                    }
                    else
                    {
                        NPC.velocity = Vector2.Zero;
                        MiscCounter++;
                    }

                    if (AICounter++ == 180)
                    {
                        SlashPlayer = false;

                        InitialPosition = NPC.Center;
                        NPC.velocity = Vector2.Zero;

                        MoveDirection = Main.rand.NextBool() ? -1 : 1;

                        Attacks[] RandomAttack = (Attacks[])Enum.GetValues(typeof(Attacks));
                        RandomAttack.Shuffle();

                        AICase = (int)RandomAttack[0];
                        AICounter = 0;
                        MiscCounter = 0;

                        NPC.netUpdate = true;
                    }
                    break;
                case (int)AIStates.Swing:
                    // Move backwards while disappearing
                    if (AICounter++ <= 60)
                    {
                        Vector2 PositionOffset = Vector2.UnitX * 50 * MoveDirection;
                        NPC.Center = Vector2.Lerp(InitialPosition, InitialPosition + PositionOffset, AICounter / 60f);
                        NPC.alpha = (int)MathHelper.Lerp(0, 255, Utils.Clamp(AICounter, 0, 45) / 45f);
                    }

                    // Lunge forward with a swing
                    if (AICounter > 60 && AICounter < 180)
                    {
                        NPC.direction = MoveDirection;

                        // Determine which side of the player to 
                        if (AICounter < 150)
                        {
                            NPC.Center = player.Center + Vector2.UnitX * 75 * -MoveDirection;
                            NPC.alpha = (int)MathHelper.Lerp(255, 0, (AICounter - 60f) / 90f);
                        }
                        else
                        {
                            if (AICounter == 170)
                            {
                                NPC.velocity = Vector2.UnitX * 10 * NPC.direction;
                            }
                        }
                    }

                    if (AICounter == 180)
                    {
                        NPC.velocity = Vector2.Zero;

                        AICase = (int)AIStates.Float;
                        AICounter = 0;
                    }
                    break;
                case (int)AIStates.Throw:
                    //Main.NewText("throw");

                    if (AICounter++ == 120)
                    {
                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, NPC.DirectionTo(player.Center) * 2, ModContent.ProjectileType<PhantomSword>(), 30, 3f, Main.myPlayer, 0, NPC.whoAmI);
                    }

                    if (!ThrewSword)
                    {
                        ThrewSword = true;
                        AICase = (int)AIStates.Float;
                        AICounter = 0;
                    }
                    break;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            NPC.frame.Y = frameHeight * frame;

            switch (AICase)
            {
                case (int)AIStates.Float:
                    if (SlashPlayer)
                    {
                        if (MiscCounter < 60)
                        {
                            frame = 15;
                            frameTimer = 0;
                        }
                        else
                        {
                            if (frame < 17)
                            {
                                frameTimer++;
                                if (frameTimer % 5 == 0)
                                {
                                    frame++;
                                }
                            }
                            else
                            {
                                MiscCounter = 0;
                                SlashPlayer = false;
                            }
                        }
                    }
                    else
                    {

                        // Float animation
                        // Frames 0 - 11
                        if (frame < 11)
                        {
                            frameTimer++;
                            if (frameTimer % 8 == 0)
                            {
                                frame++;
                            }
                        }
                        else
                        {
                            frameTimer++;
                            if (frameTimer % 8 == 0)
                            {
                                frame = 0;
                                frameTimer = 0;
                            }
                        }
                    }
                    break;
                case (int)AIStates.Swing:
                    if (AICounter < 170)
                    {
                        frame = 15;
                        frameTimer = 0;
                    }
                    else
                    {
                        if (frame < 17)
                        {
                            frameTimer++;
                            if (frameTimer % 5 == 0)
                            {
                                frame++;
                            }
                        }
                    }
                    break;
                case (int)AIStates.Throw:
                    if (AICounter < 120)
                    {
                        frame = 12;
                        frameTimer = 0;
                    }
                    else
                    {
                        if (frame != 14)
                        {
                            frameTimer++;
                            if (frameTimer % 5 == 0)
                            {
                                frame++;
                            }
                        }
                    }
                    break;
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            position = NPC.Center + new Vector2(16 * -NPC.direction, 16);
            return true;
        }

        private const int MAX_FRAMES = 6;
        private const int TEXTURE_HEIGHT = 66;

        private int bottomFrame = 0;
        private int bottomFrameTimer = 0;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var spriteEffects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (bottomFrame < 5)
            {
                bottomFrameTimer++;
                if (bottomFrameTimer % 8 == 0)
                {
                    bottomFrame++;
                }
            }
            else
            {
                bottomFrameTimer++;
                if (bottomFrameTimer % 8 == 0)
                {
                    bottomFrame = 0;
                    bottomFrameTimer = 0;
                }
            }

            if (AICase == (int)AIStates.Swing && NPC.velocity != Vector2.Zero)
            {
                Texture2D npcTexture = TextureAssets.Npc[NPC.type].Value;
                for (int k = 0; k < NPC.oldPos.Length; k++)
                {
                    // Adjust drawPos if the hitbox does not match sprite dimension
                    Vector2 drawPos = NPC.oldPos[k] + npcTexture.Size() / 2f - screenPos;
                    Color afterImageColor = NPC.GetAlpha(new Color(56, 40, 26)) * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
                    spriteBatch.Draw(npcTexture, drawPos + new Vector2(-23, -29), NPC.frame, afterImageColor, NPC.rotation, npcTexture.Size() / 2f, NPC.scale, spriteEffects, 0f);
                }
            }

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.NPC + "SandPhantoms/SandPhantom_Bottom").Value;
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Color color = Color.Lerp(drawColor, Color.Transparent, NPC.alpha / 255f);
            Rectangle drawRectangle = new Rectangle(0, TEXTURE_HEIGHT * bottomFrame, texture.Width, TEXTURE_HEIGHT);

            Main.spriteBatch.Draw(texture, NPC.Center + new Vector2(0, (TEXTURE_HEIGHT / 2 * MAX_FRAMES) - 12 - (TEXTURE_HEIGHT / 2)) - Main.screenPosition, drawRectangle, color, NPC.rotation, origin, 1f, spriteEffects, 0f);

            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return Sandstorm.Happening && spawnInfo.player.ZoneDesert ? 0.025f : 0f;
        }
    }
}