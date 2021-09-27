using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Town
{
    public class LightDisc2 : ModProjectile
    {
        private bool ComingBack = false;
        private int flametimer = 0;

        public override string Texture => "Terraria/Projectile_" + ProjectileID.LightDisc;


        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.timeLeft = 100;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.melee = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Light Disc");
        }


        public override void AI()
        {
            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.type == mod.NPCType("Guide2"))
                {

                    projectile.rotation += 0.36f;

                    if (projectile.timeLeft < 65)
                    {
                        projectile.timeLeft = 10;
                        ComingBack = true;
                    }

                    if (projectile.timeLeft > 98)
                    {
                        projectile.tileCollide = false;
                    }
                    else if (!ComingBack)
                    {
                        projectile.tileCollide = true;
                    }

                    if (ComingBack)
                    {
                        flametimer++;
                        float BetweenKill = Vector2.Distance(npc.Center, projectile.Center);
                        projectile.tileCollide = false;
                        Vector2 position = projectile.Center;
                        Vector2 targetPosition = npc.Center;
                        Vector2 direction = targetPosition - position;
                        direction.Normalize();
                        projectile.velocity = direction * 18;
                        if (BetweenKill < 22)
                        {
                            projectile.Kill();
                        }
                    }
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Vector2 eee = projectile.Center;
            Main.PlaySound(SoundID.Item54, (int)eee.X, (int)eee.Y);
            {
                ComingBack = true;
            }
            return false;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            Vector2 eeee = projectile.Center;
            ComingBack = true;
        }
    }
}
