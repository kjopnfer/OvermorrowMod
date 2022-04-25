using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Items.Consumable.BossBags;
using OvermorrowMod.Content.Items.Weapons.Magic.SandStaff;
using OvermorrowMod.Content.Items.Weapons.Melee.SandSpinner;
using OvermorrowMod.Content.Items.Weapons.Ranged.SandThrower;
using OvermorrowMod.Content.Items.Weapons.Summoner.DustStaff;
using OvermorrowMod.Content.WorldGeneration;
using OvermorrowMod.Core;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    [AutoloadBossHead]
    public class SandstormBoss : ModNPC
    {
        private bool RunOnce = true;
        private bool ArmorImmune = false;

        private int RandomDirection;

        private Projectile ArenaCenter;

        private Vector2 InitialPosition;
        private Vector2 RandomOffset;
        private int MoveDirection;
        private enum AttackTypes
        {
            //Shards = 1,
            //Vortex = 2,
            //Spin = 3,
            //Wall = 4,
            ChainLightning = 5,
            //Pillars = 5
            Shockwave = 6
        }
        private int[] AttackQueue = new int[2];
        private int AttackCounter = 0;

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dharuud, the Sandstorm");
            //Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.width = 114;
            NPC.height = 102;
            NPC.aiStyle = -1;
            NPC.damage = 21;
            NPC.defense = 12;
            NPC.lifeMax = 4100;
            NPC.HitSound = SoundID.NPCHit23;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.chaseable = false;
            NPC.npcSlots = 10f;
            Music = MusicLoader.GetMusicSlot("OvermorrowMod/Sounds/Music/SandstormBoss");
        }
        public ref float AICase => ref NPC.ai[0];
        public ref float GlobalCounter => ref NPC.ai[1];
        public ref float AICounter => ref NPC.ai[2];
        public ref float AICounter2 => ref NPC.ai[3];

        public enum AIStates
        {
            PhaseTransition = -2,
            Intro = -1,
            Selector = 0,
            Orbs = 1,
            Ruins = 2,
            Vortex = 3,
            Shards = 4,
            ChainLightinng = 5,
            Shockwave = 6
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            if (RunOnce)
            {
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SandstormBoss_Chest>(), 0, NPC.whoAmI);

                RunOnce = false;
                NPC.netUpdate = true;
            }

            switch (AICase)
            {
                case (int)AIStates.Intro:
                    if (AICounter == 0)
                    {
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile Arena = Main.projectile[i];
                            if (Arena.active && Arena.ModProjectile is DharuudArena)
                            {
                                ArenaCenter = Arena;
                            }
                        }
                    }

                    if (AICounter++ == 120)
                    {
                        //AICase = (int)AIStates.Selector;
                        AICase = (int)AIStates.Vortex;

                        AICounter = 0;
                    }

                    break;
                case (int)AIStates.Selector:
                    if (AICounter++ <= 180)
                    {
                        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, (player.Center.X > NPC.Center.X ? 1 : -1) * 2, 0.05f);
                        NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, (player.Center.Y - 50) > NPC.Center.Y ? 2.5f : -2.5f, 0.02f);
                    }
                    else if (AICounter > 180 && AICounter < 300)
                    {
                        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, (player.Center.X > NPC.Center.X ? 1 : -1) * 2, 0.05f);
                        NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, (player.Center.Y - 250) > NPC.Center.Y ? 2.5f : -2.5f, 0.02f);
                    }

                    NPC.spriteDirection = NPC.direction;

                    // Chooses a random active minion to perform their attack
                    if (AICounter == 300)
                    {
                        NPC.velocity = Vector2.Zero;

                        AttackHandler();
                    }

                    if (AICounter == 540)
                    {
                        AttackTypes[] values = (AttackTypes[])Enum.GetValues(typeof(AttackTypes));
                        values = values.Shuffle();

                        for (int i = 0; i < AttackQueue.Length; i++)
                        {
                            AttackQueue[i] = (int)values[i];
                        }

                        AttackCounter = 0;

                        AICase = (int)AIStates.Ruins;
                        //AICase = AttackQueue[AttackCounter];

                        AttackCounter++;
                        AICounter = 0;
                    }

                    break;
                case (int)AIStates.Orbs:


                    break;
                case (int)AIStates.Ruins:
                    // Move to the center of the arena, fire off in all directions
                    // Choose a random point of the arena and move there, repeat thrice
                    // Align near the player before charging forward
                    // Release unstable orbs that drift towards the player before sucking in dust and exploding
                    /*if (GlobalCounter <= 240)
                    {
                        if (GlobalCounter++ == 0)
                        {
                            MoveDirection = Main.rand.NextBool() ? -1 : 1;
                            InitialPosition = npc.Center;
                        }

                        Main.NewText(GlobalCounter);

                        if (GlobalCounter <= 120)
                        {
                            npc.spriteDirection = npc.direction;
                            npc.Center = Vector2.Lerp(InitialPosition, player.Center + Vector2.UnitX * 75 * -MoveDirection, Utils.Clamp(GlobalCounter, 0, 120) / 120f);
                        }
                        else
                        {
                            if (GlobalCounter == 180)
                            {
                                npc.velocity = Vector2.UnitX * 10 * MoveDirection;
                            }
                        }

                        if (GlobalCounter == 200)
                        {
                            npc.velocity = Vector2.Zero;
                            GlobalCounter = 0;
                        }
                    }*/

                    if (GlobalCounter <= 240)
                    {
                        if (GlobalCounter++ == 0)
                        {
                            MoveDirection = Main.rand.NextBool() ? -1 : 1;
                            InitialPosition = NPC.Center;
                            RandomOffset = new Vector2(Main.rand.Next(15, 20) * 10, Main.rand.Next(-5, 5) * 10);
                        }

                        Main.NewText(GlobalCounter);

                        if (GlobalCounter <= 120)
                        {
                            NPC.spriteDirection = NPC.direction;
                            NPC.Center = Vector2.Lerp(InitialPosition, player.Center + RandomOffset * -MoveDirection, Utils.Clamp(GlobalCounter, 0, 120) / 120f);
                        }
                        else
                        {
                            if (GlobalCounter == 180)
                            {
                                if (Main.rand.NextBool())
                                {
                                    NPC.velocity = Vector2.UnitX * 20 * MoveDirection;
                                }
                                else
                                {
                                    NPC.velocity = Vector2.UnitY * 20 * MoveDirection;
                                }
                            }
                        }

                        if (GlobalCounter == 200)
                        {
                            NPC.velocity = Vector2.Zero;
                        }

                        if (GlobalCounter == 220)
                        {
                            GlobalCounter = 0;
                        }
                    }

                    if (AICounter++ % 15 == 0 && AICounter < 280)
                    {
                        for (int i = 0; i < Main.rand.Next(2, 4); i++)
                        {
                            WeightedRandom<int> RandomType = new WeightedRandom<int>(Main.rand);
                            RandomType.Add(3, 5);
                            RandomType.Add(1, 4);
                            RandomType.Add(2, 1);
                            int RuinID = RandomType.Get();

                            Vector2 RandomPosition = Desert.DesertArenaCenter + new Vector2(Main.rand.Next(-18, 18) * 38, 0);
                            int RuinType = Mod.Find<ModNPC>("Ruin" + RuinID).Type;
                            NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)RandomPosition.X, (int)RandomPosition.Y, RuinType, 0, 0f, Main.rand.Next(3, 8) * 128);
                        }
                    }

                    if (AICounter % 90 == 0)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            float OffsetDirection = i == 0 ? 1 : -1;
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.UnitY, ModContent.ProjectileType<ExplodeOrb>(), 30, 6f, Main.myPlayer, NPC.whoAmI, OffsetDirection);
                        }

                        /*for (int i = 0; i < 6; i++)
                        {
                            Vector2 SpawnPosition = npc.Center + new Vector2(28, 0).RotatedBy(MathHelper.ToRadians(360 / 6 * i));


                            Vector2 RandomVelocity = Vector2.Normalize(npc.DirectionTo(player.Center)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(60, 90))) * Main.rand.Next(3, 5);
                            Projectile.NewProjectile(SpawnPosition, RandomVelocity, ModContent.ProjectileType<LightStar>(), 30, 6f, Main.myPlayer, 0, -1);

                            RandomVelocity = Vector2.Normalize(npc.DirectionTo(player.Center)).RotatedBy(MathHelper.ToRadians(-Main.rand.Next(60, 90))) * Main.rand.Next(3, 5);
                            Projectile.NewProjectile(SpawnPosition, RandomVelocity, ModContent.ProjectileType<LightStar>(), 30, 6f, Main.myPlayer, 0, 1);
                            //NPC.NewNPC((int)SpawnPosition.X, (int)SpawnPosition.Y, ModContent.NPCType<LightBullet>(), 0, npc.whoAmI, 360 / 8 * i);

                        }*/
                    }

                    if (AICounter == 860)
                    {
                        Main.NewText("DROP");

                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC RuinNPC = Main.npc[i];
                            if (RuinNPC.active && RuinNPC.ModNPC is Ruin)
                            {
                                ((Ruin)RuinNPC.ModNPC).CanFall = true;

                                if (RuinNPC.ModNPC is Ruin1) RuinNPC.velocity.Y = Main.rand.Next(5, 7);

                                if (RuinNPC.ModNPC is Ruin2) RuinNPC.velocity.Y = Main.rand.Next(3, 6);

                                if (RuinNPC.ModNPC is Ruin3) RuinNPC.velocity.Y = Main.rand.Next(7, 10);

                                RuinNPC.noGravity = false;
                            }
                        }
                    }

                    if (AICounter == 920)
                    {
                        AICase = (int)AIStates.Selector;
                        GlobalCounter = 0;
                        AICounter = 0;
                    }
                    break;
                case (int)AIStates.Vortex:
                    if (AICounter++ == 0)
                    {
                        Vector2 ArenaCenter = Desert.DesertArenaCenter + new Vector2(1 * 16, 2 * 16) - new Vector2(8, -8);

                        Vector2 RandomPosition = ArenaCenter + new Vector2(20 * 16 * (Main.rand.NextBool() ? 1 : -1), 10 * 16 * (Main.rand.NextBool() ? 1 : -1));
                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), RandomPosition, Vector2.Zero, ModContent.ProjectileType<SandVortex>(), NPC.damage, 3f, Main.myPlayer);
                    }

                    if (AICounter == 60)
                    {
                        //AICase = (int)AIStates.Shards;
                        if (AttackCounter == 2)
                        {
                            AICase = (int)AIStates.Shards;
                            AttackCounter = 0;
                        }
                        else
                        {
                            AICase = AttackQueue[1];
                            AttackCounter++;
                        }

                        AICounter = 0;
                    }
                    break;
                case (int)AIStates.Shards:
                    if (AICounter == 0)
                    {
                        RandomDirection = Main.rand.NextBool() ? -1 : 1;
                        NPC.netUpdate = true;

                        Vector2 ArenaCenter = Desert.DesertArenaCenter + new Vector2(1 * 16, 2 * 16) - new Vector2(8, -8);

                        // ----->
                        if (RandomDirection == -1)
                        {
                            // TOP-DOWN MIDDLE
                            // Lower Right
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), ArenaCenter + new Vector2(10 * 16 + 48, (15 * 16) - 8), Vector2.Zero, ModContent.ProjectileType<PlatformTelegraph>(), NPC.damage, 3f, Main.myPlayer, 0, 1);

                            // Upper Right
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), ArenaCenter + new Vector2(10 * 16 + 48, (-15 * 16) + 8), Vector2.Zero, ModContent.ProjectileType<PlatformTelegraph>(), NPC.damage, 3f, Main.myPlayer, 0, 1);

                            // LEFT-RIGHT INNER
                            // Left
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), ArenaCenter + new Vector2(-12 * 16, 8 + 1), Vector2.Zero, ModContent.ProjectileType<PlatformTelegraph>(), NPC.damage, 3f, Main.myPlayer, 0, 1);
                            // Right
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), ArenaCenter + new Vector2(12 * 16, 8 + 1), Vector2.Zero, ModContent.ProjectileType<PlatformTelegraph>(), NPC.damage, 3f, Main.myPlayer, 0, -1);

                            // Right
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), ArenaCenter + new Vector2(25 * 16 + 47, 8 + 1), Vector2.Zero, ModContent.ProjectileType<PlatformTelegraph>(), NPC.damage, 3f, Main.myPlayer, 0, 1);
                        }
                        else // <----
                        {
                            Main.NewText("traveling LEFT");

                            // TOP-DOWN MIDDLE
                            // Lower Left
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), ArenaCenter + new Vector2(-10 * 16 - 48, (15 * 16) - 8), Vector2.Zero, ModContent.ProjectileType<PlatformTelegraph>(), NPC.damage, 3f, Main.myPlayer, 0, -1);

                            // Upper Left
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), ArenaCenter + new Vector2(-10 * 16 - 48, (-15 * 16) + 8), Vector2.Zero, ModContent.ProjectileType<PlatformTelegraph>(), NPC.damage, 3f, Main.myPlayer, 0, -1);

                            // LEFT-RIGHT INNER
                            // Left
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), ArenaCenter + new Vector2(-12 * 16, 8 + 1), Vector2.Zero, ModContent.ProjectileType<PlatformTelegraph>(), NPC.damage, 3f, Main.myPlayer, 0, 1);
                            // Right
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), ArenaCenter + new Vector2(12 * 16, 8 + 1), Vector2.Zero, ModContent.ProjectileType<PlatformTelegraph>(), NPC.damage, 3f, Main.myPlayer, 0, -1);

                            // LEFT-RIGHT OUTER
                            // Left
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), ArenaCenter + new Vector2(-25 * 16 - 47, 8 + 1), Vector2.Zero, ModContent.ProjectileType<PlatformTelegraph>(), NPC.damage, 3f, Main.myPlayer, 0, -1);
                        }
                    }

                    if (AICounter > 180 && AICounter % 3 == 0)
                    {
                        for (int i = 0; i < Main.rand.Next(2, 4); i++)
                        {
                            Vector2 RandomPosition = new Vector2(Main.rand.Next(700, 900) * RandomDirection, Main.rand.Next(-15, 15) * 30);
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), Desert.DesertArenaCenter + RandomPosition, Vector2.UnitX * Main.rand.Next(5, 8) * -RandomDirection, ModContent.ProjectileType<HorizontalFragment>(), NPC.damage, 3f, Main.myPlayer, 0, Main.rand.Next(60, 70) * 12);
                        }
                    }

                    if (AICounter++ == 480)
                    {
                        //AICase = (int)AIStates.Shards;
                        if (AttackCounter == 2)
                        {
                            AICase = (int)AIStates.Selector;
                            AttackCounter = 0;
                        }
                        else
                        {
                            AICase = AttackQueue[1];
                            AttackCounter++;
                        }

                        AICounter = 0;
                    }
                    break;
                case (int)AIStates.ChainLightinng:
                    if (AICounter++ == 180)
                    {
                        StartRotation(false);

                        int WeakLink = Main.rand.Next(1, 9);
                        int LinkID = 1;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC BarrierNPC = Main.npc[i];
                            if (BarrierNPC.active && BarrierNPC.ModNPC is Barrier)
                            {
                                int Node = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)BarrierNPC.Center.X, (int)BarrierNPC.Center.Y, ModContent.NPCType<LightningNode>(), 0, LinkID, ArenaCenter.whoAmI);
                                if (Main.npc[Node].ai[0] == WeakLink)
                                {
                                    Main.npc[Node].dontTakeDamage = false;
                                    ((LightningNode)Main.npc[Node].ModNPC).LinkColor = Color.Red;
                                }

                                LinkID++;
                            }
                        }
                    }

                    if (AICounter == 600) StartRotation();

                    if (AICounter == 1200)
                    {
                        AICase = (int)AIStates.Selector;
                        AICounter = 0;
                    }
                    break;
                case (int)AIStates.Shockwave:
                    if (AICounter++ == 0) StartRotation(false);

                    if (AICounter % 240 == 0)
                    {
                        Main.NewText("PAIR");

                        // Retrieve a random ID for the barrier
                        int RandomID = Main.rand.Next(1, 9);
                        // Retrieve the opposite pair of the barrier
                        switch (RandomID)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                for (int i = 0; i < Main.maxNPCs; i++)
                                {
                                    NPC BarrierNPC = Main.npc[i];
                                    if (BarrierNPC.active && BarrierNPC.ModNPC is Barrier)
                                    {
                                        if (((Barrier)BarrierNPC.ModNPC).BarrierID == RandomID || ((Barrier)BarrierNPC.ModNPC).BarrierID == RandomID + 4)
                                        {
                                            ((Barrier)BarrierNPC.ModNPC).Shockwave = true;
                                        }
                                    }
                                }
                                break;
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                                for (int i = 0; i < Main.maxNPCs; i++)
                                {
                                    NPC BarrierNPC = Main.npc[i];
                                    if (BarrierNPC.active && BarrierNPC.ModNPC is Barrier)
                                    {
                                        if (((Barrier)BarrierNPC.ModNPC).BarrierID == RandomID || ((Barrier)BarrierNPC.ModNPC).BarrierID == RandomID - 4)
                                        {
                                            ((Barrier)BarrierNPC.ModNPC).Shockwave = true;
                                        }
                                    }
                                }
                                break;
                        }
                    }

                    if (AICounter == 1439)
                    {
                        StartRotation();

                        AICase = (int)AIStates.Selector;
                        AICounter = 0;
                    }
                    break;
            }

            ArmorImmune = false;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC ArmorNPC = Main.npc[i];
                if (ArmorNPC.active && ArmorNPC.type == ModContent.NPCType<SandstormBoss_Chest>())
                {
                    ArmorImmune = true;
                }
            }
        }

        private void StartRotation(bool CanRotate = true)
        {
            foreach (NPC npc in Main.npc)
            {
                if (!npc.active || !(npc.ModNPC is Barrier)) continue;

                ((Barrier)npc.ModNPC).Rotate = CanRotate;
            }
        }

        private void AttackHandler()
        {
            //int[] ValidNPC = { ModContent.NPCType<BlasterMinion>() };
            int[] ValidNPC = { ModContent.NPCType<LaserMinion>(), ModContent.NPCType<BeamMinion>(), ModContent.NPCType<BlasterMinion>() };
            ValidNPC.Shuffle();

            for (int index = 0; index < ValidNPC.Length; index++)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && npc.type == ValidNPC[index])
                    {
                        if (!((DharuudMinion)npc.ModNPC).IsDisabled && !((DharuudMinion)npc.ModNPC).Grappled && !((DharuudMinion)npc.ModNPC).ReturnIdle)
                        {
                            ((DharuudMinion)npc.ModNPC).ExecuteAttack = true;
                            return;
                        }
                    }
                }
            }
        }

        public override bool? CanBeHitByItem(Player player, Item item) => false;

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            int[] Whitelist = { ModContent.ProjectileType<ForbiddenBeamFriendly>(), ModContent.ProjectileType<GiantBeam>(), ModContent.ProjectileType<ForbiddenBurst>() };
            if (projectile.friendly && !ArmorImmune)
            {
                if (Whitelist.Contains(projectile.type))
                {
                    return true;
                }
            }

            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            /*npc.frameCounter++;

            if (npc.frameCounter % 12f == 11f) // Ticks per frame
            {
                npc.frame.Y += frameHeight;
            }

            if (npc.frame.Y >= frameHeight * 4) // 6 is max # of frames
            {
                npc.frame.Y = 0; // Reset back to default
            }*/
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            //for(int i = 0; i < 9; i++)
            //{
            //    Texture2D texture = ModContent.GetTexture("OvermorrowMod/Textures/Rays");
            //    Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, Color.Yellow, MathHelper.ToRadians(npc.localAI[0] += 0.5f) + MathHelper.ToRadians(i * (360 / 9)), new Vector2(texture.Width / 2, texture.Height) / 2, new Vector2(3f, 1f), SpriteEffects.None, 0f);
            //}

            // Main.windSpeedSet lets the wind speed gradually increase
            // Main.windSpeed is instantaneous
            // windSpeed affects how strong the sandstorm will push the player

            //Texture2D texture = ModContent.GetTexture(AssetDirectory.Textures + "star_05");
            //Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            //
            //if (!Main.gamePaused) GlobalCounter -= 0.5f;
            //
            //Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, Color.Yellow, MathHelper.ToRadians(GlobalCounter), origin, 1f, SpriteEffects.None, 0f);
            //
            //texture = ModContent.GetTexture(AssetDirectory.Textures + "magic_02");
            //Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, Color.Yellow, MathHelper.ToRadians(GlobalCounter), origin, 1.5f, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        int frame = 0;
        const int MAX_FRAMES = 11;
        const int TextureHeight = 50;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("OvermorrowMod/Content/NPCs/Bosses/SandstormBoss/SandstormBoss_Arms").Value;

            NPC.frameCounter++;
            if (NPC.frameCounter % 12f == 11f)
            {
                frame += 1;
            }

            if (frame >= MAX_FRAMES)
            {
                frame = 0;
            }

            var DrawRectangle = new Rectangle(0, TextureHeight * frame, texture.Width, TextureHeight);
            Color color = Lighting.GetColor((int)NPC.Center.X / 16, (int)(NPC.Center.Y / 16f));
            spriteBatch.Draw(texture, NPC.Center + new Vector2(1, (NPC.width / 2) + 224) - screenPos, DrawRectangle, color, NPC.rotation, texture.Size() / 2f, 1f, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            //texture = ModContent.GetTexture("OvermorrowMod/Content/NPCs/Bosses/SandstormBoss/DharuudArmor");
            //Main.spriteBatch.Draw(texture, npc.Center + new Vector2(-2, 2) - Main.screenPosition, null, color, npc.rotation, texture.Size() / 2f, 1f, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

        }

        public override void OnKill()
        {
            OvermorrowWorld.downedDarude = true;

            if (Sandstorm.Happening)
            {
                Sandstorm.Happening = false;
                Sandstorm.TimeLeft = 18000;
                ModUtils.SandstormStuff();
            }

            for (int num785 = 0; num785 < 4; num785++)
            {
                Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Sand, 0f, 0f, 100, default(Color), 1.5f);
            }

            for (int num788 = 0; num788 < 40; num788++)
            {
                int num797 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Sand, 0f, 0f, 0, default(Color), 2.5f);
                Main.dust[num797].noGravity = true;
                Dust dust24 = Main.dust[num797];
                dust24.velocity *= 3f;
                num797 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Sand, 0f, 0f, 100, default(Color), 1.5f);
                dust24 = Main.dust[num797];
                dust24.velocity *= 2f;
                Main.dust[num797].noGravity = true;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ItemType<SandstormBag>()));
            var nonExpert = new LeadingConditionRule(new Conditions.NotExpert())
                .OnSuccess(ItemDropRule.OneFromOptions(1,
                    ItemType<SandstormSpinner>(),
                    ItemType<SandStaff>(),
                    ItemType<SandThrower>(),
                    ItemType<DustStaff>()));
            npcLoot.Add(nonExpert);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }
    }
}