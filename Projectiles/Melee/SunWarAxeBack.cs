using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Melee
{
    public class SunWarAxeBack : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Melee/SunWarAxeProj";
        private const string ChainTexturePath = "OvermorrowMod/Projectiles/Melee/SunDraw";
        int StarTimer = 0;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sun Axe");
        }

        public override void SetDefaults()
        {

            projectile.CloneDefaults(ProjectileID.WoodenBoomerang);
            projectile.width = 22;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.aiStyle = 3;
            projectile.timeLeft = 600;
            projectile.light = 1f;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.extraUpdates = 1;

        }

        public override void AI()
        {
            StarTimer++;
            if (StarTimer == 5)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<StarAxe>(), projectile.damage / 2, 3f, projectile.owner, 0f);
                StarTimer = 0;
            }
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var player = Main.player[projectile.owner];

            Vector2 mountedCenter = player.Center;
            Texture2D chainTexture = ModContent.GetTexture(ChainTexturePath);

            var drawPosition = projectile.Center;
            var remainingVectorToPlayer = mountedCenter - drawPosition;

            float rotation = remainingVectorToPlayer.ToRotation() - MathHelper.PiOver2;

            if (projectile.alpha == 0)
            {
                int direction = -1;

                if (projectile.Center.X < mountedCenter.X)
                    direction = 1;

                player.itemRotation = (float)Math.Atan2(remainingVectorToPlayer.Y * direction, remainingVectorToPlayer.X * direction);
            }

            // This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
            while (true)
            {
                float length = remainingVectorToPlayer.Length();

                if (length < 25f || float.IsNaN(length))
                    break;

                // drawPosition is advanced along the vector back to the player by 12 pixels
                // 12 comes from the height of ExampleFlailProjectileChain.png and the spacing that we desired between links
                drawPosition += remainingVectorToPlayer * 12 / length;
                remainingVectorToPlayer = mountedCenter - drawPosition;

                // Finally, we draw the texture at the coordinates using the lighting information of the tile coordinates of the chain section
                Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));
                spriteBatch.Draw(chainTexture, drawPosition - Main.screenPosition, null, color, (Main.player[projectile.owner].Center - projectile.Center).ToRotation(), new Vector2(5f, 5f), 1f, SpriteEffects.None, 0f);
            }

            return true;
        }
    }
}
