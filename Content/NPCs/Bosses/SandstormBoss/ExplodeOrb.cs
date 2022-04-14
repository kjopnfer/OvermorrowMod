using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    public class ExplodeOrb : ModProjectile
    {
        private float storeDirection;
        private float trigCounter = 0;
        private float period = 240;
        private float amplitude = 240;
        private float previousR = 0;

        private NPC npc;
        private bool RunOnce = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Volatile Orb");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 50;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.aiStyle = -1;
            projectile.timeLeft = 600;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
        }
        public Vector2 PolarVector(float radius, float theta)
        {
            return new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * radius;
        }

        public ref float AICounter => ref projectile.ai[0];
        public ref float AngleOffset => ref projectile.ai[1];
        public override void AI()
        {
            /*Vector2 move = Vector2.Zero;
            float distance = 4000f;
            bool target = false;

            foreach (Player player in Main.player)
            {
                if (!player.active || player.dead) continue;

                Vector2 newMove = player.Center - projectile.Center;
                float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                if (distanceTo < distance)
                {
                    move = player.Center;
                    distance = distanceTo;
                    target = true;
                }
            }
           
            if (target)
                projectile.Move(move, MathHelper.Lerp(6, 0, Utils.Clamp(projectile.ai[0]++, 0, 420) / 420f), 50);
            else
                projectile.velocity *= 0.98f;*/

            // This runs once when the projectile is created
            /*if (initProperties)
            {
                storeDirection = projectile.velocity.ToRotation();
                projectile.Center += Vector2.UnitX * 50 * projectile.ai[1];


                initProperties = false;
            }*/

            if (RunOnce)
            {
                npc = Main.npc[(int)projectile.ai[0]];

                AICounter = 0;

                RunOnce = false;
            }

            if (!npc.active) projectile.Kill();

            Player player = Main.player[npc.target];
            if (AICounter++ <= 120)
            {
                Vector2 direction = npc.DirectionTo(player.Center);
                float dist = 85f;

                projectile.Center = npc.Center + direction.RotatedBy(MathHelper.PiOver2 * AngleOffset) * dist;

                if (AICounter == 120)
                {
                    projectile.velocity = direction * 2;
                    storeDirection = projectile.velocity.ToRotation();
                }
            }
            else
            {
                trigCounter += 2 * (float)Math.PI / period;
                float r = amplitude * (float)Math.Sin(trigCounter) * AngleOffset;
                Vector2 instaVel = PolarVector(r - previousR, storeDirection + (float)Math.PI / 2);
                projectile.position += instaVel;
                previousR = r;
            }

            // Spawn one projectile at the NPC's center plus the direction given to the projectile that is towards the player's position
            // Have an offset that goes in one direction, rotated by the direction toward's the player's position

            /*trigCounter += 2 * (float)Math.PI / period;
            float r = amplitude * (float)Math.Sin(trigCounter) * (projectile.ai[0] == 0 ? 1 : -1);
            Vector2 instaVel = PolarVector(r - previousR, storeDirection + (float)Math.PI / 2);
            projectile.position += instaVel;
            previousR = r;
            projectile.rotation = (projectile.velocity + instaVel).ToRotation() + (float)Math.PI / 27;


            for (int num1103 = 0; num1103 < 2; num1103++)
            {
                int num1106 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.FireworkFountain_Blue, projectile.velocity.X, projectile.velocity.Y, 50, default(Color), 0.4f);
                switch (num1103)
                {
                    case 0:
                        Main.dust[num1106].position = (Main.dust[num1106].position + projectile.Center * 5f) / 6f;
                        break;
                    case 1:
                        Main.dust[num1106].position = (Main.dust[num1106].position + (projectile.Center + projectile.velocity / 2f) * 5f) / 6f;
                        break;
                }
                Dust dust81 = Main.dust[num1106];
                dust81.velocity *= 0.1f;
                Main.dust[num1106].noGravity = true;
                Main.dust[num1106].fadeIn = 1f;
            }

            float radius = 30;
            projectile.ai[1] += projectile.ai[0] == 0 ? 16 : -16;*/

            projectile.rotation += 0.1f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture(AssetDirectory.Boss + "SandstormBoss/ExplodeOrb_Trail");

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            var off = new Vector2(projectile.width / 2f, projectile.height / 2f);
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            var frame = new Rectangle(0, frameHeight * projectile.frame, texture.Width, frameHeight - 2);
            var orig = frame.Size() / 2f;

            Color color = Color.Yellow;
            var trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
            var fadeMult = 1f / trailLength;
            for (int i = 1; i < trailLength; i++)
            {
                Main.spriteBatch.Draw(texture, projectile.oldPos[i] - Main.screenPosition + off, frame, color * (1f - fadeMult * i), projectile.oldRot[i], orig, projectile.scale * (trailLength - i) / trailLength, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            /*spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), new Color(244, 188, 91), projectile.rotation, drawOrigin, projectile.scale * 0.4f, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), new Color(244, 188, 91), projectile.rotation, drawOrigin, projectile.scale * 0.4f, SpriteEffects.None, 0);

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), new Color(244, 188, 91), projectile.rotation + MathHelper.PiOver2, drawOrigin, new Vector2(0.15f, 1f), SpriteEffects.None, 0);
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), new Color(244, 188, 91), projectile.rotation, drawOrigin, new Vector2(0.15f, 1f), SpriteEffects.None, 0);*/


            return true;
        }
    }
}