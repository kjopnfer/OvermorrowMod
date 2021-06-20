using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.PostRider
{
    public class AngryIceShards : ModNPC
    {

        int randomX = 0;
        int randomY = 0;
        int Attacktimer = 4;
        readonly bool expert = Main.expertMode;
        private int experttimer = 0;
        private readonly int timer = 0;

        public override void SetDefaults()
        {
            npc.width = 102;
            npc.height = 102;
            npc.aiStyle = 26;
            aiType = NPCID.Tumbleweed;
            animationType = NPCID.Tumbleweed;
            npc.damage = 45;
            npc.defense = 6;
            npc.lifeMax = 300;
            npc.HitSound = SoundID.NPCHit25;
            npc.DeathSound = SoundID.NPCDeath54;
            npc.value = 25f;
            npc.lavaImmune = true;
            npc.buffImmune[BuffID.OnFire] = true;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Angry Ice Shard");
            Main.npcFrameCount[npc.type] = 1;
        }
        public override void NPCLoot()
        {
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("FrostEssence"));
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
            if(Attacktimer == 75)
            {
                npc.velocity.X = 0;
                npc.velocity.Y = 0;
                Vector2 position = npc.Center;
                Vector2 targetPosition = Main.player[npc.target].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                float speed = 10f;
                int damage = 0;
                if (!expert)
                {
                    damage = npc.damage;
                }
                else
                {
                    damage = npc.damage / 2;
                }
                Vector2 direction2 = new Vector2(direction.X, direction.Y).RotatedBy(MathHelper.ToRadians(90f));
                Vector2 direction3 = new Vector2(direction.X, direction.Y).RotatedBy(MathHelper.ToRadians(-90f));
                Vector2 direction4 = new Vector2(direction.X, direction.Y).RotatedBy(MathHelper.ToRadians(180f));
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, direction.X * speed, direction.Y * speed, mod.ProjectileType("AngryFrostWave"), damage, 0f, Main.myPlayer);
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, direction2.X * speed, direction2.Y * speed, mod.ProjectileType("AngryFrostWave"), damage, 0f, Main.myPlayer);
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, direction3.X * speed, direction3.Y * speed, mod.ProjectileType("AngryFrostWave"), damage, 0f, Main.myPlayer);
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, direction4.X * speed, direction4.Y * speed, mod.ProjectileType("AngryFrostWave"), damage, 0f, Main.myPlayer);
                Main.PlaySound(SoundID.Item8, npc.position);
                Attacktimer = 0;
            }
        }


        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return Main.hardMode == true && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && spawnInfo.player.ZoneSnow && spawnInfo.player.ZoneOverworldHeight ? 0.4f : 0f;
        }
    }
}

