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
        private bool introMessage = true;
        bool leafatt = false;
        private const string ChainTexturePath = "OvermorrowMod/NPCs/Bosses/Goblin/BossBar";
        private const string ChainTexturePath2 = "OvermorrowMod/NPCs/Bosses/Goblin/BossBar2";


        Vector2 savedplaypos;
        bool shoot;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Goblin MiniBoss");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {

            // Reduced size
            npc.width = 24;
            npc.height = 44;

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
            Player player = Main.player[npc.target];





                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile incomingProjectile = Main.projectile[i];
                    if (incomingProjectile.active && incomingProjectile.friendly && !incomingProjectile.minion && incomingProjectile.minionSlots < 0.5f)
                    {
                        incomingProjectile.velocity.Y = 0;
                        if(incomingProjectile.velocity.X > 0)
                        {
                            incomingProjectile.velocity.X = 7;
                        }
                        else
                        {
                            incomingProjectile.velocity.X = -7;
                        }
                    }
                }




            npc.TargetClosest(true);



            if (introMessage)
            {
                
                npc.ai[3]++;

                if (npc.ai[3] == 1)
                {
                    npc.life = 1;
                }

                npc.velocity.X = 0f;
                if (npc.ai[3] > 1 && npc.ai[3] < npc.lifeMax / 5 - 1)
                {
                    Main.player[npc.target].AddBuff(23, 23);
                    npc.life += 5;
                }

                if (npc.ai[3] <= npc.lifeMax / 5 + 50)
                {
                    return;
                }
                else
                {
                    introMessage = false;
                }
            }

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


                            if (changedPhase2 == true) { RandomCeiling = 4; }
                            else { RandomCeiling = 4; }
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
                case 1: // dash

                    
                            if(npc.Center.X > player.Center.X && npc.ai[1] == 5)
                            {
                                npc.velocity.X = -4.5f;
                                savedplaypos = player.Center + new Vector2(-59, 0);
                            }

                            if(npc.Center.X < player.Center.X && npc.ai[1] == 5)
                            {
                                npc.velocity.X = 4.5f;
                                savedplaypos = player.Center + new Vector2(59, 0);;
                            }
                        


                        if(npc.Center.X < savedplaypos.X && !shoot)
                        {
                            if(savedplaypos.X - npc.Center.X < 40 && !shoot)
                            {
                                shoot = true;
                            }
                        }


                        if(npc.Center.X > savedplaypos.X && !shoot)
                            {
                            if(npc.Center.X - savedplaypos.X < 40 && !shoot)
                            {
                                shoot = true;
                            }
                        }


                        if (npc.ai[1] > 80 || shoot)
                        {
                            savedplaypos = new Vector2(0, 0);
                            shoot = false;
                            npc.ai[0] = 4;
                            npc.ai[1] = 0;
                        }
                    
                    break;
                case 2: // Normal Attack

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
                        npc.velocity = Vector2.Zero;

                            if (npc.ai[1] == 30)
                            {
                                int proj1 = Projectile.NewProjectile(npc.Center + new Vector2(-60, 0), new Vector2(0, -7), 349, 20, 1f, Main.myPlayer);
                                int proj2 = Projectile.NewProjectile(npc.Center + new Vector2(-30, 0), new Vector2(0, -8), 349, 20, 1f, Main.myPlayer);
                                int proj3 = Projectile.NewProjectile(npc.Center + new Vector2(0, 0), new Vector2(0, -9), 349, 20, 1f, Main.myPlayer);
                                int proj4 = Projectile.NewProjectile(npc.Center + new Vector2(30, 0), new Vector2(0, -8), 349, 20, 1f, Main.myPlayer); 
                                int proj5 = Projectile.NewProjectile(npc.Center + new Vector2(60, 0), new Vector2(0, -7), 349, 20, 1f, Main.myPlayer);  
                            }                  
                        
                        if (npc.ai[1] > 40)
                        {
                            npc.ai[0] = 2;
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



        public override void NPCLoot()
        {

                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<HerosBlade>());

                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<MegaBuster>());

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {


            Vector2 mountedCenter = Main.player[npc.target].Center + new Vector2(400, -npc.life / 2.5f);

            Texture2D chainTexture = ModContent.GetTexture(ChainTexturePath);
            Texture2D chainTexture2 = ModContent.GetTexture(ChainTexturePath2);

            var drawPosition = Main.player[npc.target].Center + new Vector2(400, 0);
            var remainingVectorToPlayer = mountedCenter - drawPosition;

            float rotation = remainingVectorToPlayer.ToRotation() - MathHelper.PiOver2;

            if (npc.alpha == 0)
            {
                int direction = -1;

                if (npc.Center.X < mountedCenter.X)
                    direction = 1;

            }

            // This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
            while (true)
            {
                float length = remainingVectorToPlayer.Length();

                if (length < 25f || float.IsNaN(length))
                    break;

                // drawPosition is advanced along the vector back to the player by 12 pixels
                // 12 comes from the height of ExampleFlailProjectileChain.png and the spacing that we desired between links
                drawPosition += remainingVectorToPlayer * 1f / length;
                remainingVectorToPlayer = mountedCenter - drawPosition;

                // Finally, we draw the texture at the coordinates using the lighting information of the tile coordinates of the chain section
                Color color = Color.Red;
                spriteBatch.Draw(chainTexture, drawPosition - Main.screenPosition, null, color, 0, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            }

            return true;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }
    }
}