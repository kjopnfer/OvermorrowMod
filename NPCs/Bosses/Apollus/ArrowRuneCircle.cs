using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.IO;
using Terraria.Enums;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.NPCs.Bosses.Apollus
{
    public class ArrowRuneCircle : ModProjectile
    {
        public override string Texture => "OvermorrowMod/WardenClass/RuneCircles/temp2";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rune Circle");
        }
        public override void SetDefaults()
        {
            projectile.width = 70;
            projectile.height = 70;
            projectile.tileCollide = false;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.timeLeft = 1800;
            projectile.penetrate = -1;
        }
        public int timer = 0;
        public override void AI()
        {
            if (++timer % 25 == 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, Main.rand.Next(-4, 4), Main.rand.Next(-5, -3), ProjectileID.BoneArrow, 10, 1f);
                }
                /*
                Projectile.NewProjectile(projectile.Center.X - 10f, projectile.Center.Y, 0f, 10f, ProjectileID.BoneArrow, 10, 1f);
                Projectile.NewProjectile(projectile.Center.X + 10f, projectile.Center.Y, 0f, 10f, ProjectileID.BoneArrow, 10, 1f);
                Projectile.NewProjectile(projectile.Center.X - 5f, projectile.Center.Y + 3f, 0f, 10f, ProjectileID.BoneArrow, 10, 1f);
                Projectile.NewProjectile(projectile.Center.X + 5f, projectile.Center.Y + 3f, 0f, 10f, ProjectileID.BoneArrow, 10, 1f);
                */
            }
        }
    }
}
