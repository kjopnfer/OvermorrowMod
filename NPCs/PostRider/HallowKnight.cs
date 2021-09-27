using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.PostRider
{
    public class HallowKnight : ModNPC
    {



        int randomX = 0;
        int randomY = 0;
        int Attacktimer = 4;
        readonly bool expert = Main.expertMode;
        private int experttimer = 0;

        public override void SetDefaults()
        {
            npc.width = 20;
            npc.height = 56;
            npc.aiStyle = 3;
            aiType = NPCID.GoblinScout;
            animationType = NPCID.ChaosElemental;
            npc.damage = 45;
            npc.defense = 6;
            npc.lifeMax = 500;
            npc.HitSound = SoundID.NPCHit25;
            npc.DeathSound = SoundID.NPCDeath54;
            npc.value = 25f;
            npc.lavaImmune = true;
            npc.buffImmune[BuffID.OnFire] = true;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hallow Knight");
            Main.npcFrameCount[npc.type] = 15;
        }
        public override void AI()
        {

                experttimer++;
                if(expert && experttimer == 1)
                {
                    npc.life = 600;
                    npc.lifeMax = 600;
                    npc.damage = 55;
                }



            Attacktimer++;
            if(Attacktimer > 114)
            {
                randomX = Main.rand.Next(-2, 3);
                randomY = Main.rand.Next(-1, 2);
                Vector2 position = npc.Center;
                Vector2 targetPosition = Main.player[npc.target].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                float speed = 7.7f;
                int damage = 65;
                if (!expert)
                {
                    damage = npc.damage;
                }
                else
                {
                    damage = npc.damage / 2 + 10;
                }
                Vector2 directionrand = new Vector2(direction.X, direction.Y).RotatedByRandom(MathHelper.ToRadians(7f));
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, directionrand.X * speed, directionrand.Y * speed, mod.ProjectileType("RainbowBulletHostile"), damage, 0f, Main.myPlayer);
                Main.PlaySound(SoundID.Item36, npc.position);
                npc.velocity.X = 0;
            }
            if(Attacktimer == 120)
            {
                Attacktimer = 0;
            }
        }

        public override void NPCLoot()
        {
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("HallowEssence"));
        }
        public override void FindFrame(int frameHeight)
        {
            npc.spriteDirection = npc.direction;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return Main.hardMode == true && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && spawnInfo.player.ZoneHoly ? 0.45f : 0f;
        }
    }
}

