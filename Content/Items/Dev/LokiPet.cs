using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.Content.Items.Dev
{
    public class LokiPet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Loki");
            
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.CloneDefaults(ProjectileID.BlackCat);

            AIType = ProjectileID.BlackCat;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!player.dead && player.HasBuff(ModContent.BuffType<LokiPetBuff>()))
            {
                Projectile.timeLeft = 2;
            }
        }
    }
}
