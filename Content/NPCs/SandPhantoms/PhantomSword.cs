using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.SandPhantoms
{
    public class PhantomSword : ModProjectile
    {
        private Vector2 InitialPosition;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Scimitar");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 50;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }

        public ref float AICounter => ref Projectile.ai[0];
        public ref float ParentID => ref Projectile.ai[1];
        public override void AI()
        {
            Projectile.rotation += .55f * Projectile.direction;

            NPC ParentNPC = Main.npc[(int)ParentID];
            if (ParentNPC.active)
            {
                Projectile.timeLeft = 5;
            }

            Player target = null;
            if (AICounter++ < 300)
            {
                foreach (Player player in Main.player)
                {
                    if (!player.active || Projectile.Distance(player.Center) > 2000) continue;

                    target = player;
                }

                if (target != null)
                {
                    Projectile.direction = target.Center.X < Projectile.Center.X ? 1 : -1;

                    Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, (target.Center.X > Projectile.Center.X ? 1 : -1) * 4, 0.05f);
                    Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, target.Center.Y > Projectile.Center.Y ? 2.5f : -2.5f, 0.02f);
                }
                else
                {
                    Projectile.velocity = Vector2.Zero;
                }
            }
            else if (AICounter >= 300 && AICounter <= 360)
            {
                Projectile.velocity = Vector2.Zero;
                InitialPosition = Projectile.Center;
            }
            else
            {
                //projectile.velocity.X = MathHelper.Lerp(projectile.velocity.X, (ParentNPC.Center.X > projectile.Center.X ? 1 : -1) * 6, 0.05f);
                //projectile.velocity.Y = MathHelper.Lerp(projectile.velocity.Y, ParentNPC.Center.Y > projectile.Center.Y ? 2.5f : -2.5f, 0.02f);

                Projectile.Center = Vector2.Lerp(InitialPosition, ParentNPC.Center, (AICounter++ - 360) / 60f);
                Projectile.direction = ParentNPC.Center.X < Projectile.Center.X ? 1 : -1;

                if (Projectile.Hitbox.Intersects(ParentNPC.Hitbox))
                {
                    ((SandPhantom)ParentNPC.ModNPC).ThrewSword = false;
                    Projectile.Kill();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Projectile.spriteDirection = Projectile.direction;

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.NPC + "SandPhantoms/PhantomSword_Trail").Value;
            var spriteEffects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Reload(BlendState.Additive);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color = Color.Yellow;
                color *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 oldPosition = Projectile.oldPos[i];
                float oldRotation = Projectile.oldRot[i];

                Main.spriteBatch.Draw(texture, oldPosition + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, color, oldRotation, texture.Size() / 2f, Projectile.scale, spriteEffects, 0f);
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2, Projectile.scale, spriteEffects, 0);

            return false;
        }
    }
}