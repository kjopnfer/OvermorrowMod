using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Projectiles.Boss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.StormDrake
{
    public class StormDrake : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Drake");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.width = 296;
            npc.height = 232;
            npc.aiStyle = -1;
            npc.damage = 45;
            npc.defense = 12;
            npc.lifeMax = 7300;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.DD2_BetsyDeath;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.boss = true;
            npc.value = Item.buyPrice(gold: 5);
            npc.npcSlots = 5f;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale);
            npc.damage = (int)(npc.damage * 1.3f);
        }

        public override void AI()
        {
            Player player = Main.player[npc.target];

            if (!player.active || player.dead)
            {
                npc.TargetClosest(false);
                npc.velocity.Y = -2000;
            }

            npc.spriteDirection = npc.direction;
            npc.ai[1]++;
            switch (npc.ai[0])
            {
                case 0: // Move towards player
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if(npc.ai[1] == 300)
                        {
                            npc.ai[0] = 1;
                            npc.ai[1] = 0;
                        }

                        npc.TargetClosest(true);
                        Vector2 target = npc.HasPlayerTarget ? player.Center : Main.npc[npc.target].Center;
                        var move = target - npc.Center;
                        var speed = 3;
                        float length = move.Length();
                        if (length > speed)
                        {
                            move *= speed / length;
                        }
                        var turnResistance = 30;
                        move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
                        length = move.Length();
                        if (length > 10)
                        {
                            move *= speed / length;
                        }
                        npc.velocity = move;
                        npc.netUpdate = true;
                    }
                    break;
                case 1: // Fire electric balls
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.velocity = new Vector2(0, 0);
                        if (npc.ai[1] % 301 == 1)
                        {
                            Main.PlaySound(SoundID.Item94, (int)npc.position.X, (int)npc.position.Y);
                            int damage = Main.expertMode ? 12 : 20;
                            Vector2 target = Main.player[npc.target].Center - npc.Center;
                            target.Normalize();

                            int direction = npc.direction == 1 ? 175 : -175; // Facing right, otherwise
                            
                            Projectile.NewProjectile(npc.Center.X, npc.Center.Y - 50, 0, 0, ModContent.ProjectileType<ElectricBallCenter>(), 0, 1, Main.myPlayer, 0, damage);
                        }

                        if(npc.ai[1] == 300)
                        {
                            npc.ai[0] = 0;
                            npc.ai[1] = 0;
                        }
                    }
                    break;
                case 2: // Position on the left or right side of the player
                    break;
            }         

            // Phase 1
            // Shoot erratic moving electric balls that home in on player before continuing in one direction
            // Dash from one side to the other

            // PHASE CHANGE: Duke Fishron pulse effect

            // Phase 2
            // Calls columns of lightning down from the sky
            // Dash from one side to the other, summons clone projectiles above and bottom to dash after a delay

            // EXPERT MODE: Phase 3
            // EXPERT VERSION: Summon a lightning clone with similar movements
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

        public override void NPCLoot()
        {
            int choice = Main.rand.Next(2);

            // Always drops one of:
            if (choice == 0)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("LightningPiercer"));
            }
            else if (choice == 1)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("LightningPiercer"));
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }
    }
}