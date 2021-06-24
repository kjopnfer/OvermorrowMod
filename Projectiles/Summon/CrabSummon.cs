
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Summon
{
    public class CrabSummon : ModProjectile
    {

        private int Shards = 0;

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.SoulscourgePirate);
            aiType = ProjectileID.SoulscourgePirate;
            projectile.netImportant = true;
            projectile.minion = true;
            projectile.height = 42;
            projectile.minionSlots = 0f;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crab Summon");
            Main.projFrames[base.projectile.type] = 15;
        }

        int RandomCRY = Main.rand.Next(-5, 6);
        int RandomCRYY = Main.rand.Next(-5, 6);
        public override void AI()
        {
            RandomCRYY = Main.rand.Next(-5, 6);
            RandomCRY = Main.rand.Next(-2, 2);
            if(Shards > 0)
            {
                Vector2 value1 = new Vector2(0f, 0f);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, mod.ProjectileType("CrabBoom"), projectile.damage * 2, 1f, projectile.owner, 0f);
                Shards = 0;
            }


            projectile.minion = true;
            projectile.minionSlots = 0f;

            projectile.damage = Main.player[projectile.owner].statDefense / 2;
            projectile.height = 37;
        }




        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Shards++;
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            {
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = 0;
                }
            }
            return false;
        }
    }
}
