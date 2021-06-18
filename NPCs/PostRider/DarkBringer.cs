using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;



namespace OvermorrowMod.NPCs.PostRider
{
    public class DarkBringer : ModNPC
    {

        readonly bool expert = Main.expertMode;
        private int experttimer = 0;
        int spritetimer = 0;
        int frame = 1;
        int timer = 0;

        public override void SetDefaults()
        {
            npc.damage = 45;
            npc.defense = 8;
            npc.lifeMax = 250;
            npc.aiStyle = NPCID.EaterofSouls;
            npc.knockBackResist = 0.5f;
            npc.width = 30;
            npc.height = 20;
            npc.scale = 2f;
            npc.noTileCollide = false;
            npc.noGravity = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath2;
            npc.value = Item.buyPrice(0, 0, 14, 7);
        }

        int damage = 65;

        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = frameHeight * frame;
            if(Main.player[npc.target].position.X < npc.position.X)
            {
                npc.spriteDirection = -1;
            }
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wormruptor");
            Main.npcFrameCount[npc.type] = 2;
        }
        
        public override void AI()
        {

            npc.TargetClosest(true);

            if (Main.player[npc.target].position.X < npc.position.X)
            {
                npc.velocity.X -=  0.1f;
            }

            if (Main.player[npc.target].position.Y < npc.position.Y )
            {
                npc.velocity.Y -= 0.3f;
            }

            if (Main.player[npc.target].position.X > npc.position.X)
            {
                npc.velocity.X += 0.1f;
            }

            if (Main.player[npc.target].position.Y > npc.position.Y)
            {
                npc.velocity.Y += 0.1f;
            }

            if(npc.velocity.X > 4f)
            {
                npc.velocity.X = 4f;
            }
            if(npc.velocity.X < -4f)
            {
                npc.velocity.X = -4f;
            }

            if(npc.velocity.Y > 4f)
            {
                npc.velocity.Y = 4f;
            }
            if(npc.velocity.Y < -4f)
            {
                npc.velocity.Y = -4f;
            }

            npc.noTileCollide = false;
            npc.noGravity = true;
                experttimer++;
                if(expert && experttimer == 1)
                {
                    npc.life = 500;
                    npc.lifeMax = 500;
                    npc.damage = 75;
                }


                spritetimer++;
                if(spritetimer > 5)
                {
                    frame++;
                    spritetimer = 0;
                }
                if(frame > 1)
                {
                    frame = 0;
                }


            npc.rotation = (Main.player[npc.target].Center - npc.Center).ToRotation();
            timer++;
            if(timer == 54)
            {
                Vector2 position = npc.Center;
                Vector2 targetPosition = Main.player[npc.target].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                float speed = 7f;
                int type = mod.ProjectileType("WormHeadHostile");
                Main.PlaySound(4, npc.position, 13);
                if(!expert)
                {
                    damage = npc.damage;
                }
                else
                {
                    damage = npc.damage / 2;
                }

                Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
            }
            if(timer == 100)
            {
                timer = 0;
            }
        }

        public override void NPCLoot()
        {
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CorruptEssence"));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return Main.hardMode == true && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && spawnInfo.player.ZoneCorrupt ? 0.47f : 0f;
        }
    }
}


