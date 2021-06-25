using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Boss;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System;
using Terraria.Localization;

namespace OvermorrowMod.NPCs.Bosses.StormDrake
{
    [AutoloadBossHead]
    public class StormDrake2 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Drake");
            Main.npcFrameCount[npc.type] = 6;
            NPCID.Sets.TrailCacheLength[npc.type] = 14;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }
        public override void SetDefaults()
        {
            npc.width = 296;
            npc.height = 232;
            npc.aiStyle = -1;
            npc.damage = 37;
            npc.defense = 14;
            npc.lifeMax = 7600;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.DD2_BetsyDeath;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.boss = true;
            npc.value = Item.buyPrice(gold: 3);
            npc.npcSlots = 10f;
            // music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/StormDrake");
            // bossBag = ModContent.ItemType<DrakeBag>();
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.localAI[0]);
            writer.Write(npc.localAI[1]);
            writer.Write(npc.localAI[2]);
            writer.Write(npc.localAI[3]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.localAI[0] = reader.ReadSingle();
            npc.localAI[1] = reader.ReadSingle();
            npc.localAI[2] = reader.ReadSingle();
            npc.localAI[3] = reader.ReadSingle();
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale);
            npc.defense = 19;
        }
        public override void AI()
        {
            // 0 1 2 5
            // npc.ai definitions:
            // 0 = basic timer for everything
            // 1 = another basic timer for everything
            // 2 = current npc state: 0 = phase 1, 1 = phase 2 anim, 2 = phase 2
            // localai is just extra timers
            Player player = Main.player[npc.target];
            if (!player.active || player.dead || npc.Distance(player.Center) > 6000)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead || npc.Distance(player.Center) > 6000)
                {
                    // despawn
                    npc.velocity.X *= 0.9f;
                    npc.velocity.Y -= 0.5f;
                    if(npc.timeLeft > 20)
                    {
                        npc.timeLeft = 20;
                        if (Main.raining)
                        {
                            Main.raining = false;
                            Main.rainTime = 0;
                        }
                    }
                }
            }
            // handle phase 2
            if (npc.ai[2] == 0 && npc.life < npc.lifeMax / 2)
            {
                npc.ai[0] = 0;
                npc.ai[1] = 0;
                npc.ai[2] = 1;
            }
            // phase 2 anim
            if (npc.ai[2] == 1)
            {
                npc.ai[0]++;
                if (npc.ai[0] == 1)
                {
                    if (Main.netMode == NetmodeID.SinglePlayer) // Singleplayer
                    {
                        Main.NewText("The air crackles with electricity...", Color.Teal);
                    }
                    else if (Main.netMode == NetmodeID.Server) // Server
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("The air crackles with electricity..."), Color.Teal);
                    }
                }
                npc.velocity *= 0.99f;
                npc.dontTakeDamage = true;
                if (!Main.raining)
                {
                    Main.raining = true;
                    Main.rainTime = 180;
                }
                else
                {
                    Main.rainTime += 120;
                }
                if (npc.ai[0] == 70)
                {
                    Main.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.Roar, 0), (int)npc.position.X, (int)npc.position.Y);
                }
                if (npc.ai[0] > 170)
                {
                    npc.ai[0] = 0;
                    npc.ai[2] = 2;
                }
                else
                {
                    return;
                }
            }
            // spawn lightning if phase 2
            if (npc.ai[2] == 2)
            {
                npc.localAI[1]++;
                if (npc.localAI[1] > 120)
                {
                    npc.localAI[1] = 0;
                    int amount = Main.expertMode ? 4 : 6;
                    int count = Main.rand.Next(2, amount);
                    for (int i = 0; i < count; i++)
                    {
                        Projectile.NewProjectile(player.Center + Vector2.UnitX * Main.rand.Next(-600, 600), Vector2.UnitY, ModContent.ProjectileType<LaserWarning>(), (int)(npc.damage / (Main.expertMode ? 3 : 6)), 1f);
                    }
                }
            }
            switch (npc.aiAction)
            {
                case 0:
                // basic movement above player and select dash direction
                npc.ai[0]++;
                npc.direction = npc.spriteDirection = player.Center.X > npc.Center.X ? 1 : -1;
                float dist = Vector2.Distance(npc.Center, player.Center);
                float speed = dist > 600f ? 16 : 10;
                npc.velocity = npc.DirectionTo(player.Center - Vector2.UnitY * 350f) * speed;
                if (npc.ai[0] > 240)
                {
                    npc.ai[0] = 0;
                    npc.aiAction++;
                    // dash direction
                    npc.localAI[0] = Main.rand.NextBool() ? -1 : 1; // 0 or 1
                }
                break;
                case 1:
                // npc.ai[1] is the current dash
                // npc.localAI[0] is the direction
                if (npc.ai[1] == 0)
                {
                    if (npc.ai[0] < 300)
                    {
                        npc.velocity = npc.DirectionTo(player.Center - Vector2.UnitX * 450 * npc.localAI[0]) * 10;
                        npc.direction = npc.spriteDirection = player.Center.X > npc.Center.X ? 1 : -1;
                        if (npc.ai[0] == 150)
                        {
                            Vector2 position = npc.Center - Vector2.UnitX * 98.5f * npc.direction - Vector2.UnitY * 48.5f;
                            float speed3 = npc.life <= npc.lifeMax * 0.70 ? 15 : 10;
                            // Projectile.NewProjectiles(position, npc.DirectionTo(player.Center) * speed3, ModContent.ProjectileType<LightningBreath>(), npc.damage / (Main.expertMode ? 5 : 4), 3f, Main.myPlayer);
                        }
                    }
                    else if (npc.ai[0] < 420)
                    {
                        // 4 second dash, 5 second preparation
                        npc.velocity.Y *= 0.99f;
                        npc.velocity.X = 900f / 240f * -npc.localAI[0];
                    }
                    else if (npc.ai[0] >= 420)
                    {
                        npc.ai[0] = 0;
                        npc.ai[1]++;
                    }
                }
                else if (npc.ai[1] == 1)
                {
                    if (npc.ai[0] < 300)
                    {
                        npc.velocity = npc.DirectionTo(player.Center - Vector2.UnitX * 450 * -npc.localAI[0]) * 10;
                        if (npc.ai[0] == 150)
                        {
                            Vector2 position = npc.Center - Vector2.UnitX * 98.5f * npc.direction - Vector2.UnitY * 48.5f;
                            float speed1 = npc.life <= npc.lifeMax * 0.70 ? 15 : 10;
                            // Projectile.NewProjectile(position, npc.DirectionTo(player.Center) * speed1, ModContent.ProjectileType<LightningBreath>(), npc.damage / (Main.expertMode ? 5 : 4), 3f, Main.myPlayer);
                        }
                    }
                    else if (npc.ai[0] < 420)
                    {
                        // 4 second dash, 5 second preparation
                        npc.velocity.Y *= 0.99f;
                        npc.velocity.X = 900f / 240f * npc.localAI[0];
                    }
                    else if (npc.ai[0] >= 420)
                    {
                        npc.ai[0] = 0;
                        npc.ai[1] = 0;
                        npc.aiAction++;
                    }
                }
                break;
                case 2:
                // b a l l s
                npc.direction = npc.spriteDirection = player.Center.X > npc.Center.X ? 1 : -1;
                npc.velocity *= 0.99f;
                npc.ai[0]++;
                if (npc.ai[0] == 1)
                {
                    int damage = Main.expertMode ? 12 : 20;
                    // Projectile.NewProjectile(npc.Center.X, npc.Center.Y - 50, 0, 0, ModContent.ProjectileType<ElectricBallCenter>(), damage, 1, Main.myPlayer, 0, npc.whoAmI);
                }
                if (npc.ai[0] > 20)
                {
                    npc.ai[0] = 0;
                    npc.aiAction++;
                }
                break;
                case 3:
                // accelerate towards player
                npc.direction = npc.spriteDirection = player.Center.X > npc.Center.X ? 1 : -1;
                npc.ai[0]++;
                float speed2 = npc.ai[2] == 2 ? 0.5f : 0.85f;
                if (npc.Center.X < player.Center.X)
                {
                    npc.velocity.X += speed2 * 2;
                }
                else
                {
                    npc.velocity.X -= speed2 * 2;
                }
                if (npc.Center.Y < player.Center.Y)
                {
                    npc.velocity.Y += speed2;
                }
                else
                {
                    npc.velocity.Y -= speed2;
                }
                if (Math.Abs(npc.velocity.X) > 16f)
                {
                    npc.velocity.X = 16f * Math.Sign(npc.velocity.X);
                }
                if (Math.Abs(npc.velocity.Y) > 16f)
                {
                    npc.velocity.Y = 16f * Math.Sign(npc.velocity.Y);
                }
                if (npc.ai[0] > 160)
                {
                    npc.ai[0] = 0;
                    npc.aiAction = 0;
                    if (npc.life < npc.lifeMax * 0.35f)
                    {
                        int damage = Main.expertMode ? 12 : 20;
                        // Projectile.NewProjectile(npc.Center.X, npc.Center.Y - 50, 0, 0, ModContent.ProjectileType<ElectricBallCenter>(), damage, 1, Main.myPlayer, 0, npc.whoAmI);
                    }
                }
                break;
            }
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if(npc.life <= 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, 206, 2 * hitDirection, -2f);
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].scale = 1.2f * npc.scale;
                    }
                    else
                    {
                        Main.dust[dust].scale = 0.7f * npc.scale;
                    }
                }

                Gore.NewGore(npc.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), Vector2.Zero, mod.GetGoreSlot("Gores/DrakeHead"), npc.scale);
                Gore.NewGore(npc.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), Vector2.Zero, mod.GetGoreSlot("Gores/DrakeWing"), npc.scale);
                Gore.NewGore(npc.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), Vector2.Zero, mod.GetGoreSlot("Gores/DrakeLeg"), npc.scale);
                Gore.NewGore(npc.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), Vector2.Zero, mod.GetGoreSlot("Gores/DrakeTail"), npc.scale);
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            // Phase 2 Debuff
            if (npc.ai[2] == 2)
            {
                target.AddBuff(BuffID.Electrified, Main.expertMode ? 240 : 120);
            }
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
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = Main.npcTexture[npc.type];
            SpriteEffects effects = npc.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 origin = npc.frame.Size() / 2;
            spriteBatch.Draw(texture, npc.Center - Main.screenPosition, npc.frame, npc.GetAlpha(drawColor), npc.rotation, origin, npc.scale, effects, 0f);

            if (npc.aiAction == 1 || npc.aiAction == 2 || npc.aiAction == 3)
            {
                for (int i = 0; i < npc.oldPos.Length; i++)
                {
                    Vector2 pos = npc.oldPos[i] - Main.screenPosition + npc.Size / 2;
                    spriteBatch.Draw(texture, pos, npc.frame, Color.White * ((float)i / npc.oldPos.Length), npc.rotation, origin, npc.scale, effects, 0f);
                }
            }
            Texture2D texture2 = mod.GetTexture("NPCs/Bosses/StormDrake/StormDrake_Glowmask");
            spriteBatch.Draw(texture2, npc.Center, npc.frame, Color.White, npc.rotation, origin, npc.scale, effects, 0.01f);
            return false;
        }
        public override void NPCLoot()
        {
            //OvermorrowWorld.downedDrake = true;

            if (Main.raining)
            {
                Main.raining = false;
                Main.rainTime = 0;
            }

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
                    //Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<LightningPiercer>());
                }
                else if (choice == 1) // Mage
                {
                    //Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<BoltStream>());
                }
                else if (choice == 2) // Warrior
                {
                    //Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<StormTalon>());
                }
                else if (choice == 3) // Ranger
                {
                    //Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<TempestGreatbow>());
                }
                else if (choice == 4) // Summoner
                {
                    //Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<DrakeStaff>());
                }

                if (Main.rand.Next(10) == 0) // Trophy Dropchance
                {
                    //Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<DrakeTrophy>());
                }
            }
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }
    }
}