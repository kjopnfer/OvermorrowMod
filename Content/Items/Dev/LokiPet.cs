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
            
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 42;
            projectile.CloneDefaults(ProjectileID.BlackCat);

            aiType = ProjectileID.BlackCat;
        }

        public override bool PreAI()
        {
            Player player = Main.player[projectile.owner];
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            if (!player.dead && player.HasBuff(ModContent.BuffType<LokiPetBuff>()))
            {
                projectile.timeLeft = 2;
            }
        }
    }
}
