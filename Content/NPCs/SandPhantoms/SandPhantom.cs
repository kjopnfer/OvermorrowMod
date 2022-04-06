using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System;
using System.IO;
using Terraria;
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
            Main.npcFrameCount[npc.type] = 18;
            NPCID.Sets.TrailCacheLength[npc.type] = 7;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.width = 32;
            npc.height = 32;
            npc.damage = 26;
            npc.defense = 9;
            npc.lifeMax = 200;
            npc.aiStyle = -1;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath53;
            npc.value = 60f;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
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

        public ref float AICase => ref npc.ai[0];
        public ref float AICounter => ref npc.ai[1];
        public ref float MiscCounter => ref npc.ai[2];

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return frame > 15 && frame < 17;
        }

        public override void AI()
        {
            npc.TargetClosest();
            Player player = Main.player[npc.target];

            switch (AICase)
            {
                case (int)AIStates.Float:

                    SlashPlayer = false;
                    if (npc.Hitbox.Intersects(player.Hitbox))
                    {
                        SlashPlayer = true;
                    }


                    if (!SlashPlayer)
                    {
                        npc.velocity.X = MathHelper.Lerp(npc.velocity.X, (player.Center.X > npc.Center.X ? 1 : -1) * 2, 0.05f);
                        npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, player.Center.Y > npc.Center.Y ? 2.5f : -2.5f, 0.02f);
                    }
                    else
                    {
                        npc.velocity = Vector2.Zero;
                        MiscCounter++;
                    }

                    if (AICounter++ == 180)
                    {
                        SlashPlayer = false;

                        InitialPosition = npc.Center;
                        npc.velocity = Vector2.Zero;

                        MoveDirection = Main.rand.NextBool() ? -1 : 1;

                        Attacks[] RandomAttack = (Attacks[])Enum.GetValues(typeof(Attacks));
                        RandomAttack.Shuffle();

                        AICase = (int)RandomAttack[0];
                        AICounter = 0;
                        MiscCounter = 0;

                        npc.netUpdate = true;
                    }
                    break;
                case (int)AIStates.Swing:
                    // Move backwards while disappearing
                    if (AICounter++ <= 60)
                    {
                        Vector2 PositionOffset = Vector2.UnitX * 50 * MoveDirection;
                        npc.Center = Vector2.Lerp(InitialPosition, InitialPosition + PositionOffset, AICounter / 60f);
                        npc.alpha = (int)MathHelper.Lerp(0, 255, Utils.Clamp(AICounter, 0, 45) / 45f);
                    }

                    // Lunge forward with a swing
                    if (AICounter > 60 && AICounter < 180)
                    {
                        npc.direction = MoveDirection;

                        // Determine which side of the player to 
                        if (AICounter < 150)
                        {
                            npc.Center = player.Center + Vector2.UnitX * 75 * -MoveDirection;
                            npc.alpha = (int)MathHelper.Lerp(255, 0, (AICounter - 60f) / 90f);
                        }
                        else
                        {
                            if (AICounter == 170)
                            {
                                npc.velocity = Vector2.UnitX * 10 * npc.direction;
                            }
                        }
                    }

                    if (AICounter == 180)
                    {
                        npc.velocity = Vector2.Zero;

                        AICase = (int)AIStates.Float;
                        AICounter = 0;
                    }
                    break;
                case (int)AIStates.Throw:
                    //Main.NewText("throw");

                    if (AICounter++ == 120)
                    {
                        Projectile.NewProjectile(npc.Center, npc.DirectionTo(player.Center) * 2, ModContent.ProjectileType<PhantomSword>(), 30, 3f, Main.myPlayer, 0, npc.whoAmI);
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
            npc.spriteDirection = npc.direction;
            npc.frame.Y = frameHeight * frame;

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
            position = npc.Center + new Vector2(16 * -npc.direction, 16);
            return true;
        }

        private const int MAX_FRAMES = 6;
        private const int TEXTURE_HEIGHT = 66;

        private int bottomFrame = 0;
        private int bottomFrameTimer = 0;
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            var spriteEffects = npc.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

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

            if (AICase == (int)AIStates.Swing && npc.velocity != Vector2.Zero)
            {
                Texture2D npcTexture = Main.npcTexture[npc.type];
                for (int k = 0; k < npc.oldPos.Length; k++)
                {
                    // Adjust drawPos if the hitbox does not match sprite dimension
                    Vector2 drawPos = npc.oldPos[k] + npcTexture.Size() / 2f - Main.screenPosition;
                    Color afterImageColor = npc.GetAlpha(new Color(56, 40, 26)) * ((npc.oldPos.Length - k) / (float)npc.oldPos.Length);
                    spriteBatch.Draw(npcTexture, drawPos + new Vector2(-23, -29), npc.frame, afterImageColor, npc.rotation, npcTexture.Size() / 2f, npc.scale, spriteEffects, 0f);
                }
            }

            Texture2D texture = ModContent.GetTexture(AssetDirectory.NPC + "SandPhantoms/SandPhantom_Bottom");
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Color color = Color.Lerp(drawColor, Color.Transparent, npc.alpha / 255f);
            Rectangle drawRectangle = new Rectangle(0, TEXTURE_HEIGHT * bottomFrame, texture.Width, TEXTURE_HEIGHT);

            Main.spriteBatch.Draw(texture, npc.Center + new Vector2(0, (TEXTURE_HEIGHT / 2 * MAX_FRAMES) - 12 - (TEXTURE_HEIGHT / 2)) - Main.screenPosition, drawRectangle, color, npc.rotation, origin, 1f, spriteEffects, 0f);

            return base.PreDraw(spriteBatch, drawColor);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return Sandstorm.Happening && spawnInfo.player.ZoneDesert ? 0.025f : 0f;
        }
    }
}