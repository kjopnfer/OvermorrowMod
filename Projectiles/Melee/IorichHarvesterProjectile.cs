using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Melee
{
    public class IorichHarvesterProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Harvester of Iorich");
            // afterimage effect
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.PaladinsHammerFriendly);
            projectile.width = 72;
            projectile.height = 52;
            projectile.aiStyle = 3;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.tileCollide = true;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 1f, 0f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(Color.Green) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, new Rectangle?(), color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}