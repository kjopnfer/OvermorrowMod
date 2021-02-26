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
    public class RotatingDriplad : ModNPC
    {
        public override string Texture => "OvermorrowMod/NPCs/Bosses/DripplerBoss/Driplad";
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
            if (!Main.npc[(int)npc.ai[1]].active && !Main.npc[(int)npc.ai[1]].boss)
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

            // AI[0] is the orbit position timer
            // AI[1] is used for locating Parent NPC
            // AI[2] is the phase
            // AI[3] is the timer


            Player player = Main.player[npc.target];
            npc.TargetClosest(true);
            npc.ai[3]++;

            NPC_OrbitPosition(npc, parentNPC.Center, 350, 1.25f);

            switch (npc.ai[2])
            {
                case 0: // Do nothing
                    if (npc.ai[0] % Main.rand.Next(240, 480) == 0)
                    {
                        npc.ai[2] = 1;
                        npc.ai[3] = 0;
                    }
                    break;
                case 1: // Shoot projectile
                    if(npc.ai[0] % 60 == 0)
                    {
                        float numberProjectiles = 6 + Main.rand.Next(4);
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

                    if(npc.ai[3] == 180)
                    {
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                    }
                    break;
            }
            
            if(npc.ai[3] == 1000)
            {
                npc.ai[3] = 0;
            }
        }

        public void NPC_OrbitPosition(NPC modNPC, Vector2 position, double distance, double speed = 1.75)
        {
            double rad;
            double deg = speed * (double)npc.ai[0];
            rad = deg * (Math.PI / 180);
            npc.ai[0] += 1f;

            npc.position.X = position.X - (int)(Math.Cos(rad) * distance) - npc.width / 2;
            npc.position.Y = position.Y - (int)(Math.Sin(rad) * distance) - npc.height / 2;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life > 0)
            {
                for (int num826 = 0; (double)num826 < 10 / (double)npc.lifeMax * 100.0; num826++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f);
                }
                return;
            }
            for (int num827 = 0; num827 < 50; num827++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 5, 2.5f * (float)hitDirection, -2.5f);
            }

            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Driplad1"), npc.scale);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Driplad2"), npc.scale);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Driplad3"), npc.scale);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Driplad" + (Main.rand.Next(1, 4)).ToString()), npc.scale);
            Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Driplad" + (Main.rand.Next(1, 4)).ToString()), npc.scale);
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