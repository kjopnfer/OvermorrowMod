using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.NPCs.Bosses;
using OvermorrowMod.NPCs.Bosses.DripplerBoss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Boss
{
    public class DripplerSpawner : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Drippler Spawner");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 120;
            projectile.alpha = 255;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 3f)
            {
                int dust = Dust.NewDust(projectile.Center, projectile.width, projectile.height, 5, 0, 0, 0, default, 1.84f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.ai[0] == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int spawnedNPC = NPC.NewNPC((int)(projectile.Center.X), (int)(projectile.Center.Y), ModContent.NPCType<LoomingDrippler>(), 0, 0, projectile.ai[1]);
                    
                    if(Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, spawnedNPC);
                    }

                    Main.npc[spawnedNPC].netUpdate = true;
                }
            }
            else
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int spawnedNPC = NPC.NewNPC((int)(projectile.Center.X), (int)(projectile.Center.Y), ModContent.NPCType<Driplad>(), 0, 0, projectile.ai[1]);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, spawnedNPC);
                    }

                    Main.npc[spawnedNPC].netUpdate = true;
                }
            }
        }
    }
}