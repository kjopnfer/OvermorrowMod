/*using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Items.Materials;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.BoneSpider
{
    public class BoneSpider : ModNPC
    {
        private bool isWalking = true;
        private int frame = 0;
        private int frameTimer = 0;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Soulfire Creeper");
            Main.npcFrameCount[NPC.type] = 8;
        }

        public override void SetDefaults()
        {
            NPC.width = 76;
            NPC.height = 52;
            NPC.damage = 20;
            NPC.defense = 6;
            NPC.lifeMax = 200;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;
            NPC.knockBackResist = 0.25f;
            NPC.aiStyle = 3;
            AIType = NPCID.GoblinScout;
        }

        public override void AI()
        {
            NPC.ai[0]++;

            if (NPC.ai[0] == 240)
            {
                if (!isWalking)
                {
                    NPC.aiStyle = 3;
                    isWalking = true;
                    NPC.ai[0] = 0;
                }
                else
                {
                    NPC.velocity = Vector2.Zero;
                    NPC.aiStyle = -1;
                    isWalking = false;
                    NPC.ai[0] = 0;
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
                if (NPC.ai[0] == 120)
                {
                    Vector2 origin = NPC.Center; // Origin of the circle
                    float radius = 175; // Distance from the circle
                    int numSpawns = 3; // Points spawned on the circle
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < numSpawns; i++)
                        {
                            Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numSpawns * i)) * radius;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), position, Vector2.Zero, ModContent.ProjectileType<BlueSpiderFire>(), NPC.damage, 2f, Main.myPlayer);
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
                NPC.spriteDirection = NPC.direction;
            }
            NPC.frame.Y = frameHeight * frame;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("OvermorrowMod/Content/NPCs/BoneSpider/BoneSpider_Glowmask").Value;
            spriteBatch.Draw(texture, new Vector2(NPC.Center.X - Main.screenPosition.X, NPC.Center.Y - Main.screenPosition.Y + 4), NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoulFire>(), 3));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneDungeon ? 0.025f : 0f;
        }
    }
}*/