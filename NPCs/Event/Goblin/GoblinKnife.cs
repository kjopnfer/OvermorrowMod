using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Event.Goblin
{
    public class GoblinKnife : ModProjectile
    {
        private const string ChainTexturePath = "OvermorrowMod/NPCs/Event/Goblin/GoblinKnifeChain";
        private int timer = 0;

        bool right = false;
        bool left = false;

        int goright = 0;

        bool comebackright = false;
        bool comebackleft = false;

        Vector2 DrawToPos;

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.timeLeft = 1000;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        // This AI code is adapted from the aiStyle 15. We need to re-implement this to customize the behavior of our flail
        public override void AI()
        {

            NPC npc = Main.npc[(int)projectile.ai[1]];
            DrawToPos = npc.Center;
            if (npc.active)
            {
                projectile.timeLeft = 3;
            }

            float BetweenKill = Vector2.Distance(npc.Center, projectile.Center);


            projectile.rotation = (npc.Center - projectile.Center).ToRotation();
            timer++;
            if (timer == 1)
            {
                if (Main.player[projectile.owner].Center.X > npc.Center.X)
                {
                    right = true;
                }
                else
                {
                    left = true;
                }
            }

            projectile.position.Y = npc.Center.Y - projectile.height / 2;

            if (right)
            {
                goright += 5;
                projectile.position.X = npc.Center.X - projectile.width / 2 + goright;
            }

            if (left)
            {
                goright -= 5;
                projectile.position.X = npc.Center.X - projectile.width / 2 + goright;
            }

            if (projectile.Center.X > npc.Center.X + 150)
            {
                comebackright = true;
                right = false;
            }

            if (projectile.Center.X < npc.Center.X - 150)
            {
                comebackleft = true;
                left = false;
            }


            if (comebackright)
            {
                goright -= 10;
                projectile.position.X = npc.Center.X - projectile.width / 2 + goright;
                if (BetweenKill < 22)
                {
                    projectile.Kill();
                }
            }

            if (comebackleft)
            {
                goright += 10;
                projectile.position.X = npc.Center.X - projectile.width / 2 + goright;
                if (BetweenKill < 22)
                {
                    projectile.Kill();
                }
            }



        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, DrawToPos, 4f, ref point);
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Vector2 mountedCenter = DrawToPos;
            Texture2D chainTexture = ModContent.GetTexture(ChainTexturePath);

            var drawPosition = projectile.Center;
            var remainingVectorToPlayer = mountedCenter - drawPosition;

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
                spriteBatch.Draw(chainTexture, drawPosition - Main.screenPosition, null, color, (DrawToPos - projectile.Center).ToRotation(), chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            }

            return true;
        }
    }
}


