using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Boss;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

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
                if (!Main.raining)
                {
                    Main.raining = true;
                    Main.rainTime = 180;
                }
                npc.ai[0] = 0;
                npc.ai[1] = 0;
                npc.ai[2] = 1;
            }
            // spawn lightning if phase 2
            if (npc.ai[2] == 1)
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
                case 3:
                
                break;
            }
        }
    }
}