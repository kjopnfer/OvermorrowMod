using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.JungleBomber
{
    public class JungleBomb : ModProjectile
    {
        public override bool CanDamage() => false;
        private int timer = 0;
        private const string ChainTexturePath = "OvermorrowMod/NPCs/PostRider/Gore68";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Atama Blade");
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 68;
            projectile.height = 46;

            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 240;
            projectile.light = 0.75f;
            projectile.extraUpdates = 1;
            projectile.ignoreWater = true;
        }


        public override void AI()
        {


            timer++;
            if (timer == 17)
            {
                Main.PlaySound(SoundID.Item62, projectile.Center);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X / 7, projectile.velocity.Y / 7, mod.ProjectileType("JungleEXP"), projectile.damage + 10, 1f, projectile.owner, 0f);
                timer = 0;
            }


            Player player = Main.player[projectile.owner];
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(0f);
            if (Main.player[projectile.owner].channel)
            {
                if (Main.MouseWorld.X < projectile.position.X)
                {
                    projectile.velocity.X -= 0.1f; // accelerate to the left
                }
                if (Main.MouseWorld.X > projectile.position.X)
                {
                    projectile.velocity.X += 0.1f; // accelerate to the right
                }
                if (Main.MouseWorld.Y < projectile.position.Y)
                {
                    projectile.velocity.Y -= 0.1f; // accelerate to the down
                }
                if (Main.MouseWorld.Y > projectile.position.Y)
                {
                    projectile.velocity.Y += 0.1f; // accelerate to the up
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