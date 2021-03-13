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

namespace OvermorrowMod.NPCs.Bosses.TreeBoss
{
    [AutoloadBossHead]
    public class TreeBossP2 : ModNPC
    {
        private bool changedPhase2 = false;
        private bool secondAbsorb = Main.expertMode ? true : false; // Initialize to true if Expert Mode

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iorich, Scythe of the Dryads");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            // Afterimage effect
            NPCID.Sets.TrailCacheLength[npc.type] = 7;
            NPCID.Sets.TrailingMode[npc.type] = 1;

            // Reduced size
            npc.width = 368;
            npc.height = 338;

            // Actual dimensions
            //npc.width = 368;
            //npc.height = 338;

            npc.aiStyle = -1;
            npc.damage = 26;
            npc.defense = 14;
            npc.lifeMax = 3300;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.DD2_BetsyDeath;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.boss = true;
            npc.value = Item.buyPrice(gold: 3);
            npc.npcSlots = 10f;
            music = MusicID.Boss5;
            //music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/StormDrake");
            //bossBag = ModContent.ItemType<DrakeBag>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale);
            npc.defense = 17;
        }

        public override void AI()
        {
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

            if(npc.life > npc.lifeMax)
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

            if(npc.life <= npc.lifeMax * 0.5f)
            {
                changedPhase2 = true;
            }


            switch (npc.ai[0])
            {
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

                        if (npc.ai[1] == 180)
                        {
                            if (Main.expertMode)
                            {
                                secondAbsorb = true;
                            }
                            npc.ai[0] = 1;
                            npc.ai[1] = 0;
                        }
                    }
                    break;
                case 1: // Shoot scythes
                    if (npc.ai[0] == 1)
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

                        if (npc.ai[1] % 45 == 0)
                        {
                            int shootSpeed = Main.rand.Next(8, 12);
                            Vector2 position = npc.Center;
                            Vector2 targetPosition = Main.player[npc.target].Center;
                            Vector2 direction = targetPosition - position;
                            direction.Normalize();
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center, direction * shootSpeed, ModContent.ProjectileType<NatureScythe>(), npc.damage / 2, 3f, Main.myPlayer, 0, 0);
                            }
                            //Projectile.NewProjectile(npc.Center, direction * shootSpeed, ModContent.ProjectileType<NatureWave>(), npc.damage / 2, 3f, Main.myPlayer, 0, 0);
                        }

                        if (npc.ai[1] == 600)
                        {
                            npc.ai[0] = 2;
                            npc.ai[1] = 0;
                        }
                    }
                    break;
                case 2: // Absorb energy
                    npc.velocity = Vector2.Zero;

                    // Summon projectiles from off-screen that move towards the boss
                    if (npc.ai[1] % 20 == 0)
                    {
                        for(int i = 0; i < 6; i++)
                        {
                            float randPositionX = npc.Center.X + Main.rand.Next(-10, 10) * 600;
                            float randPositionY = npc.Center.Y + Main.rand.Next(-10, 10) * 600;
                            npc.netUpdate = true;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(new Vector2(randPositionX, randPositionY), new Vector2(0, 0), ModContent.ProjectileType<AbsorbEnergy>(), npc.damage / 3, 0f, Main.myPlayer, 0f, npc.whoAmI);
                            }
                        }
                    }

                    if (npc.ai[1] == (changedPhase2 ? 480: 360))
                    {
                        npc.ai[0] = 3;
                        npc.ai[1] = 0;
                    }
                    break;
                case 3: // Shoot nature blasts
                    npc.velocity = Vector2.Zero;

                    if (npc.ai[1] % 300 == 0)
                    {
                        int projectiles = Main.rand.Next((changedPhase2 ? 13 : 9), (changedPhase2 ? 18 : 13));
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < projectiles; i++)
                            {
                                Projectile.NewProjectile(npc.Center, new Vector2(7).RotatedBy(MathHelper.ToRadians((360 / projectiles) * i + i)), ModContent.ProjectileType<NatureBlast>(), 19, 2, player.whoAmI);
                            }
                        }
                    }

                    if(npc.ai[1] == 420)
                    {
                        if (secondAbsorb)
                        {
                            npc.ai[0] = 2;
                            npc.ai[1] = 0;
                            secondAbsorb = false;
                        }
                        else
                        {
                            npc.ai[0] = 0;
                            npc.ai[1] = 0;
                        }
                    }
                    break;
               
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

        public override void NPCLoot()
        {
            // Drop loots
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
}