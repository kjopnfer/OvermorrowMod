using Microsoft.Xna.Framework;
using OvermorrowMod.Content.NPCs.Bosses.TreeBoss;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Boss
{
    public class AbsorbEnergy : ModNPC
    {
        public override string Texture => AssetDirectory.Empty;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Natural Energy");
        }

        public override void SetDefaults()
        {
            npc.width = 10;
            npc.height = 10;
            npc.alpha = 255;
            npc.friendly = false;
            npc.noTileCollide = true;
            npc.timeLeft = 270;
            npc.lifeMax = 10;
        }

        public override void AI()
        {
            npc.damage = (int)npc.ai[3];

            // Get the ID of the Parent NPC that was passed in via AI[1]
            NPC parent = Main.npc[(int)npc.ai[1]];

            Vector2 move = Vector2.Zero;
            Vector2 newMove = parent.Center - npc.Center;
            float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
            move = newMove;
            float launchSpeed = Main.expertMode ? 60 : 75f;
            npc.velocity = (move) / launchSpeed;


            /*for (int num1101 = 0; num1101 < 3; num1101++)
            {
                int num1110 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 107, npc.velocity.X, npc.velocity.Y, 50, default(Color), 1.2f);
                Main.dust[num1110].position = (Main.dust[num1110].position + npc.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;
            }*/

            for (int num1103 = 0; num1103 < 2; num1103++)
            {
                int num1106 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.TerraBlade, npc.velocity.X, npc.velocity.Y, 50, default(Color), 0.4f);
                switch (num1103)
                {
                    case 0:
                        Main.dust[num1106].position = (Main.dust[num1106].position + npc.Center * 5f) / 6f;
                        break;
                    case 1:
                        Main.dust[num1106].position = (Main.dust[num1106].position + (npc.Center + npc.velocity / 2f) * 5f) / 6f;
                        break;
                }
                Dust dust81 = Main.dust[num1106];
                dust81.velocity *= 0.1f;
                Main.dust[num1106].noGravity = true;
                Main.dust[num1106].fadeIn = 1f;
            }

            if (npc.Distance(parent.Center) < 25)
            {
                npc.ai[2] = 1;
                npc.life = 0;
                npc.active = false;
            }
        }

        public override void NPCLoot()
        {
            // Get the ID of the Parent NPC that was passed in via AI[1]
            NPC parent = Main.npc[(int)npc.ai[1]];
            if (parent.life < parent.lifeMax && npc.ai[2] == 1)
            {
                parent.life += 5;
            }
            if (npc.ai[2] == 1)
            {
                ((TreeBossP2)Main.npc[(int)npc.ai[1]].modNPC).energiesAbsorbed += 1;
            }
            else
            {
                ((TreeBossP2)Main.npc[(int)npc.ai[1]].modNPC).energiesKilled += 1;
            }
        }
    }
}