using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Town
{
    public class Twin2NPC : ModNPC
    {
        private int AttTimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Retanimini");
            Main.npcFrameCount[npc.type] = 3;
        }

        public override void SetDefaults()
        {
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.width = 22;
            npc.height = 50;
            npc.damage = 28;
            npc.defense = 6;
            npc.lifeMax = 100;
            animationType = NPCID.CursedSkull;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath4;
            npc.value = 5f;
            npc.knockBackResist = 0.4f;
            npc.aiStyle = 5;
            aiType = NPCID.Crimera;
        }




        public override void AI()
        {
            Player player = Main.player[npc.target];
            AttTimer++;
            if(AttTimer == 100)
            {
                Vector2 position = npc.Center;
                Vector2 targetPosition = Main.player[npc.target].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                float speed = 10f;
                Projectile.NewProjectile(position, direction * speed, ProjectileID.PinkLaser, npc.damage, 0f, Main.myPlayer);  
                AttTimer = 0;
            }
        }
    }
}

