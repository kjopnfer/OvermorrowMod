using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Melee
{
    public class HellBoomerang : ModProjectile
    {
        private bool ComingBack = false;
        Vector2 endPoint;
        private const string ChainTexturePath = "OvermorrowMod/Projectiles/Melee/HellBoomerangDraw";
        Vector2 SavedMove;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hell Boomerang");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.timeLeft = 100;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref projectile.velocity);
                projectile.localAI[0] = 1f;
            }


            Vector2 move = Vector2.Zero;
            float distance = 300f;
            bool target = false;
            for (int k = 0; k < 200; k++)
            {
                if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5)
                {
                    Vector2 newMove = Main.npc[k].Center - projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance)
                    {
                        SavedMove = Main.npc[k].Center;
                        move = newMove;
                        distance = distanceTo;
                        target = true;
                    }
                }
            }
            if (target && !ComingBack)
            {
                endPoint = SavedMove;
                AdjustMagnitude(ref move);
                projectile.velocity = (10 * projectile.velocity + move) / 11f;
                AdjustMagnitude(ref projectile.velocity);
                float BetweenComeBack = Vector2.Distance(SavedMove, projectile.Center);
                if (BetweenComeBack < 42)
                {
                    Main.PlaySound(SoundID.Shatter, projectile.position);
                    ComingBack = true;
                }
            }
            else
            {
                endPoint = projectile.Center;
            }


            projectile.rotation += 0.36f;

            if (projectile.timeLeft < 65 && !target)
            {
                projectile.timeLeft = 10;
                ComingBack = true;
            }

            if (projectile.timeLeft > 98)
            {
                projectile.tileCollide = false;
            }
            else if (!ComingBack)
            {
                projectile.tileCollide = true;
            }

            if (ComingBack)
            {
                float BetweenKill = Vector2.Distance(Main.player[projectile.owner].Center, projectile.Center);
                projectile.tileCollide = false;
                Vector2 position = projectile.Center;
                Vector2 targetPosition = Main.player[projectile.owner].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                projectile.velocity = direction * 18;
                if (BetweenKill < 32)
                {
                    projectile.Kill();
                }
            }
        }



        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // if(!target) this is always true here
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Shatter, projectile.position);
            {
                ComingBack = true;
            }

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, endPoint, 4f, ref point);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Texture2D chainTexture = ModContent.GetTexture(ChainTexturePath);
            projectile.alpha = 0;
            var drawPosition = projectile.Center;

            Vector2 unit = endPoint - projectile.Center; // changing all endpoints it just how you change it, dont change other stuff it wont go well
            float length = unit.Length();
            unit.Normalize();
            for (float k = 0; k <= length; k += 10f)
            {
                Vector2 drawPos = projectile.Center + unit * k - Main.screenPosition;
                Color alpha = Color.Orange;
                Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));

                spriteBatch.Draw(chainTexture, drawPos, null, color, (SavedMove - projectile.Center).ToRotation(), new Vector2(5, 5), 1f, SpriteEffects.None, 0f);
            }

            return true;
        }

        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 11f)
            {
                vector *= 11f / magnitude;
            }
        }
    }
}