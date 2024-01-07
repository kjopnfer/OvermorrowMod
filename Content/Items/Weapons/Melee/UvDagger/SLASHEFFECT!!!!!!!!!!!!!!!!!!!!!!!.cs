using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Content.Items.Weapons.Melee
{
    public class SlashEffect : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Content/Items/Weapons/Melee/UvDagger/SLASHEFFECT!!!!!!!!!!!!!!!!!!!!!!!";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8; //7?
        }
        public override void SetDefaults()
        {
            Projectile.scale = 0.75f;
            Projectile.width = 168;
            Projectile.height = 26;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied);
            return base.PreDraw(ref lightColor);
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        
        public override void AI()
        {
            Projectile.rotation = Projectile.ai[1];
            if (Projectile.ai[0] % 2 == 0)
                Projectile.frame++;
            if (Projectile.frame == 7)
                Projectile.Kill();
            Projectile.ai[0]++;
        }
    }
}