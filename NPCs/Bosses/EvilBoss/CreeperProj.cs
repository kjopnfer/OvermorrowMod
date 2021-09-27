using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using System;

namespace OvermorrowMod.NPCs.Bosses.EvilBoss
{
    public class CreeperProj : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.FlamingScythe);
            aiType = ProjectileID.FlamingScythe;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.width = 18;
            projectile.height = 18;
            projectile.timeLeft = 60;
        }
    }
}