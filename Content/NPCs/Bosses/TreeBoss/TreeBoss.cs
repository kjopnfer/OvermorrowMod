using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using OvermorrowMod.Sounds.Music;

namespace OvermorrowMod.Content.NPCs.Bosses.TreeBoss
{
    [AutoloadBossHead]
    public class TreeBoss : ModNPC
    {
        private bool changedPhase2 = false;
        private bool introMessage = true;
        private int bufferCount = 0;

        private enum spawnDirection { left, right }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iorich, the Guardian");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            // Reduced size
            npc.width = 203;
            npc.height = 298;

            // Actual dimensions
            //npc.width = 372;
            //npc.height = 300;

            npc.aiStyle = -1;
            //npc.damage = 31;
            npc.damage = 0;
            npc.defense = 14;
            npc.lifeMax = 3300;
            npc.HitSound = SoundID.NPCHit1;
            npc.knockBackResist = 0f;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.boss = true;
            npc.npcSlots = 10f;
            //music = MusicID.Boss5;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/TreeMan1");
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale);
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

        public override void AI()
        {
            if (!OvermorrowWorld.downedTree && introMessage)
            {
                npc.dontTakeDamage = true;
                npc.netUpdate = true;
                npc.ai[3]++;

                if (npc.ai[3] == 180)
                {
                    BossText("I heed thy call.");
                }

                if (npc.ai[3] == 360)
                {
                    BossText("Thou wishes to unlock the secrets of the Dryads?");
                }

                if (npc.ai[3] == 540)
                {
                    BossText("Very well, I shalt test thy resolve.");
                }

                if (npc.ai[3] <= 600)
                {
                    return;
                }
                else
                {
                    introMessage = false;
                    npc.dontTakeDamage = false;
                    npc.netUpdate = true;
                }
            }

            Player player = Main.player[npc.target];

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
                npc.velocity.Y = 2000;
            }

            if (npc.life <= npc.lifeMax * 0.5f)
            {
                changedPhase2 = true;
            }

            // General MOVESET:
            // Shoot thorns from under the ground (50% thorns linger longer)
            // Drop seeds that float down from above
            // (50%) Shoot multiple thorns from under the ground that are close together at the player
            // (25%) Shoot multiple seeds the float down from above

            // EXPERT MODE:
            // Shoots a wave of thorns


            switch (npc.ai[0])
            {
                case 0: // General case
                    // Do nothing
                    if (npc.ai[1] == 30)
                    {
                        int randCeiling = npc.life <= npc.lifeMax * 0.5f ? 3 : 5;
                        int waveChance = Main.expertMode ? Main.rand.Next(0, randCeiling) : -1;

                        if (changedPhase2)
                        {
                            if (waveChance != -1) // Expert mode version
                            {
                                if (waveChance == 0)
                                {
                                    npc.ai[0] = 4;
                                    npc.ai[1] = 0;
                                }
                                else
                                {
                                    npc.ai[0] = 2;
                                    npc.ai[1] = 0;
                                }
                            }
                            else // Default Non-expert mode version
                            {
                                npc.ai[0] = 2;
                                npc.ai[1] = 0;
                            }
                        }
                        else
                        {
                            if (waveChance != -1) // Expert mode version
                            {
                                if (waveChance == 0)
                                {
                                    npc.ai[0] = 4;
                                    npc.ai[1] = 0;
                                }
                                else
                                {
                                    npc.ai[0] = 2;
                                    npc.ai[1] = 0;
                                }
                            }
                            else // Default Non-expert mode version
                            {
                                npc.ai[0] = 2;
                                npc.ai[1] = 0;
                            }
                        }
                    }
                    break;
                case 1: // Spawn thorns
                    if (npc.ai[1] % 60 == 0 && npc.ai[1] < 240)
                    {
                        // Get the ground beneath the player
                        Vector2 playerPos = new Vector2(player.position.X / 16, player.position.Y / 16);
                        Tile tile = Framing.GetTileSafely((int)playerPos.X, (int)playerPos.Y);
                        while (!tile.active() || tile.type == TileID.Trees)
                        {
                            playerPos.Y += 1;
                            tile = Framing.GetTileSafely((int)playerPos.X, (int)playerPos.Y);
                        }
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            //Projectile.NewProjectile(playerPos * 16, new Vector2(0, -10), ModContent.ProjectileType<ThornHead>(), 26, 2.5f, Main.myPlayer, 0f, 0f);
                        }
                    }

                    if (npc.ai[1] == 240)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            // Get the ground beneath the player
                            Vector2 playerPos = new Vector2((player.position.X - 30 * i) / 16, (player.position.Y) / 16);
                            Vector2 playerPos2 = new Vector2((player.position.X + 30 * i) / 16, (player.position.Y) / 16);
                            Tile tile = Framing.GetTileSafely((int)playerPos.X, (int)playerPos.Y);
                            while (!tile.active() || tile.type == TileID.Trees)
                            {
                                playerPos.Y += 1;
                                tile = Framing.GetTileSafely((int)playerPos.X, (int)playerPos.Y);
                            }

                            Tile tile2 = Framing.GetTileSafely((int)playerPos2.X, (int)playerPos2.Y);
                            while (!tile2.active() || tile2.type == TileID.Trees)
                            {
                                playerPos2.Y += 1;
                                tile2 = Framing.GetTileSafely((int)playerPos2.X, (int)playerPos2.Y);
                            }
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (i == 0)
                                {
                                    //Projectile.NewProjectile(playerPos * 16, new Vector2(0, -10), ModContent.ProjectileType<ThornHead>(), 28, 2.5f, Main.myPlayer, 0f, 0f);
                                }
                                else
                                {
                                    //Projectile.NewProjectile(playerPos * 16, new Vector2(0, -10), ModContent.ProjectileType<ThornHead>(), 28, 2.5f, Main.myPlayer, 0f, 0f);
                                    //Projectile.NewProjectile(playerPos2 * 16, new Vector2(0, -10), ModContent.ProjectileType<ThornHead>(), 28, 2.5f, Main.myPlayer, 0f, 0f);
                                }
                            }
                        }
                    }

                    if (npc.ai[1] == 300)
                    {
                        npc.ai[0] = 0;
                        npc.ai[1] = 1;
                    }
                    break;
                case 2: // Shoot seeds
                    if (npc.ai[1] % 75 == 0)
                    {
                        int numSeeds = npc.life <= npc.lifeMax * 0.25f ? 16 : 13;
                        float numberProjectiles = Main.rand.Next(7, numSeeds);
                        Vector2 position = npc.Center;
                        int speedX = 1;
                        int speedY = Main.rand.Next(-25, -15);
                        float rotation = MathHelper.ToRadians(45);
                        position += Vector2.Normalize(new Vector2(speedX, speedY)) * 45f; //this defines the distance of the projectiles form the player when the projectile spawns
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < numberProjectiles; i++)
                            {
                                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .4f; // This defines the projectile roatation and speed. .4f == projectile speed
                                //Projectile.NewProjectile(npc.Center.X, npc.Center.Y - 85, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<FloatingSeeds>(), 17, 1f, Main.myPlayer);
                            }
                        }
                    }

                    if (npc.ai[1] == 300)
                    {
                        if (changedPhase2)
                        {
                            npc.ai[0] = 3;
                            npc.ai[1] = 0;
                        }
                        else
                        {
                            npc.ai[0] = 1;
                            npc.ai[1] = 0;
                        }
                    }
                    break;
                case 3: // Multiple thorns
                    if (npc.ai[1] % 120 == 0)
                    {
                        int randChoice = Main.rand.Next(2);
                        npc.netUpdate = true;
                        if (randChoice == 0)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                // Get the ground beneath the player
                                Vector2 playerPos = new Vector2((player.position.X - 30 * i) / 16, (player.position.Y) / 16);
                                Vector2 playerPos2 = new Vector2((player.position.X + 30 * i) / 16, (player.position.Y) / 16);
                                Tile tile = Framing.GetTileSafely((int)playerPos.X, (int)playerPos.Y);
                                while (!tile.active() || tile.type == TileID.Trees)
                                {
                                    playerPos.Y += 1;
                                    tile = Framing.GetTileSafely((int)playerPos.X, (int)playerPos.Y);
                                }

                                Tile tile2 = Framing.GetTileSafely((int)playerPos2.X, (int)playerPos2.Y);
                                while (!tile2.active() || tile2.type == TileID.Trees)
                                {
                                    playerPos2.Y += 1;
                                    tile2 = Framing.GetTileSafely((int)playerPos2.X, (int)playerPos2.Y);
                                }
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    if (i == 0)
                                    {
                                        //Projectile.NewProjectile(playerPos * 16, new Vector2(0, -10), ModContent.ProjectileType<ThornHead>(), 28, 2.5f, Main.myPlayer, 0f, 0f);
                                    }
                                    else
                                    {
                                        //Projectile.NewProjectile(playerPos * 16, new Vector2(0, -10), ModContent.ProjectileType<ThornHead>(), 28, 2.5f, Main.myPlayer, 0f, 0f);
                                        //Projectile.NewProjectile(playerPos2 * 16, new Vector2(0, -10), ModContent.ProjectileType<ThornHead>(), 28, 2.5f, Main.myPlayer, 0f, 0f);
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                // Get the ground beneath the player
                                Vector2 playerPos = new Vector2((player.position.X - 90 * i) / 16, (player.position.Y) / 16);
                                Vector2 playerPos2 = new Vector2((player.position.X + 90 * i) / 16, (player.position.Y) / 16);
                                Tile tile = Framing.GetTileSafely((int)playerPos.X, (int)playerPos.Y);
                                while (!tile.active() || tile.type == TileID.Trees)
                                {
                                    playerPos.Y += 1;
                                    tile = Framing.GetTileSafely((int)playerPos.X, (int)playerPos.Y);
                                }

                                Tile tile2 = Framing.GetTileSafely((int)playerPos2.X, (int)playerPos2.Y);
                                while (!tile2.active() || tile2.type == TileID.Trees)
                                {
                                    playerPos2.Y += 1;
                                    tile2 = Framing.GetTileSafely((int)playerPos2.X, (int)playerPos2.Y);
                                }
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    if (i == 0)
                                    {
                                        //Projectile.NewProjectile(playerPos * 16, new Vector2(0, -10), ModContent.ProjectileType<ThornHead>(), 28, 2.5f, Main.myPlayer, 0f, 0f);
                                    }
                                    else
                                    {
                                        //Projectile.NewProjectile(playerPos * 16, new Vector2(0, -10), ModContent.ProjectileType<ThornHead>(), 28, 2.5f, Main.myPlayer, 0f, 0f);
                                        //Projectile.NewProjectile(playerPos2 * 16, new Vector2(0, -10), ModContent.ProjectileType<ThornHead>(), 28, 2.5f, Main.myPlayer, 0f, 0f);
                                    }
                                }
                            }
                        }
                    }

                    if (npc.ai[1] == 320)
                    {
                        npc.ai[0] = 0;
                        npc.ai[1] = 0;
                    }
                    break;
                case 4: // Thorns wave
                    if (npc.ai[1] % 15 == 0)
                    {
                        // Get the ground beneath the player
                        Vector2 npcPos = new Vector2((npc.position.X - 60 * bufferCount) / 16, npc.position.Y / 16);
                        Tile tile = Framing.GetTileSafely((int)npcPos.X, (int)npcPos.Y);
                        while (!tile.active() || tile.type == TileID.Trees)
                        {
                            npcPos.Y += 1;
                            tile = Framing.GetTileSafely((int)npcPos.X, (int)npcPos.Y);
                        }

                        // Same thing going right, I'm lazy
                        Vector2 npcPos2 = new Vector2((npc.position.X + npc.width + (60 * bufferCount)) / 16, npc.position.Y / 16);
                        Tile tile2 = Framing.GetTileSafely((int)npcPos2.X, (int)npcPos2.Y);
                        while (!tile2.active() || tile2.type == TileID.Trees)
                        {
                            npcPos2.Y += 1;
                            tile2 = Framing.GetTileSafely((int)npcPos2.X, (int)npcPos2.Y);
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            //Projectile.NewProjectile(npcPos2 * 16, new Vector2(0, -10), ModContent.ProjectileType<ThornHead>(), 31, 2.5f, Main.myPlayer, 0f, 0f);
                            //
                            //Projectile.NewProjectile(npcPos * 16, new Vector2(0, -10), ModContent.ProjectileType<ThornHead>(), 31, 2.5f, Main.myPlayer, 0f, 0f);
                            bufferCount++;
                        }
                    }

                    if (npc.ai[1] == 180)
                    {
                        npc.ai[0] = 5;//2;
                        npc.ai[1] = 0;

                        bufferCount = 0;
                    }
                    break;
                case 5: // segmented thorns wave
                    {
                        if (npc.ai[1] == 0)
                        {
                            // Get the ground beneath the player
                            Vector2 npcPos = new Vector2((npc.position.X - /*60*/ 500 * bufferCount) / 16, npc.position.Y / 16);
                            Tile tile = Framing.GetTileSafely((int)npcPos.X, (int)npcPos.Y);
                            while (!tile.active() || tile.type == TileID.Trees)
                            {
                                npcPos.Y += 1;
                                tile = Framing.GetTileSafely((int)npcPos.X, (int)npcPos.Y);
                            }

                            // Same thing going right, I'm lazy
                            Vector2 npcPos2 = new Vector2((npc.position.X + npc.width + (/*60*/ 500 * bufferCount)) / 16, npc.position.Y / 16);
                            Tile tile2 = Framing.GetTileSafely((int)npcPos2.X, (int)npcPos2.Y);
                            while (!tile2.active() || tile2.type == TileID.Trees)
                            {
                                npcPos2.Y += 1;
                                tile2 = Framing.GetTileSafely((int)npcPos2.X, (int)npcPos2.Y);
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                //Projectile.NewProjectile(npcPos2 * 16, new Vector2(0, -10), ModContent.ProjectileType<ThornHead>(), 31, 2.5f, Main.myPlayer, 0f, 0f);
                                //
                                //Projectile.NewProjectile(npcPos * 16, new Vector2(0, -10), ModContent.ProjectileType<ThornHead>(), 31, 2.5f, Main.myPlayer, 0f, 0f);
                                bufferCount++;
                            }
                        }

                        if (npc.ai[1] == 480)
                        {
                            npc.ai[0] = 2;
                            npc.ai[1] = 0;

                            bufferCount = 0;
                        }
                    }
                    break;
            }

            npc.ai[1]++;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;

            if (npc.frameCounter % 12f == 11f) // Ticks per frame
            {
                npc.frame.Y += frameHeight;
            }
            if (npc.frame.Y >= frameHeight * 4) // 4 is max # of frames
            {
                npc.frame.Y = 0; // Reset back to default
            }
        }

        public override bool CheckDead()
        {
            npc.boss = false;
            return base.CheckDead();
        }

        public override void NPCLoot()
        {
            NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<TreeBossP2>(), 0, 0f, 0f, 0f, 0f);

            // Spawn 2nd Phase
            if (Main.netMode == NetmodeID.SinglePlayer) // Singleplayer
            {
                Main.NewText("Iorich has uprooted!", Color.Green);
            }
            else if (Main.netMode == NetmodeID.Server) // Server
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("Iorich has uprooted!"), Color.Green);
            }
        }
    }
}
