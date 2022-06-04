using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.DripplerBoss
{
    [AutoloadBossHead]
    public class LoomingDrippler : ModNPC
    {
        public bool run;
        private NPC parentNPC;
        private int moveSpeed;
        private int randIncrementer;
        private bool secondTeleport = false;
        private int randSwitch = 300;
        private int storedDamage;
        private Vector2 origin;
        int attackCounter;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Looming Drippler");
            Main.npcFrameCount[NPC.type] = 8;
        }

        public override void SetDefaults()
        {
            NPC.width = 28;
            NPC.height = 30;
            NPC.damage = 28;
            NPC.defense = 0;
            NPC.lifeMax = 75;//200;
            NPC.HitSound = SoundID.NPCHit19;
            NPC.knockBackResist = 0.4f;
            NPC.DeathSound = SoundID.NPCDeath22;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * /*0.5f*/ 1 * bossLifeScale);
            NPC.defense = 0;
            NPC.knockBackResist = 0f;
        }

        public override void AI()
        {

            if (run)
            {
                NPC.velocity = Vector2.UnitY * -7.5f;
            }
            else
            {
                // Vanilla code is garbage that breaks itself, seriously what is even wrong with the X velocity of this crap
                // You can only count on your own code nowadays
                // This is the stupidest NPC I've ever made and I've made bosses, it LITERALLY WON'T MOVE

                if ((!Main.npc[(int)NPC.ai[1]].boss) || !Main.bloodMoon)
                {
                    NPC.TargetClosest(false);
                    NPC.direction = 1;
                    NPC.velocity.Y = NPC.velocity.Y - 0.1f;
                    if (NPC.timeLeft > 20)
                    {
                        NPC.timeLeft = 20;
                        return;
                    }
                }
                else
                {
                    parentNPC = Main.npc[(int)NPC.ai[1]];
                }

                Player player = Main.player[NPC.target];

                NPC.TargetClosest(true);
                NPC.ai[0]++;

                if (NPC.ai[3] == 0)
                {
                    moveSpeed = Main.rand.Next(5, 7);
                    NPC.ai[3]++;
                }

                // AI[0] is the counter
                // AI[1] is the parent NPC
                // AI[2] is the phase
                // AI[3] is the randomly assigned movespeed

                switch (NPC.ai[2])
                {
                    case -1: // Hide NPC
                        if (OvermorrowWorld.DripladShoot)
                        {
                            if (NPC.alpha < 255)
                            {
                                NPC.alpha += 3;
                            }

                            NPC.velocity = Vector2.Zero;
                            NPC.damage = 0;
                        }
                        else
                        {
                            if (NPC.alpha > 0)
                            {
                                NPC.alpha -= 3;
                            }

                            if (NPC.alpha <= 0)
                            {
                                NPC.damage = storedDamage;
                                NPC.ai[2] = 0;
                                NPC.ai[0] = 0;
                            }
                        }
                        break;
                    case 0: // Follow player
                        if (OvermorrowWorld.DripplerCircle)
                        {
                            NPC.ai[2] = 2;
                            NPC.ai[0] = 0;
                            break;
                        }

                        if (OvermorrowWorld.DripladShoot)
                        {
                            storedDamage = NPC.damage;
                            NPC.ai[2] = -1;
                            NPC.ai[0] = 0;
                            break;
                        }

                        Vector2 moveTo = player.Center;
                        var move = moveTo - NPC.Center;

                        float length = move.Length();
                        if (length > moveSpeed)
                        {
                            move *= moveSpeed / length;
                        }
                        var turnResistance = 45;
                        move = (NPC.velocity * turnResistance + move) / (turnResistance + 1f);
                        length = move.Length();
                        if (length > 10)
                        {
                            move *= moveSpeed / length;
                        }
                        NPC.velocity.X = move.X;
                        NPC.velocity.Y = move.Y * .98f;

                        if (NPC.ai[0] % randSwitch == 0)
                        {
                            NPC.ai[2] = 2;
                            NPC.ai[0] = 0;
                        }
                        break;
                    case 1: // Shoot at player
                        if (OvermorrowWorld.DripplerCircle)
                        {
                            NPC.ai[2] = 2;
                            NPC.ai[0] = 0;
                            break;
                        }

                        NPC.velocity = Vector2.Zero;
                        if (NPC.ai[0] % 30 == 0)
                        {
                            int shootSpeed = Main.rand.Next(6, 10);

                            if (parentNPC.life <= parentNPC.lifeMax * 0.39f)
                            {
                                int chooseShoot = Main.rand.Next(2);
                                if (chooseShoot == 0)
                                {
                                    float numberProjectiles = 3;
                                    float rotation = MathHelper.ToRadians(45);
                                    Vector2 delta = player.Center - NPC.Center;
                                    float magnitude = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
                                    if (magnitude > 0)
                                    {
                                        delta *= 5f / magnitude;
                                    }
                                    else
                                    {
                                        delta = new Vector2(0f, 5f);
                                    }

                                    SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        for (int i = 0; i < numberProjectiles; i++)
                                        {
                                            Vector2 perturbedSpeed = new Vector2(delta.X, delta.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .3f;
                                            // * 3f increases speed
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, perturbedSpeed.X * 3f, perturbedSpeed.Y * 3f, ModContent.ProjectileType<BloodyBall>(), NPC.damage / /*3*/ 2, 2f, Main.myPlayer, 0f, 0f);
                                        }
                                    }
                                }
                                else
                                {
                                    SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Main.player[NPC.target].Center) * shootSpeed, ModContent.ProjectileType<BloodyBall>(), NPC.damage / /*3*/ 2, 3f, Main.myPlayer, 0, 0);
                                    }
                                }
                            }
                            else
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Main.player[NPC.target].Center) * shootSpeed, ModContent.ProjectileType<BloodyBall>(), NPC.damage / /*4*/ 3, 3f, Main.myPlayer, 0, 0);
                                }
                            }
                        }

                        if (NPC.ai[0] == 90)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[2] = 0;
                        }
                        break;
                    case 2: // Teleportation buffer
                        if (NPC.ai[2] == 2)
                        {
                            int countDripplers = 0;
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<LoomingDrippler>())
                                {
                                    countDripplers++;
                                }
                            }

                            NPC.velocity = Vector2.Zero;

                            if (NPC.alpha < 255)
                            {
                                if (NPC.alpha > 255)
                                {
                                    NPC.alpha = 255;
                                }
                                else
                                {
                                    NPC.alpha += 3;
                                }
                            }

                            if (NPC.alpha >= 255)
                            {
                                if (OvermorrowWorld.DripplerCircle)
                                {
                                    NPC.ai[2] = 3;
                                    ((DripplerBoss)Main.npc[(int)NPC.ai[1]].ModNPC).turnonattackindicator = true;
                                    NPC.ai[0] = 0;
                                    NPC.ai[3] = Main.rand.Next(0, 360); // rotation counter
                                    origin = player.Center;
                                }
                                else
                                {
                                    NPC.ai[2] = 4;
                                    NPC.ai[0] = 0;
                                }
                            }
                        }
                        break;
                    case 3: // Rotate around the player
                        NPC_OrbitPosition(NPC, origin, 300, 1f);

                        if (NPC.alpha > 0)
                        {
                            NPC.alpha -= 3;
                        }

                        if (NPC.ai[0] >= 395)
                        {
                            NPC.alpha += 3;
                        }

                        if (NPC.ai[0] == 480)
                        {
                            OvermorrowWorld.DripplerCircle = false;
                            NPC.alpha = 0;
                            NPC.ai[2] = 2; // switch
                            NPC.ai[0] = 0;
                            NPC.ai[3] = 0;
                        }
                        break;
                    case 4: // Teleport to a random position
                        if (OvermorrowWorld.DripladShoot)
                        {
                            storedDamage = NPC.damage;
                            NPC.ai[2] = -1;
                            NPC.ai[0] = 0;
                            break;
                        }

                        if (NPC.ai[0] == 1)
                        {
                            randIncrementer = Main.rand.Next(2, 5);
                            Vector2 randPos = new Vector2(player.Center.X + Main.rand.Next(-6, 6) * 100, player.Center.Y + Main.rand.Next(-6, 6) * 100);
                            while (Main.tile[(int)randPos.X / 16, (int)randPos.Y / 16].HasTile)
                            {
                                randPos = new Vector2(player.Center.X + Main.rand.Next(-6, 6) * 100, player.Center.Y + Main.rand.Next(-6, 6) * 100);
                            }
                            NPC.position = randPos;
                            NPC.netUpdate = true;
                        }
                        if (OvermorrowWorld.loomingdripplerdeadcount <= 6)
                        {
                            NPC.velocity = Vector2.Zero;
                        }
                        if (NPC.alpha > 0)
                        {
                            NPC.alpha -= randIncrementer;
                        }

                        if (NPC.alpha <= 0 && OvermorrowWorld.loomingdripplerdeadcount <= 6)
                        {
                            int shootSpeed = Main.rand.Next(6, 10);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (Main.rand.NextBool(2))
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.DirectionTo(Main.player[NPC.target].Center) * shootSpeed, ModContent.ProjectileType<BloodyBall>(), NPC.damage / /*3*/ 2, 3f, Main.myPlayer, 0, 0);
                                }
                            }

                            if (!secondTeleport)
                            {
                                secondTeleport = true;
                                NPC.ai[2] = 2;
                                NPC.ai[0] = 0;
                            }
                            else
                            {
                                secondTeleport = false;
                                randSwitch = Main.rand.Next(300, 700);
                                NPC.ai[2] = 0;
                                NPC.ai[0] = 0;
                            }
                        }
                        else if (NPC.alpha <= 0 && OvermorrowWorld.loomingdripplerdeadcount > 6)
                        {
                            Vector2 targetPosition = Main.player[NPC.target].Center;
                            if (++attackCounter == 30)
                            {
                                Vector2 origin = NPC.Center;
                                float radius = 45;
                                int numLocations = 30;
                                for (int i = 0; i < 30; i++)
                                {
                                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                                    Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                                    int dust = Dust.NewDust(position, 2, 2, DustID.HeartCrystal, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                    Main.dust[dust].noGravity = true;
                                }
                                NPC.velocity = 10f * NPC.DirectionTo(new Vector2(Main.rand.NextFloat(targetPosition.X - 25, targetPosition.X + 25), Main.rand.NextFloat(targetPosition.Y - 25, targetPosition.Y + 25)));
                            }
                            else if (attackCounter > 60 && attackCounter < 120)
                            {
                                NPC.velocity = new Vector2(MathHelper.Lerp(NPC.velocity.X, 0, 0.025f), MathHelper.Lerp(NPC.velocity.Y, 0, 0.025f));
                            }

                            if (attackCounter > 120)
                            {
                                if (!secondTeleport)
                                {
                                    secondTeleport = true;
                                    NPC.ai[2] = 2;
                                    NPC.ai[0] = 0;
                                    attackCounter = 0;
                                    NPC.velocity = Vector2.Zero;
                                }
                                else
                                {
                                    secondTeleport = false;
                                    randSwitch = Main.rand.Next(300, 700);
                                    NPC.ai[2] = 0;
                                    NPC.ai[0] = 0;
                                    attackCounter = 0;
                                    NPC.velocity = Vector2.Zero;
                                }
                            }
                        }
                        break;
                }
            }
        }

        public void NPC_OrbitPosition(NPC modNPC, Vector2 position, double distance, double speed = 1.75)
        {
            double rad;
            double deg = speed * (double)NPC.ai[3];
            rad = deg * (Math.PI / 180);
            NPC.ai[3] += 2f;

            NPC.position.X = position.X - (int)(Math.Cos(rad) * distance) - NPC.width / 2;
            NPC.position.Y = position.Y - (int)(Math.Sin(rad) * distance) - NPC.height / 2;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life > 0)
            {
                for (int num826 = 0; (double)num826 < 10 / (double)NPC.lifeMax * 100.0; num826++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f);
                }
                return;
            }
            for (int num827 = 0; num827 < 50; num827++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 2.5f * (float)hitDirection, -2.5f);
            }
            var source = NPC.GetSource_OnHit(NPC);

            Gore.NewGore(source, NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/Looming1").Type, NPC.scale);
            Gore.NewGore(source, NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/Looming2").Type, NPC.scale);
            Gore.NewGore(source, NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/Looming3").Type, NPC.scale);
            Gore.NewGore(source, NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/Looming" + (Main.rand.Next(1, 4)).ToString()).Type, NPC.scale);
            Gore.NewGore(source, NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/Looming" + (Main.rand.Next(1, 4)).ToString()).Type, NPC.scale);
        }

        public override void FindFrame(int frameHeight)
        {
            int num = TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type];

            NPC.rotation = NPC.velocity.X * 0.15f;
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter > 6.0)
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y += num;
            }
            if (NPC.frame.Y >= num * Main.npcFrameCount[NPC.type])
            {
                NPC.frame.Y = 0;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.alpha == 0)
            {
                Texture2D texture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/DripplerBoss/LoomingDrippler_Glowmask").Value;
                spriteBatch.Draw(texture, new Vector2(NPC.Center.X - screenPos.X, NPC.Center.Y - screenPos.Y - 4), NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
        }

        public override void OnKill()
        {
            OvermorrowWorld.loomingdripplerdeadcount += 1;
            if (Main.npc[(int)NPC.ai[1]].active && Main.npc[(int)NPC.ai[1]].boss)
            {
                if (Main.npc[(int)NPC.ai[1]].active && Main.npc[(int)NPC.ai[1]].boss)
                {
                    NPC parentNPC = Main.npc[(int)NPC.ai[1]];
                    if (Main.expertMode)
                    {
                        parentNPC.life -= 200;//400;
                    }
                    else
                    {
                        parentNPC.life -= 100;//200;
                    }
                }
            }
        }
    }
}