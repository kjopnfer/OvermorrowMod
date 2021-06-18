using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using System;

namespace OvermorrowMod.Items.Weapons.MechWep.MechProj
{
    public class MechMageProjLaser : ModProjectile
    {

        int RandomX = Main.rand.Next(-5, 6);
        int RandomY = Main.rand.Next(-5, 6);

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.LaserMachinegunLaser);
            aiType = ProjectileID.LaserMachinegunLaser;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.timeLeft = 50;
            projectile.tileCollide = false;
        }
        
        public override string Texture => "Terraria/Projectile_" + ProjectileID.LaserMachinegunLaser;
    }
}