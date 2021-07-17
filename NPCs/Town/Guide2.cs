using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Projectiles.Boss;
using OvermorrowMod.Items.Placeable.Boss;
using OvermorrowMod.Items.Weapons.PreHardmode.Melee;
using OvermorrowMod.Items.Weapons.PreHardmode.Magic;
using OvermorrowMod.Items.Weapons.PreHardmode.Ranged;
using OvermorrowMod.WardenClass.Weapons.Artifacts;
using OvermorrowMod.Items.Weapons.PreHardmode.Summoner;
using OvermorrowMod.Items.BossBags;
using OvermorrowMod.NPCs.PostRider.NightCrawler;

namespace OvermorrowMod.NPCs.Town
{

    public class Guide2 : ModNPC
    {
        private bool changedPhase2 = false;
        int bulltimer;
        int arrowtimer;
        int magictimer;


        bool Lamp = false;
        bool TwinANI = false;
        int frame = 0;
        int SummStopper = 0;
        int disctimer;
        bool leafatt = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enraged Guide");
            Main.npcFrameCount[npc.type] = 24;
        }

        public override void SetDefaults()
        {
            // Afterimage effect
            NPCID.Sets.TrailCacheLength[npc.type] = 7;
            NPCID.Sets.TrailingMode[npc.type] = 1;

            // Reduced size
            npc.width = 86;
            npc.height = 58;

            // Actual dimensions
            //npc.width = 368;
            //npc.height = 338;

            npc.aiStyle = -1;
            npc.damage = 26;
            npc.defense = 14;
            npc.lifeMax = 3300;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.Item25;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.boss = true;
            npc.value = Item.buyPrice(gold: 3);
            npc.npcSlots = 10f;
            //music = MusicID.Boss5;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/TreeBoss");
            bossBag = ModContent.ItemType<TreeBag>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale * 0.65f);
            npc.defense = 17;
        }

        private void BossText(string text) // boss messages
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(text, Color.Green);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(text), Color.Green);
            }
        }

        int RandomCase = 0;
        int LastCase = 0;
        int RandomCeiling;
        bool movement = true;

        public override void AI()
        {
            SummStopper--;
            // Death animation code
            if (npc.ai[3] > 0f)
            {
                npc.velocity = Vector2.Zero;

                if (npc.ai[2] > 0)
                {
                    npc.ai[2]--;

                    if (npc.ai[2] == 480)
                    {
                        BossText(":(");
                    }

                    if (npc.ai[2] == 300)
                    {
                        BossText(">:(");
                    }

                    if (npc.ai[2] == 120)
                    {
                        BossText("Not cool");
                    }
                }
                else
                {
                    npc.dontTakeDamage = true;
                    npc.ai[3]++; // Death timer
                    npc.velocity.X *= 0.95f;

                    if (npc.velocity.Y < 0.5f)
                    {
                        npc.velocity.Y = npc.velocity.Y + 0.01f;
                    }

                    if (npc.velocity.X > 0.5f)
                    {
                        npc.velocity.Y = npc.velocity.Y - 0.01f;
                    }

                    if (npc.ai[3] > 120f)
                    {
                        npc.Opacity = 1f - (npc.ai[3] - 120f) / 60f;
                    }

                    if (Main.rand.NextBool(5) && npc.ai[3] < 120f)
                    {
                        // This dust spawn adapted from the Pillar death code in vanilla.
                        for (int dustNumber = 0; dustNumber < 6; dustNumber++)
                        {
                            Dust dust = Main.dust[Dust.NewDust(npc.Left, npc.width, npc.height / 2, 107, 0f, 0f, 0, default(Color), 1f)];
                            dust.position = npc.Center + Vector2.UnitY.RotatedByRandom(4.1887903213500977) * new Vector2(npc.width * 1.5f, npc.height * 1.1f) * 0.8f * (0.8f + Main.rand.NextFloat() * 0.2f);
                            dust.velocity.X = 0f;
                            dust.velocity.Y = -Math.Abs(dust.velocity.Y - (float)dustNumber + npc.velocity.Y - 4f) * 3f;
                            dust.noGravity = true;
                            dust.fadeIn = 1f;
                            dust.scale = 1f + Main.rand.NextFloat() + (float)dustNumber * 0.3f;
                        }
                    }

                    if (npc.ai[3] % 30f == 1f)
                    {
                        //Main.PlaySound(4, npc.Center, 22);
                        Main.PlaySound(SoundID.Item25, npc.Center); // every half second while dying, play a sound
                    }

                    if (npc.ai[3] >= 180f)
                    {
                        npc.life = 0;
                        npc.HitEffect(0, 0);
                        npc.checkDead(); // This will trigger ModNPC.CheckDead the second time, causing the real death.
                    }
                }
                return;
            }

            npc.TargetClosest(true);
            Player player = Main.player[npc.target];

            npc.spriteDirection = npc.direction;

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

            if (npc.life > npc.lifeMax)
            {
                npc.life = npc.lifeMax;
            }

            npc.ai[1]++;

            // Moveset
            // Throw scythes
            // Throw a wall of scythes, repeat this and top twice
            // Charge 3 times
            // Spawn nature waves in all directions
            // Move toward player slowly

            if (npc.life <= npc.lifeMax * 0.5f)
            {
                changedPhase2 = true;
            }

            switch (npc.ai[0])
            {
                case -1: // case switching
                    {
                        if (movement == true)
                        {
                            if (changedPhase2 == true) { RandomCeiling = 6; }
                            else { RandomCeiling = 6; }
                            while (RandomCase == LastCase)
                            {
                                RandomCase = Main.rand.Next(1, RandomCeiling);
                            }
                            LastCase = RandomCase;
                            movement = false;
                            npc.ai[0] = RandomCase;
                        }
                        else
                        {
                            movement = true;
                            npc.ai[0] = 0;
                        }
                    }
                    break;
                case 0: // Follow player
                    if (npc.ai[0] == 0)
                    {
                        Vector2 moveTo = player.Center;
                        var move = moveTo - npc.Center;
                        var speed = 5;

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
                        npc.velocity.X = move.X;
                        npc.velocity.Y = move.Y * .98f;

                        if (npc.ai[1] > (changedPhase2 ? 90 : 120))
                        {
                            npc.ai[0] = -1;
                            npc.ai[1] = 0;
                        }
                    }
                    break;
                case 1: // Shoot scythes
                    if (npc.ai[0] == 1)
                    {
                        if(SummStopper < 1)
                        {

                            if(Vector2.Distance(npc.Center, Main.player[npc.target].Center) < 350f)
                            {
                                Vector2 GuidePos4 = npc.Center;
                                Vector2 PlayerPosition4 = Main.player[npc.target].Center;
                                Vector2 GuideDirection4 = PlayerPosition4 - GuidePos4;
                                GuideDirection4.Normalize();
                                npc.velocity += GuideDirection4 * -0.06f;  
                            }
                            else
                            {
                                Vector2 GuidePos5 = npc.Center;
                                Vector2 PlayerPosition5 = Main.player[npc.target].Center;
                                Vector2 GuideDirection5 = PlayerPosition5 - GuidePos5;
                                GuideDirection5.Normalize();
                                npc.velocity += GuideDirection5 * 0.08f;  
                            }

                            if (npc.ai[1] == 100)
                            {
                                NPC.NewNPC((int)npc.Center.X , (int)npc.Center.Y, mod.NPCType("Twin1NPC"));
                                NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, mod.NPCType("Twin2NPC"));
                                TwinANI = true;
                                frame = 17;
                            }

                            if (npc.ai[1] == 200)
                            {
                                NPC.NewNPC((int)npc.Center.X , (int)npc.Center.Y, mod.NPCType("Twin1NPC"));
                                NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, mod.NPCType("Twin2NPC"));
                                TwinANI = true;
                                frame = 17;
                            }

                            if (npc.ai[1] == 300)
                            {
                                NPC.NewNPC((int)npc.Center.X , (int)npc.Center.Y, mod.NPCType("Twin1NPC"));
                                NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, mod.NPCType("Twin2NPC"));
                                TwinANI = true;
                                frame = 17;
                            }



                            if(npc.velocity.Y < -9f)
                            {
                                npc.velocity.Y = -9f;
                            }

                            if(npc.velocity.Y > 9f)
                            {
                                npc.velocity.Y = 9f;
                            }


                            if(npc.velocity.X < -9f)
                            {
                                npc.velocity.X = -9f;
                            }

                            if(npc.velocity.X > 9f)
                            {
                                npc.velocity.X = 9f;
                            }



                            if (npc.ai[1] > 400)
                            {
                                SummStopper = 2000;
                                npc.ai[0] = -1;
                                npc.ai[1] = 0;
                            }
                        }
                        else
                        {
                            npc.ai[0] = -1;
                            npc.ai[1] = 0; 
                        }
                    }
                    break;
                case 2: // Absorb energy
                    if (npc.ai[0] == 2)
                    {
                        Vector2 moveTo3 = player.Center;
                        moveTo3.X += 50 * (npc.Center.X < moveTo3.X ? -1 : 1);
                        var move3 = moveTo3 - npc.Center;
                        var speed3 = 1;

                        float length = move3.Length();
                        if (length > speed3)
                        {
                            move3 *= speed3 / length;
                        }
                        var turnResistance = 45;
                        move3 = (npc.velocity * turnResistance + move3) / (turnResistance + 1f);
                        length = move3.Length();
                        if (length > 10)
                        {
                            move3 *= speed3 / length;
                        }
                        npc.velocity.X = move3.X;
                        npc.velocity.Y = move3.Y * .98f;



                        bulltimer++;
                        arrowtimer++;
                        if(npc.ai[1] < 100)
                        {
                            if (bulltimer > 9)
                            {
                                Vector2 position = npc.Center;
                                Vector2 targetPosition = Main.player[npc.target].Center;
                                Vector2 direction = targetPosition - position;
                                direction.Normalize();
                                float speed = 9f;
                                int proj = Projectile.NewProjectile(position, direction * speed, 180, npc.damage, 0f, Main.myPlayer);  
                                bulltimer = 0;    
                            }
                        }
                        if(npc.ai[1] > 99)
                        {
                            if (arrowtimer > 9)
                            {
                                Vector2 position = npc.Center + new Vector2(0, -1000);
                                Vector2 targetPosition = Main.player[npc.target].Center;
                                Vector2 direction = targetPosition - position;
                                direction.Normalize();
                                float speed = 50f;
                                Vector2 Rot1 = new Vector2(direction.X,  direction.Y).RotatedByRandom(MathHelper.ToRadians(5));
                                Vector2 Rot2 = new Vector2(direction.X,  direction.Y).RotatedByRandom(MathHelper.ToRadians(5));
                                Vector2 Rot3 = new Vector2(direction.X,  direction.Y).RotatedByRandom(MathHelper.ToRadians(5));
                                Projectile.NewProjectile(position, Rot1 * speed, 82, npc.damage, 0f, Main.myPlayer);  
                                Projectile.NewProjectile(position, Rot2 * speed, 82, npc.damage, 0f, Main.myPlayer);  
                                Projectile.NewProjectile(position, Rot3 * speed, 82, npc.damage, 0f, Main.myPlayer);  
                                arrowtimer = 0;    
                            }
                        }




                        if (npc.ai[1] > 200)
                        {
                            npc.ai[0] = -1;
                            npc.ai[1] = 0;
                        }
                    }
                    break;
                case 5: // Shoot nature blasts

                    if (npc.ai[0] == 5)
                    {

                        int RandomX = Main.rand.Next(-100, 100);
                        int RandomY = Main.rand.Next(-100, 100);

                        magictimer++;

                        if(Vector2.Distance(npc.Center, Main.player[npc.target].Center) < 310f)
                        {
                            if(magictimer > 7)
                            {
                                Vector2 GuidePos2 = npc.Center;
                                Vector2 PlayerPosition2 = Main.player[npc.target].Center;
                                Vector2 GuideDirection2 = PlayerPosition2 - GuidePos2;
                                GuideDirection2.Normalize();
                                npc.velocity += GuideDirection2 * -0.07f;  

                                Vector2 position = npc.Center;
                                Vector2 targetPosition = Main.player[npc.target].Center;
                                Vector2 direction = targetPosition - position;
                                direction.Normalize();
                                float speed = 12f;
                                int dagger = Projectile.NewProjectile(position, direction * speed, 93, npc.damage, 0f, Main.myPlayer);  
                                Main.projectile[dagger].friendly = false;
                                Main.projectile[dagger].hostile = true;
                                Lamp = false;
                                frame = 0;
                                magictimer = 0;
                            }
                        }
                        else
                        {
                            Vector2 GuidePos3 = npc.Center;
                            Vector2 PlayerPosition3 = Main.player[npc.target].Center;
                            Vector2 GuideDirection3 = PlayerPosition3 - GuidePos3;
                            GuideDirection3.Normalize();
                            npc.velocity += GuideDirection3 * 0.07f;  

                            if(magictimer > 27)
                            {
                                Vector2 position = npc.Center;
                                Vector2 targetPosition = Main.player[npc.target].Center;
                                Vector2 direction = targetPosition - position;
                                direction.Normalize();
                                float speed = 4f;
                                int spirtflame = Projectile.NewProjectile(npc.Center.X + RandomX, npc.Center.Y + RandomY, direction.X * speed, direction.Y * speed, ModContent.ProjectileType<GuideFlame>(), npc.damage, 0f, Main.myPlayer);
                                Main.projectile[spirtflame].timeLeft = 330;
                                Lamp = true;
                                magictimer = 0;
                            }
                        }





                        if(npc.velocity.Y < -7.8f)
                        {
                            npc.velocity.Y = -7.8f;
                        }

                        if(npc.velocity.Y > 7.8f)
                        {
                            npc.velocity.Y = 7.8f;
                        }


                        if(npc.velocity.X < -7.8f)
                        {
                            npc.velocity.X = -7.8f;
                        }

                        if(npc.velocity.X > 7.8f)
                        {
                            npc.velocity.X = 7.8f;
                        }



                        if (npc.ai[1] > 350)
                        {
                            npc.ai[0] = -1;
                            npc.ai[1] = 0;
                        }
                    }
                    break;
                case 4: // Shoot nature blasts
                    if (npc.ai[0] == 4)
                    {   
                        
                        if (npc.ai[1] == 3)
                        {
                            Vector2 position = npc.Center;
                            Vector2 targetPosition = Main.player[npc.target].Center;
                            Vector2 direction = targetPosition - position;
                            direction.Normalize();
                            npc.velocity = direction * 15f;
                        }


                        if (npc.ai[1] > 30)
                        {
                            npc.ai[0] =  -1;
                            npc.ai[1] = 0;
                        }
                    }
                    break;
                case 3: // scythes
                    {
                        
                        Vector2 GuidePosition = npc.Center;
                        Vector2 PlayerPosition = Main.player[npc.target].Center;
                        Vector2 GuideDirection = PlayerPosition - GuidePosition;
                        GuideDirection.Normalize();
                        npc.velocity = GuideDirection * 5f;



                        disctimer++;
                        if(disctimer > 14)
                        {
                            disctimer = 0;
                        }

                        if(disctimer == 13 && Main.player[npc.target].ownedProjectileCounts[ModContent.ProjectileType<LightDisc2>()] < 5)
                        {
                            Vector2 position = npc.Center;
                            Vector2 targetPosition = Main.player[npc.target].Center;
                            Vector2 direction = targetPosition - position;
                            direction.Normalize();
                            float Projspeed = 15f;
                            Projectile.NewProjectile(position, direction * Projspeed, ModContent.ProjectileType<LightDisc2>(), npc.damage, 0f, Main.myPlayer);  
                            frame = 0;
                        }

                        if (npc.ai[1] > 540)
                        {
                            npc.ai[0] = 0;
                            npc.ai[1] = 0;
                        }
                        break;
                    }
            }
        }

        public override void FindFrame(int frameHeight)
        {

            npc.rotation = npc.velocity.X * 0.015f;
            npc.frame.Y = frameHeight * frame;

            if(npc.ai[0] == 0)
            {
                npc.frameCounter++;
                if (npc.frameCounter > 4) // Ticks per frame
                {
                    npc.frameCounter = 0;
                    frame += 1;
                }
                if (frame >= 8) // 6 is max # of frames
                {
                    frame = 4; // Reset back to default
                }
               if (frame < 4) // 6 is max # of frames
                {
                    frame = 4; // Reset back to default
                }
            }



            if(npc.ai[0] == 1)
            {
                if(!TwinANI)
                {
                    npc.frameCounter++;
                    if (npc.frameCounter > 4) // Ticks per frame
                    {
                        npc.frameCounter = 0;
                        frame += 1;
                    }
                    if (frame >= 8) // 6 is max # of frames
                    {
                        frame = 4; // Reset back to default
                    }
                    if (frame < 4) // 6 is max # of frames
                    {
                        frame = 4; // Reset back to default
                    }
                }
                else
                {
                    npc.frameCounter++;
                    if (npc.frameCounter > 7) // Ticks per frame
                    {
                        npc.frameCounter = 0;
                        frame += 1;
                    }
                    if(frame >= 20)
                    {
                        frame = 4;
                        TwinANI = false;
                    }
                }
            }



            if(npc.ai[0] == 2)
            {
                npc.frameCounter++;
                if(npc.ai[1] < 100)
                {

                    if (npc.frameCounter > 4) // Ticks per frame
                    {
                        npc.frameCounter = 0;
                        frame += 1;
                    }
                    if (frame >= 12) // 6 is max # of frames
                    {
                        frame = 9; // Reset back to default
                    }
                    if (frame < 9) // 6 is max # of frames
                    {
                        frame = 9; // Reset back to default
                    }
                }
                else
                {
                    if (npc.frameCounter > 4) // Ticks per frame
                    {
                        npc.frameCounter = 0;
                        frame += 1;
                    }
                    if (frame >= 16) // 6 is max # of frames
                    {
                        frame = 13; // Reset back to default
                    }
                    if (frame < 13) // 6 is max # of frames
                    {
                        frame = 13; // Reset back to default
                    }
                }
            }


            if(npc.ai[0] == 3)
            {
                npc.frameCounter++;
                if (npc.frameCounter > 4) // Ticks per frame
                {
                    npc.frameCounter = 0;
                    frame += 1;
                }
                if (frame >= 8) // 6 is max # of frames
                {
                    frame = 4; // Reset back to default
                }
            }






            if(npc.ai[0] == 5)
            {
                if(!Lamp)
                {
                    npc.frameCounter++;
                    if (npc.frameCounter > 4) // Ticks per frame
                    {
                        npc.frameCounter = 0;
                        frame += 1;
                    }
                    if (frame >= 8) // 6 is max # of frames
                    {
                        frame = 4; // Reset back to default
                    }
                }
                else
                {
                    npc.frameCounter++;
                    if (npc.frameCounter > 4) // Ticks per frame
                    {
                        npc.frameCounter = 0;
                        frame += 1;
                    }
                    if (frame >= 24) // 6 is max # of frames
                    {
                        frame = 21; // Reset back to default
                    }
                    if (frame < 21) // 6 is max # of frames
                    {
                        frame = 21; // Reset back to default
                    }
                }
            }
        }
        
        

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Vector2 drawOrigin = new Vector2(Main.npcTexture[npc.type].Width * 0.5f, npc.height * 0.5f);
            for (int k = 0; k < npc.oldPos.Length; k++)
            {
                // Adjust drawPos if the hitbox does not match sprite dimension
                Vector2 drawPos = npc.oldPos[k] - Main.screenPosition + drawOrigin;
                Color afterImageColor = npc.life <= npc.lifeMax * 0.5 ? Color.Red : Color.LightGreen;
                Color color = npc.GetAlpha(afterImageColor) * ((float)(npc.oldPos.Length - k) / (float)npc.oldPos.Length);
                spriteBatch.Draw(Main.npcTexture[npc.type], drawPos, npc.frame, color, npc.rotation, drawOrigin, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }
            return base.PreDraw(spriteBatch, drawColor);
        }

        public override bool CheckDead()
        {
            if (npc.ai[3] == 0f)
            {
                npc.ai[2] = OvermorrowWorld.downedTree ? 0 : 540;
                npc.ai[3] = 1f;
                npc.damage = 0;
                npc.life = npc.lifeMax;
                npc.dontTakeDamage = true;
                npc.netUpdate = true;
                return false;
            }
            return true;
        }

        public override void NPCLoot()
        {
            OvermorrowWorld.downedTree = true;

            if (Main.expertMode)
            {
                npc.DropBossBags();
            }
            else
            {
                int choice = Main.rand.Next(5);
                // Always drops one of:
                if (choice == 0) // Warden
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<EarthCrystal>());
                }
                else if (choice == 1) // Mage
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<IorichStaff>());
                }
                if (choice == 2) // Warrior
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<IorichHarvester>());
                }
                else if (choice == 3) // Ranger
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<IorichBow>());
                }
                else if (choice == 4) // Summoner
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<IorichWand>());
                }

                if (Main.rand.Next(10) == 0) // Trophy Dropchance
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<TreeTrophy>());
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }
    }
}