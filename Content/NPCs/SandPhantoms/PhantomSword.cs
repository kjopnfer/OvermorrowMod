using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.NPCs.Bosses.DripplerBoss;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.SandPhantoms
{
    public class PhantomSword : ModProjectile
    {
        private Vector2 InitialPosition;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scimitar");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 50;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
        }

        public ref float AICounter => ref projectile.ai[0];
        public ref float ParentID => ref projectile.ai[1];
        public override void AI()
        {
            projectile.rotation += .55f * projectile.direction;

            NPC ParentNPC = Main.npc[(int)ParentID];
            if (ParentNPC.active)
            {
                projectile.timeLeft = 5;
            }

            Player target = null;
            if (AICounter++ < 300)
            {
                foreach (Player player in Main.player)
                {
                    if (!player.active || projectile.Distance(player.Center) > 2000) continue;

                    target = player;
                }

                if (target != null)
                {
                    projectile.direction = target.Center.X < projectile.Center.X ? 1 : -1;

                    projectile.velocity.X = MathHelper.Lerp(projectile.velocity.X, (target.Center.X > projectile.Center.X ? 1 : -1) * 4, 0.05f);
                    projectile.velocity.Y = MathHelper.Lerp(projectile.velocity.Y, target.Center.Y > projectile.Center.Y ? 2.5f : -2.5f, 0.02f);
                }
                else
                {
                    projectile.velocity = Vector2.Zero;
                }
            }
            else if (AICounter >= 300 && AICounter <= 360)
            {
                projectile.velocity = Vector2.Zero;
                InitialPosition = projectile.Center;
            }
            else
            {
                //projectile.velocity.X = MathHelper.Lerp(projectile.velocity.X, (ParentNPC.Center.X > projectile.Center.X ? 1 : -1) * 6, 0.05f);
                //projectile.velocity.Y = MathHelper.Lerp(projectile.velocity.Y, ParentNPC.Center.Y > projectile.Center.Y ? 2.5f : -2.5f, 0.02f);

                projectile.Center = Vector2.Lerp(InitialPosition, ParentNPC.Center, (AICounter++ - 360) / 60f);
                projectile.direction = ParentNPC.Center.X < projectile.Center.X ? 1 : -1;

                if (projectile.Hitbox.Intersects(ParentNPC.Hitbox))
                {
                    ((SandPhantom)ParentNPC.modNPC).ThrewSword = false;
                    projectile.Kill();
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            
            projectile.spriteDirection = projectile.direction;

            Texture2D texture = ModContent.GetTexture(AssetDirectory.NPC + "SandPhantoms/PhantomSword_Trail");
            var spriteEffects = projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Reload(BlendState.Additive);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color = Color.Yellow;
                color *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 oldPosition = projectile.oldPos[i];
                float oldRotation = projectile.oldRot[i];

                Main.spriteBatch.Draw(texture, oldPosition + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), null, color, oldRotation, texture.Size() / 2f, projectile.scale, spriteEffects, 0f);
            }
            spriteBatch.Reload(BlendState.AlphaBlend);

            texture = Main.projectileTexture[projectile.type];
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation, texture.Size() / 2, projectile.scale, spriteEffects, 0f);

            return false;
        }
    }
}