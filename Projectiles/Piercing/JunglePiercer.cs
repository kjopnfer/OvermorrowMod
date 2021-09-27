using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Projectiles.Piercing
{
    public class JunglePiercer : ModProjectile
    {
        private const string ChainTexturePath = "OvermorrowMod/Projectiles/Piercing/VinePiercerChain";
        public override string Texture => "OvermorrowMod/Projectiles/Piercing/VinePiercerProjectile";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stingvine");
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {

            projectile.width = 22;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = 4;
            projectile.timeLeft = 240;
            projectile.extraUpdates = 1;
            projectile.ignoreWater = true;
        }


        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);

            float between = Vector2.Distance(Main.player[projectile.owner].Center, projectile.Center);
            if (between > 350)
            {

                Vector2 position = projectile.Center;
                Vector2 targetPosition = Main.player[projectile.owner].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                projectile.velocity = direction * 4.6f;
            }

            Player player = Main.player[projectile.owner];

            //projectile.rotation = (player.Center - projectile.Center).ToRotation();

            if (Main.player[projectile.owner].channel)
            {
                if (Main.MouseWorld.X < projectile.position.X)
                {
                    projectile.velocity.X -= 0.065f; // accelerate to the left
                }
                if (Main.MouseWorld.X > projectile.position.X)
                {
                    projectile.velocity.X += 0.065f; // accelerate to the right
                }
                if (Main.MouseWorld.Y < projectile.position.Y)
                {
                    projectile.velocity.Y -= 0.065f; // accelerate to the down
                }
                if (Main.MouseWorld.Y > projectile.position.Y)
                {
                    projectile.velocity.Y += 0.065f; // accelerate to the up
                }
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

            // This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
            while (true)
            {
                float length = remainingVectorToPlayer.Length();

                if (length < 25f || float.IsNaN(length))
                    break;

                // drawPosition is advanced along the vector back to the player by 12 pixels
                // 12 comes from the height of ExampleFlailProjectileChain.png and the spacing that we desired between links
                drawPosition += remainingVectorToPlayer * 28 / length;
                remainingVectorToPlayer = mountedCenter - drawPosition;

                // Finally, we draw the texture at the coordinates using the lighting information of the tile coordinates of the chain section
                Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));
                spriteBatch.Draw(chainTexture, drawPosition - Main.screenPosition, null, color, rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            }

            return true;
        }
    }
}