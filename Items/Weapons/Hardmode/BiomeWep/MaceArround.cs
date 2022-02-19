using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.Hardmode.BiomeWep
{
    public class MaceArround : ModProjectile
    {
        private const string ChainTexturePath = "OvermorrowMod/Items/Weapons/Hardmode/BiomeWep/Spine";
        int okay1 = 0;
        int okay2 = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Example Flail Ball"); // Set the projectile name to Example Flail Ball
        }

        public override void SetDefaults()
        {
            projectile.width = 102;
            projectile.height = 102;
            projectile.timeLeft = 26;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        // This AI code is adapted from the aiStyle 15. We need to re-implement this to customize the behavior of our flail
        public override void AI()
        {

            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, DustID.IceTorch, projectile.oldVelocity.X * 0.2f, projectile.oldVelocity.Y * 0.2f, 1, new Color(), 2.3f);
            }
            if (Main.MouseWorld.X < Main.player[projectile.owner].Center.X && okay2 < 1)
            {
                okay1++;
            }
            if (Main.MouseWorld.X > Main.player[projectile.owner].Center.X && okay1 < 1)
            {
                okay2++;
            }
            if (okay1 > 0)
            {
                NPC npc = Main.npc[(int)projectile.ai[0]];
                projectile.Center = npc.Center;
                projectile.position.X = 150 * (float)Math.Cos(projectile.rotation) + Main.player[projectile.owner].Center.X - 51f;
                projectile.position.Y = 150 * (float)Math.Sin(projectile.rotation) + Main.player[projectile.owner].Center.Y - 51f;
                projectile.rotation += (float)((2 * Math.PI) / (Math.PI * 2 * 40 / 10)); // 200 is the speed, god only knows what dividing by 10 does
            }
            if (okay2 > 0)
            {
                NPC npc = Main.npc[(int)projectile.ai[0]];
                projectile.Center = npc.Center;
                projectile.position.X = -150 * (float)Math.Cos(projectile.rotation) + Main.player[projectile.owner].Center.X - 51f;
                projectile.position.Y = -150 * (float)Math.Sin(projectile.rotation) + Main.player[projectile.owner].Center.Y - 51f;
                projectile.rotation += (float)((2 * Math.PI) / (Math.PI * 2 * -40 / 10)); // 200 is the speed, god only knows what dividing by 10 does
            }



        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(44, 350);
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
                drawPosition += remainingVectorToPlayer * 9 / length;
                remainingVectorToPlayer = mountedCenter - drawPosition;

                // Finally, we draw the texture at the coordinates using the lighting information of the tile coordinates of the chain section
                Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));
                spriteBatch.Draw(chainTexture, drawPosition - Main.screenPosition, null, color, rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            }

            return true;
        }
    }
}
