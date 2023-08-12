using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Interfaces;
using System;
using System.Xml;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Projectiles
{
    public class TestTileOverlay : ModProjectile, ITileOverlay
    {
        public override string Texture => AssetDirectory.Textures + "SLIME";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("SLIME TEST");
        }

        public override void SetDefaults()
        {
            Projectile.width = 615;
            Projectile.height = 481;
            Projectile.timeLeft = 420;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
        }

        float counter = 0;
        public override void AI()
        {
            if (Projectile.timeLeft < 195)
                Projectile.friendly = false;
            counter += (float)(Math.PI / 2f) / 200;
        }

        public override bool PreDraw(ref Color lightColor) => false;
        public void DrawOverTiles(SpriteBatch spriteBatch)
        {
            Color color = Color.White;
            color *= (float)Math.Cos(counter);
            spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        }
    }
}