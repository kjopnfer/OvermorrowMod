using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Ranged
{
    // This ModNPC serves as an example of a complete AI example.
    public class SpikeSling : ModNPC
    {

        readonly bool expert = Main.expertMode;
        private int experttimer = 0;

        public override void SetDefaults()
        {
            npc.width = 34;
            npc.height = 34;
            npc.aiStyle = 21;
            npc.dontTakeDamage = true;
            npc.friendly = true;
            npc.damage = 10;
            npc.lifeMax = 3;
            npc.HitSound = SoundID.NPCHit25;
            npc.DeathSound = SoundID.NPCDeath54;
            npc.AddBuff(BuffID.Poisoned, 120);
            npc.alpha = 0;
            npc.color = Color.Green;
        }

        public override void AI()
        {
            float distanceFromTarget = 130f;
            Vector2 targetCenter = npc.position;
            bool foundTarget = false;
            // This code is required either way, used for finding a target
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC tar = Main.npc[i];
                if (tar.friendly == false)
                {
                    float betweenatt = Vector2.Distance(tar.Center, npc.Center);
                    float between = Vector2.Distance(tar.Center, Main.player[npc.target].Center);
                    bool closest = Vector2.Distance(npc.Center, targetCenter) > between;
                    bool inRange = between < distanceFromTarget;
                    if (betweenatt < 34)
                    {
                        tar.StrikeNPC(npc.damage, 1, 0);
                        npc.dontTakeDamage = false;
                        npc.life = -3000;
                    }


                    if (((closest && inRange) || !foundTarget))
                    {
                        distanceFromTarget = between;
                        targetCenter = tar.Center;
                        foundTarget = true;
                    }
                }
            }
            experttimer++;

            if (npc.velocity.Y != 0)
            {
                if (Main.MouseWorld.X < Main.player[npc.target].Center.X)
                {
                    npc.velocity.X = -5;
                }
                if (Main.MouseWorld.X > Main.player[npc.target].Center.X)
                {
                    npc.velocity.X = 5;
                }
                npc.damage = 5;
            }

        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Roller");
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(50))
            {
                Item.NewItem(npc.getRect(), ItemID.Gel, 4);
            }
        }
    }
}

