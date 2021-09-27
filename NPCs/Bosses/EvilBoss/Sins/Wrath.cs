using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;



namespace OvermorrowMod.NPCs.Bosses.EvilBoss.Sins
{
    public class Wrath : ModProjectile
    {

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.LightDisc);
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.penetrate = 2;
            projectile.timeLeft = 85;
            projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            projectile.friendly = false;
            projectile.hostile = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Texture2D texture = mod.GetTexture("NPCs/Bosses/EvilBoss/Sins/Wrath");

                Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
                for (int k = 0; k < projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                    Color color = projectile.GetAlpha(Color.White) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                    spriteBatch.Draw(texture, drawPos, new Rectangle?(), color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
                }
            return true;
        }


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sin of Wrath");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }
    }
}