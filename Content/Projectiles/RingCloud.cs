using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace OvermorrowMod.Content.Projectiles
{
    public class RingCloud : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
        }
        
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 38;
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
        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 255 - Projectile.alpha);

        public override void AI()
        {
            Projectile.rotation = Projectile.ai[1];
            if (Projectile.ai[0] % 2 == 0)
            {
                if (Projectile.frame == 7)
                    Projectile.Kill();
                Projectile.frame++;
            }
            Projectile.alpha += 9;
            //Projectile.scale += 0.025f;
            Projectile.ai[0]++;
        }
        /*public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            base.DrawBehind(index, drawCacheProjsBehindNPCsAndTiles, drawCacheProjsBehindNPCs, drawCacheProjsBehindProjectiles, drawCacheProjsOverWiresUI);
        }*/
    }
}