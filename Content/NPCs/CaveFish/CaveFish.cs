/*using OvermorrowMod.Content.Biomes;
using OvermorrowMod.Content.Items.Accessories;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.CaveFish
{
    public class CaveFish : ModNPC
    {
        protected float speed = 7f;
        protected float speedY = 4f;
        protected float acceleration = 0.25f;
        protected float accelerationY = 0.2f;
        protected float correction = 0.95f;
        protected bool targetDryPlayer = true;
        protected float idleSpeed = 2f;
        protected bool bounces = true;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sullen Angler");
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.width = 64;
            NPC.height = 42;
            NPC.lifeMax = 100;
            NPC.damage = 26;
            NPC.defense = 13;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath7;
        }

        public override void AI()
        {
            if (NPC.direction == 0)
            {
                NPC.TargetClosest(true);
            }
            if (NPC.wet)
            {
                bool flag30 = false;
                NPC.TargetClosest(false);
                if (Main.player[NPC.target].wet && !Main.player[NPC.target].dead)
                {
                    flag30 = true;
                }
                if (!flag30)
                {
                    if (NPC.collideX)
                    {
                        NPC.velocity.X *= -1f;
                        NPC.direction *= -1;
                        NPC.netUpdate = true;
                    }
                    if (NPC.collideY)
                    {
                        NPC.netUpdate = true;
                        if (NPC.velocity.Y > 0f)
                        {
                            NPC.velocity.Y = -NPC.velocity.Y;
                            NPC.directionY = -1;
                            NPC.ai[0] = -1f;
                        }
                        else if (NPC.velocity.Y < 0f)
                        {
                            NPC.velocity.Y = -NPC.velocity.Y;
                            NPC.directionY = 1;
                            NPC.ai[0] = 1f;
                        }
                    }
                }
                if (flag30)
                {
                    NPC.TargetClosest(true);
                    if (NPC.velocity.X * NPC.direction < 0f)
                    {
                        NPC.velocity.X *= correction;
                    }
                    NPC.velocity.X += NPC.direction * acceleration;
                    NPC.velocity.Y += NPC.directionY * accelerationY;
                    if (NPC.velocity.X > speed)
                    {
                        NPC.velocity.X = speed;
                    }
                    if (NPC.velocity.X < -speed)
                    {
                        NPC.velocity.X = -speed;
                    }
                    if (NPC.velocity.Y > speedY)
                    {
                        NPC.velocity.Y = speedY;
                    }
                    if (NPC.velocity.Y < -speedY)
                    {
                        NPC.velocity.Y = -speedY;
                    }
                }
                else
                {
                    if (targetDryPlayer)
                    {
                        if (Main.player[NPC.target].position.Y > NPC.position.Y)
                        {
                            NPC.directionY = 1;
                        }
                        else
                        {
                            NPC.directionY = -1;
                        }
                        NPC.velocity.X += (float)NPC.direction * 0.1f * idleSpeed;
                        if (NPC.velocity.X < -idleSpeed || NPC.velocity.X > idleSpeed)
                        {
                            NPC.velocity.X *= 0.95f;
                        }
                        if (NPC.ai[0] == -1f)
                        {
                            float num356 = -0.3f * idleSpeed;
                            if (NPC.directionY < 0)
                            {
                                num356 = -0.5f * idleSpeed;
                            }
                            if (NPC.directionY > 0)
                            {
                                num356 = -0.1f * idleSpeed;
                            }
                            NPC.velocity.Y -= 0.01f * idleSpeed;
                            if (NPC.velocity.Y < num356)
                            {
                                NPC.ai[0] = 1f;
                            }
                        }
                        else
                        {
                            float num357 = 0.3f * idleSpeed;
                            if (NPC.directionY < 0)
                            {
                                num357 = 0.1f * idleSpeed;
                            }
                            if (NPC.directionY > 0)
                            {
                                num357 = 0.5f * idleSpeed;
                            }
                            NPC.velocity.Y += 0.01f * idleSpeed;
                            if (NPC.velocity.Y > num357)
                            {
                                NPC.ai[0] = -1f;
                            }
                        }
                    }
                    else
                    {
                        NPC.velocity.X += (float)NPC.direction * 0.1f * idleSpeed;
                        if (NPC.velocity.X < -idleSpeed || NPC.velocity.X > idleSpeed)
                        {
                            NPC.velocity.X *= 0.95f;
                        }
                        if (NPC.ai[0] == -1f)
                        {
                            NPC.velocity.Y -= 0.01f * idleSpeed;
                            if ((double)NPC.velocity.Y < -0.3)
                            {
                                NPC.ai[0] = 1f;
                            }
                        }
                        else
                        {
                            NPC.velocity.Y += 0.01f * idleSpeed;
                            if ((double)NPC.velocity.Y > 0.3)
                            {
                                NPC.ai[0] = -1f;
                            }
                        }
                    }
                    int num358 = (int)(NPC.position.X + (float)(NPC.width / 2)) / 16;
                    int num359 = (int)(NPC.position.Y + (float)(NPC.height / 2)) / 16;
                    if (Main.tile[num358, num359 - 1].LiquidAmount > 128)
                    {
                        if (Main.tile[num358, num359 + 1].HasTile)
                        {
                            NPC.ai[0] = -1f;
                        }
                        else if (Main.tile[num358, num359 + 2].HasTile)
                        {
                            NPC.ai[0] = -1f;
                        }
                    }
                    if (!targetDryPlayer && ((double)NPC.velocity.Y > 0.4 || (double)NPC.velocity.Y < -0.4))
                    {
                        NPC.velocity.Y *= 0.95f;
                    }
                }
            }
            else
            {
                if (NPC.velocity.Y == 0f)
                {
                    if (!bounces)
                    {
                        NPC.velocity.X *= 0.94f;
                        if ((double)NPC.velocity.X > -0.2 && (double)NPC.velocity.X < 0.2)
                        {
                            NPC.velocity.X = 0f;
                        }
                    }
                    else if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.velocity.Y = (float)Main.rand.Next(-50, -20) * 0.1f;
                        NPC.velocity.X = (float)Main.rand.Next(-20, 20) * 0.1f;
                        NPC.netUpdate = true;
                    }
                }
                NPC.velocity.Y += 0.3f;
                if (NPC.velocity.Y > 10f)
                {
                    NPC.velocity.Y = 10f;
                }
                NPC.ai[0] = 1f;
            }
            NPC.rotation = NPC.velocity.Y * (float)NPC.direction * 0.1f;
            if ((double)NPC.rotation < -0.2)
            {
                NPC.rotation = -0.2f;
            }
            if ((double)NPC.rotation > 0.2)
            {
                NPC.rotation = 0.2f;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            NPC.frameCounter++;

            if (NPC.frameCounter % 24f == 23f) // Ticks per frame
            {
                NPC.frame.Y += frameHeight;
            }
            if (NPC.frame.Y >= frameHeight * 2) // 2 is max # of frames
            {
                NPC.frame.Y = 0; // Reset back to default
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AnglerTooth>(), 14));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.InModBiome(ModContent.GetInstance<WaterCaveBiome>()) ? 0.05f : 0f;
        }
    }
}*/