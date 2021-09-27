using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using System;

namespace OvermorrowMod.Items.Weapons.MechWep.MechProj
{
    public class MechMageProj : ModProjectile
    {

        int RandomX = Main.rand.Next(-5, 6);
        int RandomY = Main.rand.Next(-5, 6);

        int RandomX2 = Main.rand.Next(-5, 6);
        int RandomY2 = Main.rand.Next(-5, 6);

        int RandomX3 = Main.rand.Next(-5, 6);
        int RandomY3 = Main.rand.Next(-5, 6);

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.HeatRay);
            aiType = ProjectileID.HeatRay;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.timeLeft = 53;
            projectile.tileCollide = false;
        }
        public override string Texture => "Terraria/Projectile_" + ProjectileID.MagnetSphereBolt;

        public override void AI()
        {
            NPC npc = Main.npc[(int)projectile.ai[0]];
            projectile.Center = npc.Center;
            projectile.position.X = 50 * (float)Math.Cos(projectile.rotation) + Main.MouseWorld.X - 7;
            projectile.position.Y = 50 * (float)Math.Sin(projectile.rotation) + Main.MouseWorld.Y - 7;
            projectile.rotation += (float)((2 * Math.PI) / (Math.PI * 2 * 80 / 10)); // 200 is the speed, god only knows what dividing by 10 does
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            RandomX = Main.rand.Next(-5, 6);
            RandomY = Main.rand.Next(-5, 6);
            RandomX2 = Main.rand.Next(-3, 4);
            RandomY2 = Main.rand.Next(-3, 4);
            RandomX3 = Main.rand.Next(-8, 9);
            RandomY3 = Main.rand.Next(-8, 9);
            Vector2 value1 = new Vector2(0f, 0f);
            Projectile.NewProjectile(Main.MouseWorld.X, Main.MouseWorld.Y, value1.X + RandomX, value1.Y + RandomY, ProjectileID.MoonlordArrowTrail, projectile.damage, 3f, projectile.owner, 0f);
            Projectile.NewProjectile(Main.MouseWorld.X, Main.MouseWorld.Y, value1.X + RandomX2, value1.Y + RandomY2, ProjectileID.MoonlordArrowTrail, projectile.damage, 3f, projectile.owner, 0f);
            Projectile.NewProjectile(Main.MouseWorld.X, Main.MouseWorld.Y, value1.X + RandomX3, value1.Y + RandomY3, ProjectileID.MoonlordArrowTrail, projectile.damage, 3f, projectile.owner, 0f);
        }
    }
}