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
    public class DripplerBoss : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dripplord, the Bloody Assimilator");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.width = 320;
            npc.height = 482;
            npc.aiStyle = -1;
            npc.damage = 31;
            npc.defense = 35;
            npc.lifeMax = 6500;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.DD2_BetsyDeath;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.boss = true;
            npc.value = Item.buyPrice(gold: 2, silver: 50);
            npc.npcSlots = 10f;
            music = MusicID.Boss2;
            //music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/StormDrake");
            //bossBag = ModContent.ItemType<DrakeBag>();

            npc.buffImmune[BuffID.Bleeding] = true;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.Frostburn] = true;
            npc.buffImmune[BuffID.CursedInferno] = true;
            npc.buffImmune[BuffID.WeaponImbueCursedFlames] = true;
            npc.buffImmune[BuffID.ShadowFlame] = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale);
            npc.damage = (int)(npc.damage * 1.2f);
            npc.defense = 50;
        }

        public override void AI()
        {
            if(npc.life <= 0)
            {
                npc.NPCLoot();
            }

            Player player = Main.player[npc.target];
            //npc.dontTakeDamage = true;
            
            // Check that it is a Blood Moon & that it is night time
            if (!Main.bloodMoon)
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

            // Handles Despawning
            if (npc.target < 0 || npc.target == 255 || player.dead || !player.active)
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

            if (!player.active || player.dead)
            {
                npc.TargetClosest(false);
                npc.velocity.Y = -2000;
            }

            // Inflicts 'Congealed Blood' debuff that slows down player movement

            // General MOVESET:
            // Float towards the player
            // Throw blood at the player
            // Throw blood orbs that shoot into 4 directions
            // Pauses and roars, summoning Dripplers from offscreen
            // Summon Greater Dripplers that hover above player and rain down blood

            // PHASE 2:
            // Spawn Dripplers in all directions
            // Shoot lasers in all directions from the central eye
            // Expert Mode: Throw blood orbs that linger on the screen

            // Dripplers that die damage the boss

            switch (npc.ai[0])
            {
                case 0: // Float towards the player
                    if (npc.ai[0] == 0)
                    {
                        Vector2 moveTo = player.Center;
                        var move = moveTo - npc.Center;
                        var speed = 2;

                        float length = move.Length();
                        if (length > speed)
                        {
                            move *= speed / length;
                        }
                        var turnResistance = 45;
                        move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
                        length = move.Length();
                        if (length > 10)
                        {
                            move *= speed / length;
                        }
                        npc.velocity = move;

                        if(npc.life <= npc.lifeMax * 0.5f)
                        {
                            if(npc.ai[1] % 100 == 0)
                            {
                                // I'm lazy
                                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, -10f, 0f, ModContent.ProjectileType<SplittingBlood>(), npc.damage / 2, 2f, Main.myPlayer, 0f, 0f);
                                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0f, 10f, ModContent.ProjectileType<SplittingBlood>(), npc.damage / 2, 2f, Main.myPlayer, 0f, 0f);
                                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 10f, 0f, ModContent.ProjectileType<SplittingBlood>(), npc.damage / 2, 2f, Main.myPlayer, 0f, 0f);
                                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0f, -10f, ModContent.ProjectileType<SplittingBlood>(), npc.damage / 2, 2f, Main.myPlayer, 0f, 0f);
                            }
                        }

                        // Check number of spawned NPCs before switching phase
                        int countDripplers = 0;
                        int countDriplads = 0;
                        for(int i = 0; i < Main.maxNPCs; i++)
                        {
                            if(Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<LoomingDrippler>())
                            {
                                countDripplers++;
                            }

                            if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<Driplad>())
                            {
                                countDriplads++;
                            }
                        }

                        if (npc.ai[1] == 270)
                        {
                            if (npc.life <= npc.lifeMax * 0.66f && countDriplads <= 0) // Spawn driplads
                            {
                                npc.ai[0] = 2;
                                npc.ai[1] = 0;
                            }
                            else if (countDripplers <= 0) // Spawn dripplers
                            {
                                npc.ai[0] = 3;
                                npc.ai[1] = 0;
                            }
                            else // Follow player
                            {
                                npc.ai[0] = 0;
                                npc.ai[1] = 0;
                            }
                        } 
                    }
                    break;
                case 2: // Spawn Driplads
                    if (npc.ai[0] == 2)
                    {
                        npc.velocity = Vector2.Zero;

                        if (npc.ai[1] == 90)
                        {
                            // Wall of Flesh scream sound
                            Main.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.NPCKilled, 10), (int)npc.Center.X, (int)npc.Center.Y);
                            //float rotation = MathHelper.ToRadians(360);

                            Vector2 origin = npc.Center; // Origin of the circle
                            float radius = 750; // Distance from the circle
                            int numSpawns = 5; // Points spawned on the circle
                            for (int i = 0; i < 5; i++)
                            {
                                Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numSpawns * i)) * radius;
                                //NPC.NewNPC((int)(position.X), (int)(position.Y), NPCID.Drippler);
                                Projectile.NewProjectile(position.X, position.Y, 0, 0, ModContent.ProjectileType<DripplerSpawner>(), 0, 0f, Main.myPlayer, 1, npc.whoAmI);
                            }
                        }

                        if (npc.ai[1] == 270)
                        {
                            npc.ai[0] = 0;
                            npc.ai[1] = 0;
                        }

                        /*if (npc.ai[1] % 90 == 0)
                        {
                            /*float numberProjectiles = 5 + Main.rand.Next(4); // This defines how many projectiles to shot
                            float rotation = MathHelper.ToRadians(30);
                            Vector2 position = npc.Center;
                            int speedX = -180 + (int)npc.ai[1];
                            int speedY = Main.rand.Next(-160, -80);
                            position += Vector2.Normalize(new Vector2(speedX, speedY)) * 20; //this defines the distance of the projectiles form the player when the projectile spawns
                            for (int i = 0; i < numberProjectiles; i++)
                            {
                                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .1f; // This defines the projectile roatation and speed. .4f == projectile speed
                                Projectile.NewProjectile(npc.Center.X, npc.Center.Y - 170, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<BloodyBall>(), npc.damage, 2f, Main.myPlayer);
                            }
                            float numberProjectiles = 5 + Main.rand.Next(3); // This defines how many projectiles to shot
                            float rotation = MathHelper.ToRadians(7);
                            Vector2 position = npc.Center;
                            float speedX = player.Center.X - position.X;
                            float speedY = player.Center.Y - position.Y;

                            float length = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
                            float num12 = 15 / length;
                            speedX = speedX * num12;
                            speedY = speedY * num12;
                            //int speedX = 2;
                            //int speedY = Main.rand.Next(-160, -80);
                            position += Vector2.Normalize(new Vector2(speedX, speedY)) * 30; //this defines the distance of the projectiles form the player when the projectile spawns
                            for (int i = 0; i < numberProjectiles; i++)
                            {
                                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .5f; // This defines the projectile roatation and speed. .4f == projectile speed
                                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<BloodyBall>(), npc.damage, 2f, Main.myPlayer);
                            }
                        }

                        if(npc.ai[1] == 270)
                        {
                            npc.ai[0] = 0;
                            npc.ai[1] = 0;
                        }*/
                    }
                    break;
                case 3: // Summon Dripplers
                    if (npc.ai[0] == 3)
                    {

                        npc.velocity = Vector2.Zero;

                        if(npc.ai[1] == 90)
                        {
                            // Wall of Flesh scream sound
                            Main.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.NPCKilled, 10), (int)npc.Center.X, (int)npc.Center.Y);
                            //float rotation = MathHelper.ToRadians(360);

                            Vector2 origin = npc.Center; // Origin of the circle
                            float radius = 975; // Distance from the circle
                            int numSpawns = 12; // Points spawned on the circle
                            for (int i = 0; i < 12; i++)
                            {
                                Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numSpawns * i)) * radius;
                                //NPC.NewNPC((int)(position.X), (int)(position.Y), NPCID.Drippler);
                                Projectile.NewProjectile(position.X, position.Y, 0, 0, ModContent.ProjectileType<DripplerSpawner>(), 0, 0f, Main.myPlayer, 0, npc.whoAmI);
                            }
                        }

                        if (npc.ai[1] == 270)
                        {
                            npc.ai[0] = 0;
                            npc.ai[1] = 0;
                        }
                    }
                    break;
            }

            npc.ai[1]++;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            /*if (npc.life > 0)
            {
                int randSpawn = Main.rand.Next(10);
                if (randSpawn == 0)
                {
                    NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.Drippler);
                }
            }*/
            base.HitEffect(hitDirection, damage);
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage *= 0;
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;

            if (npc.frameCounter % 6f == 5f)
            {
                npc.frame.Y += frameHeight;
            }
            if (npc.frame.Y >= frameHeight * 6) // 6 is max # of frames
            {
                npc.frame.Y = 0; // Reset back to default
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = mod.GetTexture("NPCs/Bosses/DripplerBoss/DripplerBoss_Glowmask");
            spriteBatch.Draw(texture, new Vector2(npc.Center.X - Main.screenPosition.X, npc.Center.Y - Main.screenPosition.Y + 4), npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void NPCLoot()
        {
            int choice = Main.rand.Next(1);
            if(choice == 0) // Warden
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<BloodyAntikythera>());
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }
    }
}