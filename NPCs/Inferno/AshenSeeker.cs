using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Inferno
{
    public class AshenSeeker : ModNPC
    {
        public override void SetStaticDefaults()
        {

            Main.npcFrameCount[npc.type] = 1;
        }
        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.width = 14;
            npc.height = 14;
            npc.aiStyle = -1;
            npc.scale = 2.2f;
            npc.damage = 27;
            npc.defense = 3;
            npc.lifeMax = 75;
            npc.HitSound = SoundID.NPCHit25;
            npc.DeathSound = SoundID.NPCDeath54;
            npc.value = 25f;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.lavaImmune = true;

        }
        public override void AI()
        {
            float speed = 0.1f;
            npc.TargetClosest(true);
            Vector2 targetPosition = Main.player[npc.target].position;

            if (targetPosition.X < npc.position.X & npc.velocity.X >= -5)
            {
                npc.velocity.X -= speed;
            }

            if (targetPosition.Y < npc.position.Y & npc.velocity.Y >= -5)
            {
                npc.velocity.Y -= speed;
            }

            if (targetPosition.X > npc.position.X & npc.velocity.X <= 5)
            {
                npc.velocity.X += speed;
            }


            if (targetPosition.Y > npc.position.Y & npc.velocity.Y <= 5)
            {
                npc.velocity.Y += speed;
            }

            if (npc.velocity.X > 4f)
            {
                npc.velocity.X = 0f;
            }
            if (npc.velocity.X < -4f)
            {
                npc.velocity.X = 0f;
            }

            if (npc.velocity.Y > 4f)
            {
                npc.velocity.Y = 0f;
            }
            if (npc.velocity.Y < -4f)
            {
                npc.velocity.Y = 0f;
            }
        }
    }
}
