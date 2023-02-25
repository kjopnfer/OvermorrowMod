/*using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
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
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<SandstormBoss_Chest>(), 0, NPC.whoAmI);

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
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), RandomPosition, Vector2.Zero, ModContent.ProjectileType<SandVortex>(), NPC.damage, 3f, Main.myPlayer);
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
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), ArenaCenter + new Vector2(10 * 16 + 48, (15 * 16) - 8), Vector2.Zero, ModContent.ProjectileType<PlatformTelegraph>(), NPC.damage, 3f, Main.myPlayer, 0, 1);

                            // Upper Right
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), ArenaCenter + new Vector2(10 * 16 + 48, (-15 * 16) + 8), Vector2.Zero, ModContent.ProjectileType<PlatformTelegraph>(), NPC.damage, 3f, Main.myPlayer, 0, 1);

                            // LEFT-RIGHT INNER
                            // Left
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), ArenaCenter + new Vector2(-12 * 16, 8 + 1), Vector2.Zero, ModContent.ProjectileType<PlatformTelegraph>(), NPC.damage, 3f, Main.myPlayer, 0, 1);
                            // Right
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), ArenaCenter + new Vector2(12 * 16, 8 + 1), Vector2.Zero, ModContent.ProjectileType<PlatformTelegraph>(), NPC.damage, 3f, Main.myPlayer, 0, -1);

                            // Right
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), ArenaCenter + new Vector2(25 * 16 + 47, 8 + 1), Vector2.Zero, ModContent.ProjectileType<PlatformTelegraph>(), NPC.damage, 3f, Main.myPlayer, 0, 1);
                        }
                        else // <----
                        {
                            Main.NewText("traveling LEFT");

                            // TOP-DOWN MIDDLE
                            // Lower Left
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), ArenaCenter + new Vector2(-10 * 16 - 48, (15 * 16) - 8), Vector2.Zero, ModContent.ProjectileType<PlatformTelegraph>(), NPC.damage, 3f, Main.myPlayer, 0, -1);

                            // Upper Left
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), ArenaCenter + new Vector2(-10 * 16 - 48, (-15 * 16) + 8), Vector2.Zero, ModContent.ProjectileType<PlatformTelegraph>(), NPC.damage, 3f, Main.myPlayer, 0, -1);

                            // LEFT-RIGHT INNER
                            // Left
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), ArenaCenter + new Vector2(-12 * 16, 8 + 1), Vector2.Zero, ModContent.ProjectileType<PlatformTelegraph>(), NPC.damage, 3f, Main.myPlayer, 0, 1);
                            // Right
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), ArenaCenter + new Vector2(12 * 16, 8 + 1), Vector2.Zero, ModContent.ProjectileType<PlatformTelegraph>(), NPC.damage, 3f, Main.myPlayer, 0, -1);

                            // LEFT-RIGHT OUTER
                            // Left
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), ArenaCenter + new Vector2(-25 * 16 - 47, 8 + 1), Vector2.Zero, ModContent.ProjectileType<PlatformTelegraph>(), NPC.damage, 3f, Main.myPlayer, 0, -1);
                        }
                    }

                    if (AICounter > 180 && AICounter % 3 == 0)
                    {
                        for (int i = 0; i < Main.rand.Next(2, 4); i++)
                        {
                            Vector2 RandomPosition = new Vector2(Main.rand.Next(700, 900) * RandomDirection, Main.rand.Next(-15, 15) * 30);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), Desert.DesertArenaCenter + RandomPosition, Vector2.UnitX * Main.rand.Next(5, 8) * -RandomDirection, ModContent.ProjectileType<HorizontalFragment>(), NPC.damage, 3f, Main.myPlayer, 0, Main.rand.Next(60, 70) * 12);
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
                                int Node = NPC.NewNPC(NPC.GetSource_FromAI(), (int)BarrierNPC.Center.X, (int)BarrierNPC.Center.Y, ModContent.NPCType<LightningNode>(), 0, LinkID, ArenaCenter.whoAmI);
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

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }
    }
}*/