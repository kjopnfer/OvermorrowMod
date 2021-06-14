using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Items.Materials;
using OvermorrowMod.Projectiles.NPCs.Hostile;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs
{
    public class BoneSpider : ModNPC
    {
        private bool isWalking = true;
        private int frame = 0;
        private int frameTimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soulfire Creeper");
            Main.npcFrameCount[npc.type] = 8;
        }

        public override void SetDefaults()
        {
            npc.width = 76;
            npc.height = 52;
            npc.damage = 20;
            npc.defense = 6;
            npc.lifeMax = 200;
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath2;
            npc.value = 60f;
            npc.knockBackResist = 0.25f;
            npc.aiStyle = 3;
            aiType = NPCID.GoblinScout;
        }

        public override void AI()
        {
            npc.ai[0]++;

            if (npc.ai[0] == 240)
            {
                if (!isWalking)
                {
                    npc.aiStyle = 3;
                    isWalking = true;
                    npc.ai[0] = 0;
                }
                else
                {
                    npc.velocity = Vector2.Zero;
                    npc.aiStyle = -1;
                    isWalking = false;
                    npc.ai[0] = 0;
                }
            }

            if (isWalking)
            {
                // I was dumb and didn't realize the frame size multiplier works from 0 to (frame - 1)
                // Walking animation
                if (frame >= 4 && frame < 7)
                {
                    frameTimer++;
                    if (frameTimer % 7 == 0)
                    {
                        frame++;
                    }
                }
                else
                {
                    frameTimer++;
                    if (frameTimer % 7 == 0)
                    {
                        frame = 4;
                        frameTimer = 0;
                    }
                }
            }
            else
            {
                // Idle animation
                if(npc.ai[0] == 120)
                {
                    Vector2 origin = npc.Center; // Origin of the circle
                    float radius = 175; // Distance from the circle
                    int numSpawns = 3; // Points spawned on the circle
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < numSpawns; i++)
                        {
                            Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numSpawns * i)) * radius;
                            Projectile.NewProjectile(position, Vector2.Zero, ModContent.ProjectileType<SpiderFire>(), npc.damage, 2f, Main.myPlayer);
                        }
                    }
                }

                if (frame < 3)
                {
                    frameTimer++;
                    if (frameTimer % 6 == 0)
                    {
                        frame++;
                    }
                }
                else
                {
                    frameTimer++;
                    if (frameTimer % 6 == 0)
                    {
                        frame = 0;
                        frameTimer = 0;
                    }
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (isWalking)
            {
                npc.spriteDirection = npc.direction;
            }
            npc.frame.Y = frameHeight * frame;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = mod.GetTexture("NPCs/BoneSpider_Glowmask");
            spriteBatch.Draw(texture, new Vector2(npc.Center.X - Main.screenPosition.X, npc.Center.Y - Main.screenPosition.Y + 4), npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void NPCLoot()
        {
            if(Main.rand.Next(4) == 0)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<SoulFire>());
            }
        }
    }
}