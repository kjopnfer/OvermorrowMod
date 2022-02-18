using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.NPCs.Bosses.StormDrake;
using OvermorrowMod.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.SandstormBoss
{
    public abstract class Ruin : ModProjectile
    {
        protected Vector2 InitialPosition;
        public override bool CanDamage() => projectile.ai[0] > 120;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Buried Ruin");
        }
        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindNPCsAndTiles.Add(index);
        }

        public override void AI()
        {
            if (projectile.ai[0]++ < 120)
            {
                projectile.Center -= Vector2.UnitY;
                InitialPosition = projectile.Center;
            }
            else
            {
                projectile.Center = Vector2.Lerp(InitialPosition, InitialPosition + Vector2.UnitY * 50, (float)Math.Sin(projectile.localAI[0]++ / 120f));
            }

            projectile.rotation = MathHelper.Lerp(0, MathHelper.PiOver4, (float)Math.Sin(projectile.localAI[0] / 240));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Color color = Color.Orange;
            float mult = (0.55f + (float)Math.Sin(Main.GlobalTime * 2) * 0.1f);
            float scale = projectile.scale * 2 * mult;

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, color, projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0f);

            return base.PreDraw(spriteBatch, lightColor);
        }

        /*public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * 200f, projectile.width, ref a);
        }*/
    }

    public class Ruin1 : Ruin
    {
        public override void SetDefaults()
        {
            projectile.width = 94;
            projectile.height = 102;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.timeLeft = 1200;
        }    
    }

    public class Ruin2 : Ruin
    {
        public override void SetDefaults()
        {
            projectile.width = 142;
            projectile.height = 132;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.timeLeft = 1200;
        }
    }

    public class Ruin3 : Ruin
    {
        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 60;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.timeLeft = 1200;
        }
    }
}