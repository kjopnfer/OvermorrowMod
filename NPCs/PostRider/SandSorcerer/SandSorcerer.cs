using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.PostRider.SandSorcerer
{
    public class SandSorcerer : ModNPC
    {
        int Attacktimer = 4;
        readonly bool expert = Main.expertMode;
        private int experttimer = 0;
        int damage = 45;
        public override void SetDefaults()
        {
            npc.width = 30;
            npc.height = 56;
            npc.damage = 45;
            npc.defense = 6;
            npc.lifeMax = 500;
            npc.HitSound = SoundID.NPCHit25;
            npc.DeathSound = SoundID.NPCDeath54;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.buffImmune[BuffID.OnFire] = true;
            aiType = NPCID.NebulaBrain;
            npc.aiStyle = 97;//-1;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand Sorcerer");
        }
        public override void AI()
        {




            experttimer++;
            if (expert && experttimer == 1)
            {
                npc.life = 600;
                npc.lifeMax = 600;
                npc.damage = 55;
            }

            Attacktimer++;
            int type = mod.ProjectileType("SandBolt2");
            if (expert)
            {
                damage = 32;
            }
            else
            {
                damage = 50;
            }

            if (Attacktimer == 10)
            {
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, 0, type, damage, 0f, Main.myPlayer);
                Main.PlaySound(SoundID.Item8, npc.position);
            }
            if (Attacktimer == 20)
            {
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, 0, type, damage, 0f, Main.myPlayer);
                Main.PlaySound(SoundID.Item8, npc.position);
            }
            if (Attacktimer == 30)
            {
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, 0, type, damage, 0f, Main.myPlayer);
                Main.PlaySound(SoundID.Item8, npc.position);
            }
            if (Attacktimer == 40)
            {
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, 0, type, damage, 0f, Main.myPlayer);
                Main.PlaySound(SoundID.Item8, npc.position);
            }
            if (Attacktimer == 50)
            {
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, 0, type, damage, 0f, Main.myPlayer);
                Main.PlaySound(SoundID.Item8, npc.position);
            }
            if (Attacktimer == 60)
            {
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, 0, type, damage, 0f, Main.myPlayer);
                Main.PlaySound(SoundID.Item8, npc.position);
            }
            if (Attacktimer == 70)
            {
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, 0, type, damage, 0f, Main.myPlayer);
                Main.PlaySound(SoundID.Item8, npc.position);
            }
            if (Attacktimer == 80)
            {
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, 0, type, damage, 0f, Main.myPlayer);
                Main.PlaySound(SoundID.Item8, npc.position);
            }
            if (Attacktimer == 90)
            {
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, 0, type, damage, 0f, Main.myPlayer);
                Main.PlaySound(SoundID.Item8, npc.position);
            }
            if (Attacktimer == 100)
            {
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, 0, type, damage, 0f, Main.myPlayer);
                Main.PlaySound(SoundID.Item8, npc.position);
            }

            if (Attacktimer == 300)
            {
                Attacktimer = 0;
            }
        }

        public override void NPCLoot()
        {
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DesertEssence"));
        }
        public override void FindFrame(int frameHeight)
        {
            npc.spriteDirection = npc.direction;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return Main.hardMode == true && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && spawnInfo.player.ZoneDesert && spawnInfo.player.ZoneOverworldHeight ? 0.15f : 0f;
        }
    }
}

