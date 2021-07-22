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

namespace OvermorrowMod.NPCs.Bosses.Goblin
{
    public class GoblinMiniBoss : ModNPC
    {
        private bool changedPhase2 = false;
        private int StopHeal = 0;

        bool leafatt = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Goblin MiniBoss");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            // Afterimage effect
            NPCID.Sets.TrailCacheLength[npc.type] = 7;
            NPCID.Sets.TrailingMode[npc.type] = 1;

            // Reduced size
            npc.width = 24;
            npc.height = 56;

            // Actual dimensions
            //npc.width = 368;
            //npc.height = 338;

            npc.aiStyle = -1;
            npc.damage = 26;
            npc.defense = 3;
            npc.lifeMax = 1000;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.Item25;
            npc.knockBackResist = 0f;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.boss = false;
            npc.value = Item.buyPrice(gold: 3);
            npc.npcSlots = 10f;
            bossBag = ModContent.ItemType<TreeBag>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale * 0.65f);
            npc.defense = 4;
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
            StopHeal--;
            // Death animation code
            if (npc.ai[3] > 0f)
            {
                npc.velocity = Vector2.Zero;

                if (npc.ai[2] > 0)
                {
                    npc.ai[2]--;

                    if (npc.ai[2] == 480)
                    {
                        BossText("I deem thee fit to inherit their powers.");
                    }

                    if (npc.ai[2] == 300)
                    {
                        BossText("Thou Dryad shalt guide thee.");
                    }

                    if (npc.ai[2] == 120)
                    {
                        BossText("Fare thee well.");
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


                            if (changedPhase2 == true) { RandomCeiling = 5; }
                            else { RandomCeiling = 5; }
                            while (RandomCase == LastCase)
                            {
                                RandomCase = Main.rand.Next(1, RandomCeiling);
                            }
                            LastCase = RandomCase;
                            movement = false;
                            npc.ai[0] = RandomCase;


                    }
                    break;
                case 0: // Follow player
                    if (npc.ai[0] == 0)
                    {
                        if (npc.ai[1] > 30)
                        {
                            npc.ai[0] = -1;
                            npc.ai[1] = 0;
                        }
                    }
                    break;
                case 1: // Shoot scythes

                            if(npc.Center.X > player.Center.X && npc.ai[1] == 5)
                            {
                                npc.velocity.X = -6;
                            }

                            if(npc.Center.X < player.Center.X && npc.ai[1] == 5)
                            {
                                npc.velocity.X = 6;
                            }
                        

                        if (npc.ai[1] > 25)
                        {
                            npc.velocity.X *= 0.5f;
                        }

                        if (npc.ai[1] > 60)
                        {
                            npc.ai[0] = -1;
                            npc.ai[1] = 0;
                        }
                    
                    break;
                case 2: // Absorb energy

                        if (npc.ai[1] == 15)
                        {
                            if(npc.Center.X > player.Center.X)
                            {
                                int proj = Projectile.NewProjectile(npc.Center, new Vector2(-7, 0), 124, 20, 1f, Main.myPlayer);
                                Main.projectile[proj].friendly = false;
                                Main.projectile[proj].hostile = true;
                            }

                            if(npc.Center.X < player.Center.X)
                            {
                                int proj = Projectile.NewProjectile(npc.Center, new Vector2(7, 0), 124, 20, 1f, Main.myPlayer);
                                Main.projectile[proj].friendly = false;
                                Main.projectile[proj].hostile = true;
                            }
                        }

                        if (npc.ai[1] > 20)
                        {
                            npc.ai[0] = -1;
                            npc.ai[1] = 0;
                        }

                    break;
                case 3: // Shoot nature blasts
                    npc.velocity = Vector2.Zero;

                    if (npc.ai[1] == 50)
                    {
                        if(npc.Center.X > player.Center.X)
                        {
                            Projectile.NewProjectile(npc.Center, new Vector2(-4, 0), 44, 20, 1f, Main.myPlayer);
                        }

                        if(npc.Center.X < player.Center.X)
                        {
                            Projectile.NewProjectile(npc.Center, new Vector2(4, 0), 44, 20, 1f, Main.myPlayer);
                        }
                    }

                    if (npc.ai[1] == 100)
                    {
                        if(npc.Center.X > player.Center.X)
                        {
                            Projectile.NewProjectile(npc.Center, new Vector2(-4, 0), 44, 20, 1f, Main.myPlayer);
                        }

                        if(npc.Center.X < player.Center.X)
                        {
                            Projectile.NewProjectile(npc.Center, new Vector2(4, 0), 44, 20, 1f, Main.myPlayer);
                        }
                    }

                    if (npc.ai[1] == 160)
                    {
                        if(npc.Center.X > player.Center.X)
                        {
                            int proj = Projectile.NewProjectile(npc.Center, new Vector2(-4.5f, 0), 348, 20, 1f, Main.myPlayer);
                            Main.projectile[proj].tileCollide = false;
                        }

                        if(npc.Center.X < player.Center.X)
                        {
                            int proj = Projectile.NewProjectile(npc.Center, new Vector2(4.5f, 0), 348, 20, 1f, Main.myPlayer);
                            Main.projectile[proj].tileCollide = false;
                        }
                    }
                    if (npc.ai[1] > 200)
                    {
                        npc.ai[0] = -1;
                        npc.ai[1] = 0;
                    }
                    break;
                case 4: // scythes
                    {
                        bool shoot;
                        npc.velocity = Vector2.Zero;
                        if(npc.ai[1] == 1)
                        {
                            if(Vector2.Distance(npc.Center, player.Center) < 50f)
                            {
                                shoot = true;
                            }
                            else
                            {
                                npc.ai[1] = 40;
                            }
                        }
                        if(shoot = true)
                        {
                            if (npc.ai[1] == 45)
                            {
                                int proj1 = Projectile.NewProjectile(npc.Center + new Vector2(-40, 0), new Vector2(0, -5), 349, 20, 1f, Main.myPlayer);
                                int proj2 = Projectile.NewProjectile(npc.Center + new Vector2(-20, 0), new Vector2(0, -5), 349, 20, 1f, Main.myPlayer);
                                int proj3 = Projectile.NewProjectile(npc.Center + new Vector2(20, 0), new Vector2(0, -5), 349, 20, 1f, Main.myPlayer); 
                                int proj4 = Projectile.NewProjectile(npc.Center + new Vector2(40, 0), new Vector2(0, -5), 349, 20, 1f, Main.myPlayer);  
                            }
                        }                        
                        
                        if (npc.ai[1] > 40)
                        {
                            npc.ai[0] = -1;
                            npc.ai[1] = 0;
                        }
                        break;
                    }
            }
        }

        public override void FindFrame(int frameHeight)
        {

            npc.rotation = npc.velocity.X * 0.015f;

            npc.frameCounter++;

            if (npc.frameCounter % 12f == 11f) // Ticks per frame
            {
                npc.frame.Y += frameHeight;
            }
            if (npc.frame.Y >= frameHeight * 6) // 6 is max # of frames
            {
                npc.frame.Y = 0; // Reset back to default
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Vector2 drawOrigin = new Vector2(Main.npcTexture[npc.type].Width * 0.5f, npc.height * 0.5f);
            for (int k = 0; k < npc.oldPos.Length; k++)
            {
                // Adjust drawPos if the hitbox does not match sprite dimension
                Vector2 drawPos = npc.oldPos[k] - Main.screenPosition + drawOrigin;
                Color afterImageColor = npc.life <= npc.lifeMax * 0.5 ? Color.Green : Color.LightGreen;
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