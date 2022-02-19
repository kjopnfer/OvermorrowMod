using Terraria.ID;
using Terraria.ModLoader;


namespace OvermorrowMod.Items.Weapons.MechWep.MechProj
{
    public class SawBlade : ModProjectile
    {



        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("SawBlade");     //The English name of the projectile
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;        //The recording mode
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.DemonScythe);
            projectile.width = 48;               //The width of projectile hitbox
            projectile.height = 48;              //The height of projectile hitbox
            projectile.friendly = true;         //Can the projectile deal damage to enemies?
            projectile.hostile = false;         //Can the projectile deal damage to the player?
            projectile.ranged = true;           //Is the projectile shoot by a ranged weapon?
            projectile.penetrate = 10;           //How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            projectile.alpha = 0;
            projectile.timeLeft = 870;
            projectile.light = 1f;            //How much light emit around the projectile
            projectile.ignoreWater = true;          //Does the projectile's speed be influenced by water?
            projectile.tileCollide = true;          //Can the projectile collide with tiles?
            projectile.extraUpdates = 1;            //Set to above 0 if you want the projectile to update multiple time in a frame
        }
        public override void AI()
        {
            if (projectile.timeLeft == 500)
            {
                projectile.velocity.Y = projectile.velocity.Y + 4.4f; // 0.1f for arrow gravity, 0.4f for knife gravity
                if (projectile.velocity.Y > 10000f) // This check implements "terminal velocity". We don't want the projectile to keep getting faster and faster. Past 16f this projectile will travel through blocks, so this check is useful.
                {
                    projectile.velocity.Y = 10000f;
                }
            }
        }
    }
}