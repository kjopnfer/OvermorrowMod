using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Biome
{
    public class TestHook : ModProjectile
    {

        private int SavedDMG = 0;
        private int timer = 0;
        private bool ComingBack = false;
        private int flametimer = 0;
        Vector2 DrawToPos;
        private const string ChainTexturePath = "OvermorrowMod/NPCs/Biome/GranNPCChain";

        public override void SetDefaults()
        {
            projectile.width = 44;
            projectile.height = 44;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.melee = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Clamper");
            Main.projFrames[base.projectile.type] = 2;
        }

        public override void AI()
        {


            NPC npc = Main.npc[(int)projectile.ai[1]];

            if(npc.active)
            {
                projectile.timeLeft = 3;
            }
            DrawToPos = npc.Center;
            npc.rotation = (npc.Center - Main.player[projectile.owner].Center).ToRotation() + MathHelper.ToRadians(-90f);

            timer++;
            if(timer == 100)
            {
                Vector2 position = projectile.Center;
                Vector2 targetPosition = Main.player[projectile.owner].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                projectile.velocity = direction * 5f;
                Main.PlaySound(2, npc.position, 99);
                timer = 0;
            }
            
            if(npc.velocity.X < 1 && npc.velocity.X > -1)
            {
                projectile.rotation = (projectile.Center - Main.player[projectile.owner].Center).ToRotation() + MathHelper.ToRadians(-90f);
                projectile.frame = 0;
            }
            else
            {
                projectile.rotation = (npc.Center - projectile.Center).ToRotation() + MathHelper.ToRadians(-90f);
                projectile.frame = 1;
            }


            if(Vector2.Distance(npc.Center, projectile.Center) < 77)
            {
                npc.velocity *= 0f;
            }

            if(timer > 25 && timer < 46)
            {
                projectile.velocity *= 0.4f;

                Vector2 position = npc.Center;
                Vector2 targetPosition = projectile.Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                npc.velocity = direction * 7f;
            }


			if (!Main.npc[(int)projectile.ai[1]].active) 
            {
				projectile.Kill();
				projectile.active = false;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var player = Main.player[projectile.owner];

            Vector2 mountedCenter = DrawToPos;
            Texture2D chainTexture = ModContent.GetTexture(ChainTexturePath);

            var drawPosition = projectile.Center;
            var remainingVectorToPlayer = mountedCenter - drawPosition;

            float rotation = remainingVectorToPlayer.ToRotation() - MathHelper.PiOver2;

            if (projectile.alpha == 0)
            {
                int direction = -1;

                if (projectile.Center.X < mountedCenter.X)
                {
                    direction = 1;
                }

            }

            // This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
            while (true)
            {
                float length = remainingVectorToPlayer.Length();

                if (length < 25f || float.IsNaN(length))
                    break;

                // drawPosition is advanced along the vector back to the player by 12 pixels
                // 12 comes from the height of ExampleFlailProjectileChain.png and the spacing that we desired between links
                drawPosition += remainingVectorToPlayer * 35 / length;
                remainingVectorToPlayer = mountedCenter - drawPosition;

                // Finally, we draw the texture at the coordinates using the lighting information of the tile coordinates of the chain section
                Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));
                spriteBatch.Draw(chainTexture, drawPosition - Main.screenPosition, null, color, (DrawToPos - projectile.Center).ToRotation(), chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            }

            return true;
        }
    }
}



