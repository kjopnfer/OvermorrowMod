using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.StormDrake
{
    public class ElectricBallCenter : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        private bool spawnedProjectiles = false;
        private int storeDamage = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Ball");
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 720;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0) // Make this projectile deal no damage
            {
                storeDamage = Projectile.damage;
                Projectile.damage = 0;
            }

            // Unnecessary code
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            if (Projectile.ai[0] < 240) // Stay still for 120 seconds
            {
                Projectile.velocity = new Vector2(0, 0);

                // Get the ID of the Parent NPC that was passed in via AI[1]
                NPC parent = Main.npc[(int)Projectile.ai[1]];
                Projectile.Center = parent.Center;
                Projectile.netUpdate = true;

                if (Projectile.ai[0] == 239)
                {
                    SoundEngine.PlaySound(SoundID.Roar, Projectile.position);
                }
            }
            else // Launch at the nearest player
            {
                Vector2 move = Vector2.Zero;
                float distance = 6000f; // Search distance
                if (Projectile.ai[0] < 710/*!launchedProjectile*/)
                {
                    Vector2 direction = Main.player[Main.npc[(int)Projectile.ai[1]].target].Center - Projectile.Center;
                    float distanceTo = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
                    if (distanceTo < distance)
                    {
                        move = direction;
                        direction.SafeNormalize(Vector2.Zero);
                        float launchSpeed = Projectile.ai[0] > 480 ? 0.05f : 0.025f /*75f : 100f*/;
                        direction *= launchSpeed;
                        distance = distanceTo;
                        //projectile.velocity = (move) / launchSpeed;
                        float inertia = 150f;
                        Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                    }
                }
            }

            if (Projectile.ai[0] == 710)
            {
                Projectile.velocity *= 1.5f;
            }


            if (!spawnedProjectiles)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        // AI[0] is the ID of the parent projectile, AI[1] is the degree of the initial position in a circle 
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ElectricBall>(), storeDamage, 1, Main.myPlayer, Projectile.whoAmI, 30f * i);
                    }
                }
                spawnedProjectiles = true;
                Projectile.netUpdate = true;
            }

            Projectile.ai[0]++;
        }
    }
}