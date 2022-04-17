using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

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
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            // Reduced size
            NPC.width = 203;
            NPC.height = 298;

            // Actual dimensions
            //npc.width = 372;
            //npc.height = 300;

            NPC.aiStyle = -1;
            //npc.damage = 31;
            NPC.damage = 0;
            NPC.defense = 14;
            NPC.lifeMax = 3300;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.boss = true;
            NPC.npcSlots = 10f;
            //music = MusicID.Boss5;
            Music = SoundLoader.GetSoundSlot("Sounds/Music/TreeBoss");
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * bossLifeScale);
            NPC.defense = 17;
        }

        private void BossText(string text) // boss messages
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(text, Color.Green);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(text), Color.Green);
            }
        }

        public override void AI()
        {
            if (!OvermorrowWorld.downedTree && introMessage)
            {
                NPC.dontTakeDamage = true;
                NPC.netUpdate = true;
                NPC.ai[3]++;

                if (NPC.ai[3] == 180)
                {
                    BossText("I heed thy call.");
                }

                if (NPC.ai[3] == 360)
                {
                    BossText("Thou wishes to unlock the secrets of the Dryads?");
                }

                if (NPC.ai[3] == 540)
                {
                    BossText("Very well, I shalt test thy resolve.");
                }

                if (NPC.ai[3] <= 600)
                {
                    return;
                }
                else
                {
                    introMessage = false;
                    NPC.dontTakeDamage = false;
                    NPC.netUpdate = true;
                }
            }

            Player player = Main.player[NPC.target];

            // Handles Despawning
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
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

            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                NPC.velocity.Y = 2000;
            }

            if (NPC.life <= NPC.lifeMax * 0.5f)
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


            switch (NPC.ai[0])
            {
                case 0: // General case
                    // Do nothing
                    if (NPC.ai[1] == 30)
                    {
                        int randCeiling = NPC.life <= NPC.lifeMax * 0.5f ? 3 : 5;
                        int waveChance = Main.expertMode ? Main.rand.Next(0, randCeiling) : -1;

                        if (changedPhase2)
                        {
                            if (waveChance != -1) // Expert mode version
                            {
                                if (waveChance == 0)
                                {
                                    NPC.ai[0] = 4;
                                    NPC.ai[1] = 0;
                                }
                                else
                                {
                                    NPC.ai[0] = 2;
                                    NPC.ai[1] = 0;
                                }
                            }
                            else // Default Non-expert mode version
                            {
                                NPC.ai[0] = 2;
                                NPC.ai[1] = 0;
                            }
                        }
                        else
                        {
                            if (waveChance != -1) // Expert mode version
                            {
                                if (waveChance == 0)
                                {
                                    NPC.ai[0] = 4;
                                    NPC.ai[1] = 0;
                                }
                                else
                                {
                                    NPC.ai[0] = 2;
                                    NPC.ai[1] = 0;
                                }
                            }
                            else // Default Non-expert mode version
                            {
                                NPC.ai[0] = 2;
                                NPC.ai[1] = 0;
                            }
                        }
                    }
                    break;
                case 1: // Spawn thorns
                    if (NPC.ai[1] % 60 == 0 && NPC.ai[1] < 240)
                    {
                        // Get the ground beneath the player
                        Vector2 playerPos = new Vector2(player.position.X / 16, player.position.Y / 16);
                        Tile tile = Framing.GetTileSafely((int)playerPos.X, (int)playerPos.Y);
                        while (!tile.HasTile || tile.TileType == TileID.Trees)
                        {
                            playerPos.Y += 1;
                            tile = Framing.GetTileSafely((int)playerPos.X, (int)playerPos.Y);
                        }
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            //Projectile.NewProjectile(playerPos * 16, new Vector2(0, -10), ModContent.ProjectileType<ThornHead>(), 26, 2.5f, Main.myPlayer, 0f, 0f);
                        }
                    }

                    if (NPC.ai[1] == 240)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            // Get the ground beneath the player
                            Vector2 playerPos = new Vector2((player.position.X - 30 * i) / 16, (player.position.Y) / 16);
                            Vector2 playerPos2 = new Vector2((player.position.X + 30 * i) / 16, (player.position.Y) / 16);
                            Tile tile = Framing.GetTileSafely((int)playerPos.X, (int)playerPos.Y);
                            while (!tile.HasTile || tile.TileType == TileID.Trees)
                            {
                                playerPos.Y += 1;
                                tile = Framing.GetTileSafely((int)playerPos.X, (int)playerPos.Y);
                            }

                            Tile tile2 = Framing.GetTileSafely((int)playerPos2.X, (int)playerPos2.Y);
                            while (!tile2.HasTile || tile2.TileType == TileID.Trees)
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

                    if (NPC.ai[1] == 300)
                    {
                        NPC.ai[0] = 0;
                        NPC.ai[1] = 1;
                    }
                    break;
                case 2: // Shoot seeds
                    if (NPC.ai[1] % 75 == 0)
                    {
                        int numSeeds = NPC.life <= NPC.lifeMax * 0.25f ? 16 : 13;
                        float numberProjectiles = Main.rand.Next(7, numSeeds);
                        Vector2 position = NPC.Center;
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

                    if (NPC.ai[1] == 300)
                    {
                        if (changedPhase2)
                        {
                            NPC.ai[0] = 3;
                            NPC.ai[1] = 0;
                        }
                        else
                        {
                            NPC.ai[0] = 1;
                            NPC.ai[1] = 0;
                        }
                    }
                    break;
                case 3: // Multiple thorns
                    if (NPC.ai[1] % 120 == 0)
                    {
                        int randChoice = Main.rand.Next(2);
                        NPC.netUpdate = true;
                        if (randChoice == 0)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                // Get the ground beneath the player
                                Vector2 playerPos = new Vector2((player.position.X - 30 * i) / 16, (player.position.Y) / 16);
                                Vector2 playerPos2 = new Vector2((player.position.X + 30 * i) / 16, (player.position.Y) / 16);
                                Tile tile = Framing.GetTileSafely((int)playerPos.X, (int)playerPos.Y);
                                while (!tile.HasTile || tile.TileType == TileID.Trees)
                                {
                                    playerPos.Y += 1;
                                    tile = Framing.GetTileSafely((int)playerPos.X, (int)playerPos.Y);
                                }

                                Tile tile2 = Framing.GetTileSafely((int)playerPos2.X, (int)playerPos2.Y);
                                while (!tile2.HasTile || tile2.TileType == TileID.Trees)
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
                                while (!tile.HasTile || tile.TileType == TileID.Trees)
                                {
                                    playerPos.Y += 1;
                                    tile = Framing.GetTileSafely((int)playerPos.X, (int)playerPos.Y);
                                }

                                Tile tile2 = Framing.GetTileSafely((int)playerPos2.X, (int)playerPos2.Y);
                                while (!tile2.HasTile || tile2.TileType == TileID.Trees)
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

                    if (NPC.ai[1] == 320)
                    {
                        NPC.ai[0] = 0;
                        NPC.ai[1] = 0;
                    }
                    break;
                case 4: // Thorns wave
                    if (NPC.ai[1] % 15 == 0)
                    {
                        // Get the ground beneath the player
                        Vector2 npcPos = new Vector2((NPC.position.X - 60 * bufferCount) / 16, NPC.position.Y / 16);
                        Tile tile = Framing.GetTileSafely((int)npcPos.X, (int)npcPos.Y);
                        while (!tile.HasTile || tile.TileType == TileID.Trees)
                        {
                            npcPos.Y += 1;
                            tile = Framing.GetTileSafely((int)npcPos.X, (int)npcPos.Y);
                        }

                        // Same thing going right, I'm lazy
                        Vector2 npcPos2 = new Vector2((NPC.position.X + NPC.width + (60 * bufferCount)) / 16, NPC.position.Y / 16);
                        Tile tile2 = Framing.GetTileSafely((int)npcPos2.X, (int)npcPos2.Y);
                        while (!tile2.HasTile || tile2.TileType == TileID.Trees)
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

                    if (NPC.ai[1] == 180)
                    {
                        NPC.ai[0] = 5;//2;
                        NPC.ai[1] = 0;

                        bufferCount = 0;
                    }
                    break;
                case 5: // segmented thorns wave
                    {
                        if (NPC.ai[1] == 0)
                        {
                            // Get the ground beneath the player
                            Vector2 npcPos = new Vector2((NPC.position.X - /*60*/ 500 * bufferCount) / 16, NPC.position.Y / 16);
                            Tile tile = Framing.GetTileSafely((int)npcPos.X, (int)npcPos.Y);
                            while (!tile.HasTile || tile.TileType == TileID.Trees)
                            {
                                npcPos.Y += 1;
                                tile = Framing.GetTileSafely((int)npcPos.X, (int)npcPos.Y);
                            }

                            // Same thing going right, I'm lazy
                            Vector2 npcPos2 = new Vector2((NPC.position.X + NPC.width + (/*60*/ 500 * bufferCount)) / 16, NPC.position.Y / 16);
                            Tile tile2 = Framing.GetTileSafely((int)npcPos2.X, (int)npcPos2.Y);
                            while (!tile2.HasTile || tile2.TileType == TileID.Trees)
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

                        if (NPC.ai[1] == 480)
                        {
                            NPC.ai[0] = 2;
                            NPC.ai[1] = 0;

                            bufferCount = 0;
                        }
                    }
                    break;
            }

            NPC.ai[1]++;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            if (NPC.frameCounter % 12f == 11f) // Ticks per frame
            {
                NPC.frame.Y += frameHeight;
            }
            if (NPC.frame.Y >= frameHeight * 4) // 4 is max # of frames
            {
                NPC.frame.Y = 0; // Reset back to default
            }
        }

        public override bool CheckDead()
        {
            NPC.boss = false;
            return base.CheckDead();
        }

        public override void OnKill()
        {
            NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<TreeBossP2>(), 0, 0f, 0f, 0f, 0f);

            // Spawn 2nd Phase
            if (Main.netMode == NetmodeID.SinglePlayer) // Singleplayer
            {
                Main.NewText("Iorich has uprooted!", Color.Green);
            }
            else if (Main.netMode == NetmodeID.Server) // Server
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Iorich has uprooted!"), Color.Green);
            }
        }
    }
}