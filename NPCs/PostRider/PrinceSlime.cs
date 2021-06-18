using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.GameContent.Events;
using Terraria.ID;

namespace OvermorrowMod.NPCs.PostRider
{
    // This ModNPC serves as an example of a complete AI example.
    public class PrinceSlime : ModNPC
    {
        readonly bool expert = Main.expertMode;
        private int jumptimer = 0;
        int spritetimer = 0;
        int frame = 1;
        bool jumping = false;
        int flyspritetimer = 0;

        int spritecatch = 0;
        int spritecatch2 = 0;
        int expertBS = 0;
        int attacktimer = 0;
        int RandomX = Main.rand.Next(-5, 6);
        int RandomY = Main.rand.Next(-1, 4);
        public override void SetDefaults()
        {
            npc.width = 42;
            npc.height = 34;
            npc.damage = 27;
            npc.defense = 10;
            npc.lifeMax = 175;
            npc.HitSound = SoundID.NPCHit25;
            npc.DeathSound = SoundID.NPCDeath54;
            npc.alpha = 0;
            npc.noTileCollide = false;
            npc.noGravity = false;
            npc.value = 25f;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.AddBuff(BuffID.Poisoned, 120);
        }



        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prince Slime");
            Main.npcFrameCount[npc.type] = 6;
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = frameHeight * frame;
            if (jumptimer > 150)
            {
                npc.spriteDirection = -npc.direction;
                npc.rotation = npc.velocity.X * 0.04f;
            }
            else
            {
                npc.spriteDirection = -1;
                npc.rotation = 0;
            }
        }
        public override void AI()
        {
            if (expert)
            {
                int expertBS = 10;
            }


            npc.TargetClosest(true);
            jumptimer++;



            if (jumptimer > 150)
            {
                spritecatch2++;
                if (spritecatch2 == 1)
                {
                    frame = 3;
                }
                spritecatch = 0;
                flyspritetimer++;
                if (flyspritetimer > 5)
                {
                    frame++;
                    flyspritetimer = 0;
                }
                if (frame > 5)
                {
                    frame = 3;
                }
            }

            else
            {
                spritecatch++;
                spritecatch2 = 0;
                if (spritecatch == 1)
                {
                    frame = 0;
                }
                spritetimer++;
                if (spritetimer > 5)
                {
                    frame++;
                    spritetimer = 0;
                }
                if (frame > 1)
                {
                    frame = 0;
                }

            }


            RandomX = Main.rand.Next(-1, 2);
            RandomY = Main.rand.Next(-1, 4);
            attacktimer++;
            if (jumptimer < 150 && attacktimer == 10)
            {
                Vector2 value1 = new Vector2(0f, -7f);
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, value1.X + RandomX, value1.Y + RandomY, 605, npc.damage / 2 - expertBS, 1f);
            }
            if (attacktimer > 10)
            {
                attacktimer = 0;
            }

            if (jumptimer > 150 && Main.player[npc.target].position.X < npc.position.X)
            {
                npc.velocity.Y -= 0.7f;
                npc.velocity.X -= 0.7f;
            }
            if (jumptimer > 150 && Main.player[npc.target].position.X > npc.position.X)
            {
                npc.velocity.Y -= 0.7f;
                npc.velocity.X += 0.7f;
            }

            if (jumptimer > 165)
            {
                npc.velocity.Y += 0.7f;
            }

            if (jumptimer == 180)
            {
                jumptimer = 0;
            }
        }
        public override void NPCLoot()
        {
            Item.NewItem(npc.getRect(), ItemID.Gel, 50);
        }


        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return Main.hardMode == true && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && spawnInfo.player.ZoneOverworldHeight && Main.dayTime ? SpawnCondition.OverworldDaySlime.Chance * 0.3f : 0f;
        }
    }
}


