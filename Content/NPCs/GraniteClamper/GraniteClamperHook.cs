using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.GraniteClamper
{
    public class GraniteClamperHook : ModProjectile
    {
        private int timer = 0;
        Vector2 DrawToPos;
        private const string ChainTexturePath = "OvermorrowMod/Content/NPCs/GraniteClamper/GranNPCChain";

        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Clamper");
            Main.projFrames[base.Projectile.type] = 2;
        }

        public override void AI()
        {


            NPC npc = Main.npc[(int)Projectile.ai[1]];

            if (npc.active)
            {
                Projectile.timeLeft = 3;
            }
            DrawToPos = npc.Center;
            npc.rotation = (npc.Center - Main.player[Projectile.owner].Center).ToRotation() + MathHelper.ToRadians(-90f);

            timer++;
            if (timer == 100)
            {
                Vector2 position = Projectile.Center;
                Vector2 targetPosition = Main.player[Projectile.owner].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                Projectile.velocity = direction * 5f;
                SoundEngine.PlaySound(SoundID.Item99, npc.position);
                timer = 0;
            }

            if (npc.velocity.X < 1 && npc.velocity.X > -1)
            {
                Projectile.rotation = (Projectile.Center - Main.player[Projectile.owner].Center).ToRotation() + MathHelper.ToRadians(-90f);
                Projectile.frame = 0;
            }
            else
            {
                Projectile.rotation = (npc.Center - Projectile.Center).ToRotation() + MathHelper.ToRadians(-90f);
                Projectile.frame = 1;
            }


            if (Vector2.Distance(npc.Center, Projectile.Center) < 77)
            {
                npc.velocity *= 0f;
            }

            if (timer > 25 && timer < 46)
            {
                Projectile.velocity *= 0.4f;

                Vector2 position = npc.Center;
                Vector2 targetPosition = Projectile.Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                npc.velocity = direction * 7f;
            }


            if (!Main.npc[(int)Projectile.ai[1]].active)
            {
                Projectile.Kill();
                Projectile.active = false;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 mountedCenter = DrawToPos;
            Texture2D chainTexture = ModContent.Request<Texture2D>(ChainTexturePath).Value;

            var drawPosition = Projectile.Center;
            var remainingVectorToPlayer = mountedCenter - drawPosition;

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
                Main.EntitySpriteDraw(chainTexture, drawPosition - Main.screenPosition, null, color, (DrawToPos - Projectile.Center).ToRotation(), chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}



