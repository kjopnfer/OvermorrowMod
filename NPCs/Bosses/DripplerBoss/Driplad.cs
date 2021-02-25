using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Projectiles.Boss;
using OvermorrowMod.WardenClass.Weapons.Artifacts;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.DripplerBoss
{
    [AutoloadBossHead]
    public class Driplad : ModNPC
    {
        private NPC parentNPC;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dripplad");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 68;
            npc.height = 110;
            npc.damage = 30;
            npc.defense = 20;
            npc.lifeMax = 250;
            npc.HitSound = SoundID.NPCHit19;
            npc.knockBackResist = 0f;
            npc.DeathSound = SoundID.NPCDeath22;
            npc.noGravity = true;
            npc.noTileCollide = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale);
            npc.damage = (int)(npc.damage * 1.2f);
        }

        public override void AI()
        {
            if (!Main.npc[(int)npc.ai[1]].active)
            {
                npc.TargetClosest(false);
                npc.direction = 1;
                npc.velocity.Y = npc.velocity.Y - 0.1f;
                if (npc.timeLeft > 20)
                {
                    npc.timeLeft = 20;
                    return;
                }
            }
            else
            {
                parentNPC = Main.npc[(int)npc.ai[1]];
            }

            Player player = Main.player[npc.target];
            npc.TargetClosest(true);
            npc.ai[0]++;
            switch (npc.ai[2])
            {
                case 0: // Do nothing
                    int reduceTimer = parentNPC.life <= parentNPC.lifeMax * 0.25 ? 60 : 0;
                    if (npc.ai[0] % Main.rand.Next(240 - reduceTimer, 300 - reduceTimer) == 0)
                    {
                        npc.ai[0] = 0;
                        npc.ai[2] = 1;
                    }
                    break;
                case 1: // Shoot projectile
                    if(npc.ai[0] % 60 == 0)
                    {
                        float numberProjectiles = 6 + Main.rand.Next(7);
                        float rotation = MathHelper.ToRadians(360);
                        Vector2 delta = player.Center - npc.Center;
                        float magnitude = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
                        if (magnitude > 0)
                        {
                            delta *= 5f / magnitude;
                        }
                        else
                        {
                            delta = new Vector2(0f, 5f);
                        }
                        for (int i = 0; i < numberProjectiles; i++)
                        {
                            Vector2 perturbedSpeed = new Vector2(delta.X, delta.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .3f;
                            // * 3f increases speed
                            Projectile.NewProjectile(npc.Center.X, npc.Center.Y, perturbedSpeed.X * 3f, perturbedSpeed.Y * 3f, ModContent.ProjectileType<BloodyBall>(), npc.damage, 2f, Main.myPlayer, 0f, 0f);
                        }
                    }

                    if(npc.ai[0] == 180)
                    {
                        npc.ai[0] = 0;
                        npc.ai[2] = 0;
                    }
                    break;
            }
            
            if(npc.ai[0] == 1000)
            {
                npc.ai[0] = 0;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            int num = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];

            npc.rotation = npc.velocity.X * 0.15f;
            npc.frameCounter += 1.0;
            if (npc.frameCounter > 6.0)
            {
                npc.frameCounter = 0.0;
                npc.frame.Y += num;
            }
            if (npc.frame.Y >= num * Main.npcFrameCount[npc.type])
            {
                npc.frame.Y = 0;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = mod.GetTexture("NPCs/Bosses/DripplerBoss/Driplad_Glowmask");
            spriteBatch.Draw(texture, new Vector2(npc.Center.X - Main.screenPosition.X, npc.Center.Y - Main.screenPosition.Y + 4), npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void NPCLoot()
        {
            if (Main.npc[(int)npc.ai[1]].active)
            {
                NPC parentNPC = Main.npc[(int)npc.ai[1]];
                parentNPC.life -= 200;
            }
        }
    }
}