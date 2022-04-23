using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Items.Consumable.BossBags;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Content.Items.Placeable.Boss;
using OvermorrowMod.Content.Items.Weapons.Magic.IorichStaff;
using OvermorrowMod.Content.Items.Weapons.Melee.IorichHarvester;
using OvermorrowMod.Content.Items.Weapons.Ranged.IorichBow;
using OvermorrowMod.Content.Items.Weapons.Summoner.IorichWand;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Content.NPCs.Bosses.TreeBoss
{
    [AutoloadBossHead]
    public class TreeBossP2 : ModNPC
    {
        private bool changedPhase2 = false;
        public int energiesAbsorbed;
        public int energiesKilled;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iorich, Scythe of the Dryads");
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            // Afterimage effect
            NPCID.Sets.TrailCacheLength[NPC.type] = 7;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            // Reduced size
            NPC.width = 368;
            NPC.height = 338;

            // Actual dimensions
            //npc.width = 368;
            //npc.height = 338;

            NPC.aiStyle = -1;
            NPC.damage = 20;
            NPC.defense = 14;
            NPC.lifeMax = 3300;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.Item25;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.value = Item.buyPrice(gold: 3);
            NPC.npcSlots = 10f;
            //music = MusicID.Boss5;
            Music = MusicLoader.GetMusicSlot("OvermorrowMod/Sounds/Music/TreeMan2");
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * bossLifeScale * 0.65f);
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

        int RandomCase = 0;
        int LastCase = 0;
        int RandomCeiling;
        bool movement = true;

        public override void AI()
        {
            // Death animation code
            if (NPC.ai[3] > 0f)
            {
                NPC.velocity = Vector2.Zero;

                if (NPC.ai[2] > 0)
                {
                    NPC.ai[2]--;

                    if (NPC.ai[2] == 480)
                    {
                        BossText("I deem thee fit to inherit their powers.");
                    }

                    if (NPC.ai[2] == 300)
                    {
                        BossText("Thou Dryad shalt guide thee.");
                    }

                    if (NPC.ai[2] == 120)
                    {
                        BossText("Fare thee well.");
                    }
                }
                else
                {
                    NPC.dontTakeDamage = true;
                    NPC.ai[3]++; // Death timer
                    NPC.velocity.X *= 0.95f;

                    if (NPC.velocity.Y < 0.5f)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + 0.01f;
                    }

                    if (NPC.velocity.X > 0.5f)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - 0.01f;
                    }

                    if (NPC.ai[3] > 120f)
                    {
                        NPC.Opacity = 1f - (NPC.ai[3] - 120f) / 60f;
                    }

                    if (Main.rand.NextBool(5) && NPC.ai[3] < 120f)
                    {
                        // This dust spawn adapted from the Pillar death code in vanilla.
                        for (int dustNumber = 0; dustNumber < 6; dustNumber++)
                        {
                            Dust dust = Main.dust[Dust.NewDust(NPC.Left, NPC.width, NPC.height / 2, DustID.TerraBlade, 0f, 0f, 0, default(Color), 1f)];
                            dust.position = NPC.Center + Vector2.UnitY.RotatedByRandom(4.1887903213500977) * new Vector2(NPC.width * 1.5f, NPC.height * 1.1f) * 0.8f * (0.8f + Main.rand.NextFloat() * 0.2f);
                            dust.velocity.X = 0f;
                            dust.velocity.Y = -Math.Abs(dust.velocity.Y - (float)dustNumber + NPC.velocity.Y - 4f) * 3f;
                            dust.noGravity = true;
                            dust.fadeIn = 1f;
                            dust.scale = 1f + Main.rand.NextFloat() + (float)dustNumber * 0.3f;
                        }
                    }

                    if (NPC.ai[3] % 30f == 1f)
                    {
                        //Main.PlaySound(4, npc.Center, 22);
                        SoundEngine.PlaySound(SoundID.Item25, NPC.Center); // every half second while dying, play a sound
                    }

                    if (NPC.ai[3] >= 180f)
                    {
                        NPC.life = 0;
                        NPC.HitEffect(0, 0);
                        NPC.checkDead(); // This will trigger ModNPC.CheckDead the second time, causing the real death.
                    }
                }
                return;
            }

            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

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
                NPC.velocity.Y = -2000;
            }

            if (NPC.life > NPC.lifeMax)
            {
                NPC.life = NPC.lifeMax;
            }

            NPC.ai[1]++;

            // Moveset
            // Throw scythes
            // Throw a wall of scythes, repeat this and top twice
            // Charge 3 times
            // Spawn nature waves in all directions
            // Move toward player slowly

            if (NPC.life <= NPC.lifeMax * 0.5f)
            {
                changedPhase2 = true;
            }

            switch (NPC.ai[0])
            {
                case -1: // case switching
                    {
                        if (movement == true)
                        {
                            if (changedPhase2 == true) { RandomCeiling = 4; }
                            else { RandomCeiling = 3; }
                            while (RandomCase == LastCase)
                            {
                                RandomCase = Main.rand.Next(1, RandomCeiling);
                            }
                            LastCase = RandomCase;
                            movement = false;
                            NPC.ai[0] = RandomCase;
                        }
                        else
                        {
                            movement = true;
                            NPC.ai[0] = 0;
                        }
                    }
                    break;
                case 0: // Follow player
                    if (NPC.ai[0] == 0)
                    {
                        Vector2 moveTo = player.Center;
                        var move = moveTo - NPC.Center;
                        var speed = 5;

                        float length = move.Length();
                        if (length > speed)
                        {
                            move *= speed / length;
                        }
                        var turnResistance = 45;
                        move = (NPC.velocity * turnResistance + move) / (turnResistance + 1f);
                        length = move.Length();
                        if (length > 10)
                        {
                            move *= speed / length;
                        }
                        NPC.velocity.X = move.X;
                        NPC.velocity.Y = move.Y * .98f;

                        if (NPC.ai[1] > (changedPhase2 ? 90 : 120))
                        {
                            NPC.ai[0] = -1;
                            NPC.ai[1] = 0;
                        }
                    }
                    break;
                case 1: // Shoot scythes
                    if (NPC.ai[0] == 1)
                    {
                        Vector2 moveTo = player.Center;
                        var move = moveTo - NPC.Center;
                        var speed = 5;

                        float length = move.Length();
                        if (length > speed)
                        {
                            move *= speed / length;
                        }
                        var turnResistance = 45;
                        move = (NPC.velocity * turnResistance + move) / (turnResistance + 1f);
                        length = move.Length();
                        if (length > 10)
                        {
                            move *= speed / length;
                        }
                        NPC.velocity.X = move.X;
                        NPC.velocity.Y = move.Y * .98f;

                        if (NPC.ai[1] % 90 == 0)
                        {
                            int shootSpeed = Main.rand.Next(8, 12);
                            Vector2 position = NPC.Center;
                            Vector2 targetPosition = Main.player[NPC.target].Center;
                            Vector2 direction = targetPosition - position;
                            direction.Normalize();
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                //Projectile.NewProjectile(npc.Center, direction * shootSpeed, ModContent.ProjectileType<NatureScythe>(), npc.damage / 2, 3f, Main.myPlayer, 0, 0);
                            }
                        }

                        if (NPC.ai[1] > 600)
                        {
                            NPC.ai[0] = -1;
                            NPC.ai[1] = 0;
                        }
                    }
                    break;
                case 2: // Absorb energy
                    NPC.velocity = Vector2.Zero;

                    // Summon projectiles from off-screen that move towards the boss
                    if (NPC.ai[1] % 20 == 0 && (energiesAbsorbed + energiesKilled) < 33 && NPC.ai[1] <= 660)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            float randPositionX = NPC.Center.X + Main.rand.Next(-10, 10) * 600;
                            float randPositionY = NPC.Center.Y + Main.rand.Next(-10, 10) * 600;
                            NPC.netUpdate = true;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                //NPC.NewNPC((int)randPositionX, (int)randPositionY, ModContent.NPCType<AbsorbEnergy>(), 0, 0f, npc.whoAmI, 0, npc.damage / 3, Main.myPlayer);
                            }
                        }
                    }

                    if (energiesKilled <= 5 && NPC.ai[1] > 660) // punish
                    {
                        NPC.ai[2] = 1;
                        Main.NewText("u suk");
                    }
                    else if (NPC.ai[1] > 660) // else
                    {
                        NPC.ai[2] = 1;
                    }

                    if (NPC.ai[1] > 660 && NPC.ai[3] == 1)
                    {
                        energiesAbsorbed = 0;
                        energiesKilled = 0;
                        NPC.ai[0] = 4;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                    }
                    break;
                case 4: // Shoot nature blasts
                    NPC.velocity = Vector2.Zero;

                    if (NPC.ai[1] == 120)
                    {
                        int projectiles = Main.rand.Next((changedPhase2 ? 13 : 9), (changedPhase2 ? 18 : 13));
                        NPC.netUpdate = true;

                        for (int i = 0; i < projectiles; i++)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                //Projectile.NewProjectile(npc.Center, new Vector2(7).RotatedBy(MathHelper.ToRadians((360 / projectiles) * i + i)), ModContent.ProjectileType<NatureBlast>(), 19, 2, Main.myPlayer);
                            }
                        }
                    }

                    if (NPC.ai[1] > 240)
                    {
                        NPC.ai[0] = -1;
                        NPC.ai[1] = 0;
                    }
                    break;
                case 3: // scythes
                    {
                        Vector2 moveTo = player.Center;
                        moveTo.X += 50 * (NPC.Center.X < moveTo.X ? -1 : 1);
                        var move = moveTo - NPC.Center;
                        var speed = 1;

                        float length = move.Length();
                        if (length > speed)
                        {
                            move *= speed / length;
                        }
                        var turnResistance = 45;
                        move = (NPC.velocity * turnResistance + move) / (turnResistance + 1f);
                        length = move.Length();
                        if (length > 10)
                        {
                            move *= speed / length;
                        }
                        NPC.velocity.X = move.X;
                        NPC.velocity.Y = move.Y * .98f;


                        if (NPC.ai[1] == 180)
                        {
                            for (int i = -2; i < 3; i++)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    //Projectile.NewProjectile(new Vector2(player.Center.X + (250 * i), player.Center.Y - 200), Vector2.UnitY * 5, ModContent.ProjectileType<NatureScythe>(), 17, 1f, Main.myPlayer, 0, 1);
                                    //Projectile.NewProjectile(new Vector2(player.Center.X + (250 * i), player.Center.Y + 200), -Vector2.UnitY * 5, ModContent.ProjectileType<NatureScythe>(), 17, 1f, Main.myPlayer, 0, 1);
                                }
                            }
                        }
                        if (NPC.ai[1] > 540)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                        }
                        break;
                    }
            }
        }

        public override void FindFrame(int frameHeight)
        {

            NPC.rotation = NPC.velocity.X * 0.015f;

            NPC.frameCounter++;

            if (NPC.frameCounter % 12f == 11f) // Ticks per frame
            {
                NPC.frame.Y += frameHeight;
            }
            if (NPC.frame.Y >= frameHeight * 6) // 6 is max # of frames
            {
                NPC.frame.Y = 0; // Reset back to default
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Value.Width * 0.5f, NPC.height * 0.5f);
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                // Adjust drawPos if the hitbox does not match sprite dimension
                Vector2 drawPos = NPC.oldPos[k] - screenPos + drawOrigin;
                Color afterImageColor = NPC.life <= NPC.lifeMax * 0.5 ? Color.Green : Color.LightGreen;
                Color color = NPC.GetAlpha(afterImageColor) * ((float)(NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, NPC.frame, color, NPC.rotation, drawOrigin, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        public override bool CheckDead()
        {
            if (NPC.ai[3] == 0f)
            {
                NPC.ai[2] = OvermorrowWorld.downedTree ? 0 : 540;
                NPC.ai[3] = 1f;
                NPC.damage = 0;
                NPC.life = NPC.lifeMax;
                NPC.dontTakeDamage = true;
                NPC.netUpdate = true;
                return false;
            }
            return true;
        }

        public override void OnKill()
        {
            OvermorrowWorld.downedTree = true;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ItemType<TreeBag>()));
            var nonExpert = new LeadingConditionRule(new Conditions.NotExpert())
                .OnSuccess(ItemDropRule.OneFromOptions(1,
                    ItemType<IorichStaff>(),
                    ItemType<IorichHarvester>(),
                    ItemType<IorichBow>(),
                    ItemType<IorichWand>()))
                .OnSuccess(ItemDropRule.Common(ItemType<TreeTrophy>(), 10))
                .OnSuccess(ItemDropRule.Common(ItemType<SapStone>(), 1, 1, 4));
            npcLoot.Add(nonExpert);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }
    }
}
